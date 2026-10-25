using System.Data;
using Microsoft.Data.Sqlite;
using SistemaVeterinario.Dominio;

namespace SistemaVeterinario.Persistencia;

internal sealed class RepositorioVeterinario
{
    private readonly string _cadena;
    public RepositorioVeterinario(string cadena) => _cadena = cadena;

    private SqliteConnection Abrir()
    {
        var c = new SqliteConnection(_cadena);
        c.Open();
        return c;
    }

    private DataTable Consultar(string sql, params (string, object)[] p)
    {
        using var cn = Abrir();
        using var cmd = cn.CreateCommand();
        cmd.CommandText = sql;
        foreach (var (n, v) in p) cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);
        using var rd = cmd.ExecuteReader();
        var t = new DataTable();
        t.Load(rd);
        return t;
    }

    private void Ejecutar(string sql, params (string, object)[] p)
    {
        using var cn = Abrir();
        using var cmd = cn.CreateCommand();
        cmd.CommandText = sql;
        foreach (var (n, v) in p) cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public DataTable Login(string usuario, string contrasena) => Consultar(
        @"SELECT Id, Tipo, Nombre, Usuario FROM Personas
          WHERE Usuario = $u AND Contrasena = $c;",
        ("$u", usuario), ("$c", contrasena));

    public DataTable ObtenerClientes() => Consultar(
        "SELECT Id, Nombre, Dpi, Telefono, Email, Nit FROM Personas WHERE Tipo='Cliente' ORDER BY Nombre;");

    public void CrearCliente(Cliente c) => Ejecutar(
        @"INSERT INTO Personas (Tipo, Nombre, Dpi, Telefono, Email, Direccion, Nit)
          VALUES ('Cliente', $n, $d, $t, $e, $dir, $nit);",
        ("$n", c.Nombre), ("$d", c.Dpi), ("$t", c.Telefono),
        ("$e", c.Email), ("$dir", c.Direccion), ("$nit", c.Nit));

    public void ActualizarCliente(int id, Cliente c) => Ejecutar(
        @"UPDATE Personas SET Nombre=$n, Dpi=$d, Telefono=$t, Email=$e, Direccion=$dir, Nit=$nit
          WHERE Id=$id AND Tipo='Cliente';",
        ("$n", c.Nombre), ("$d", c.Dpi), ("$t", c.Telefono),
        ("$e", c.Email), ("$dir", c.Direccion), ("$nit", c.Nit), ("$id", id));

    public void EliminarCliente(int id) => Ejecutar(
        "DELETE FROM Personas WHERE Id=$id AND Tipo='Cliente';", ("$id", id));

    public DataTable ObtenerVeterinarios() => Consultar(
        "SELECT Id, Nombre, Especialidad, Horario FROM Personas WHERE Tipo='Veterinario' ORDER BY Nombre;");

    public DataTable ObtenerMascotas() => Consultar(
        @"SELECT m.Id, m.Nombre, m.Especie, m.Raza, m.Peso, p.Nombre AS Propietario
          FROM Mascotas m INNER JOIN Personas p ON p.Id = m.PropietarioId
          ORDER BY m.Nombre;");

    public DataTable ObtenerMascotasPorCliente(int clienteId) => Consultar(
        "SELECT Id, Nombre, Especie FROM Mascotas WHERE PropietarioId=$id;",
        ("$id", clienteId));

    public void CrearMascota(Mascota m) => Ejecutar(
        @"INSERT INTO Mascotas (Nombre, Especie, Raza, Sexo, FechaNacimiento, Peso, PropietarioId)
          VALUES ($n, $e, $r, $s, $f, $p, $prop);",
        ("$n", m.Nombre), ("$e", m.Especie), ("$r", m.Raza), ("$s", m.Sexo),
        ("$f", m.FechaNacimiento.ToString("yyyy-MM-dd")),
        ("$p", m.Peso), ("$prop", m.PropietarioId));

    public DataTable ObtenerCitas() => Consultar(
        @"SELECT c.Id, c.Fecha, c.Hora, c.Motivo, c.Estado,
                 cl.Nombre AS Cliente, m.Nombre AS Mascota, v.Nombre AS Veterinario
          FROM Citas c
          INNER JOIN Personas cl ON cl.Id = c.ClienteId
          INNER JOIN Mascotas m ON m.Id = c.MascotaId
          INNER JOIN Personas v ON v.Id = c.VeterinarioId
          ORDER BY c.Fecha DESC, c.Hora;");
          public System.Data.DataRow? ObtenerCitaPorId(int id)
{
    var t = Consultar(
        "SELECT Id, Fecha, Hora, Motivo, Estado, ClienteId, MascotaId, VeterinarioId " +
        "FROM Citas WHERE Id=$id;",
        ("$id", id));
    return t.Rows.Count > 0 ? t.Rows[0] : null;
}

    public bool VeterinarioDisponible(int vetId, DateTime fecha, string hora)
    {
        var r = Consultar(
            @"SELECT COUNT(*) AS C FROM Citas
              WHERE VeterinarioId=$v AND Fecha=$f AND Hora=$h AND Estado<>'Cancelada';",
            ("$v", vetId), ("$f", fecha.ToString("yyyy-MM-dd")), ("$h", hora));
        return Convert.ToInt32(r.Rows[0]["C"]) == 0;
    }

    public void CrearCita(Cita c)
    {
        if (!VeterinarioDisponible(c.VeterinarioId, c.Fecha, c.Hora))
            throw new InvalidOperationException("El veterinario no está disponible en ese horario.");
        Ejecutar(
            @"INSERT INTO Citas (Fecha, Hora, Motivo, Estado, ClienteId, MascotaId, VeterinarioId)
              VALUES ($f, $h, $m, $e, $cli, $mas, $vet);",
            ("$f", c.Fecha.ToString("yyyy-MM-dd")), ("$h", c.Hora), ("$m", c.Motivo),
            ("$e", c.Estado), ("$cli", c.ClienteId), ("$mas", c.MascotaId), ("$vet", c.VeterinarioId));
    }

    public void CancelarCita(int id) => Ejecutar(
        "UPDATE Citas SET Estado='Cancelada' WHERE Id=$id;", ("$id", id));

    public void CompletarCita(int id) => Ejecutar(
        "UPDATE Citas SET Estado='Completada' WHERE Id=$id;", ("$id", id));

    public DataTable ObtenerHistorial(int mascotaId) => Consultar(
        @"SELECT Id, Fecha, Motivo, Diagnostico, Gravedad, Tratamiento
          FROM Consultas WHERE MascotaId=$id ORDER BY Fecha DESC;",
        ("$id", mascotaId));

    public void CrearConsulta(Consulta c) => Ejecutar(
        @"INSERT INTO Consultas (Fecha, Motivo, Diagnostico, Gravedad, Tratamiento,
                                 DuracionDias, Medicamentos, MascotaId, CitaId)
          VALUES ($f, $m, $d, $g, $t, $dur, $med, $mas, $cita);",
        ("$f", c.Fecha.ToString("O")), ("$m", c.Motivo), ("$d", c.Diagnostico),
        ("$g", c.Gravedad), ("$t", c.Tratamiento), ("$dur", c.DuracionDias),
        ("$med", c.Medicamentos), ("$mas", c.MascotaId),
        ("$cita", c.CitaId.HasValue ? c.CitaId.Value : DBNull.Value));


    public DataTable ObtenerFacturas() => Consultar(
        @"SELECT f.Id, f.Fecha, p.Nombre AS Cliente, f.Subtotal, f.Impuestos, f.Total, f.Estado
          FROM Facturas f INNER JOIN Personas p ON p.Id = f.ClienteId
          ORDER BY f.Fecha DESC;");

    public void CrearFactura(Factura f) => Ejecutar(
        @"INSERT INTO Facturas (Fecha, Subtotal, Impuestos, Total, Estado, ClienteId)
          VALUES ($f, $s, $i, $t, $e, $c);",
        ("$f", f.Fecha.ToString("O")), ("$s", f.Subtotal), ("$i", f.Impuestos),
        ("$t", f.Total), ("$e", f.Estado), ("$c", f.ClienteId));

    public void RegistrarPago(int facturaId, double monto, string metodo)
    {
        using var cn = Abrir();
        using var tx = cn.BeginTransaction();

        using (var cmd = cn.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = @"INSERT INTO Pagos (Monto, MetodoPago, Fecha, Estado, FacturaId)
                                VALUES ($m, $me, $f, 'Aprobado', $fac);";
            cmd.Parameters.AddWithValue("$m", monto);
            cmd.Parameters.AddWithValue("$me", metodo);
            cmd.Parameters.AddWithValue("$f", DateTime.Now.ToString("O"));
            cmd.Parameters.AddWithValue("$fac", facturaId);
            cmd.ExecuteNonQuery();
        }

        using (var cmd = cn.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = @"UPDATE Facturas SET Estado='Pagada'
                                WHERE Id=$id AND (SELECT SUM(Monto) FROM Pagos WHERE FacturaId=$id) >= Total;";
            cmd.Parameters.AddWithValue("$id", facturaId);
            cmd.ExecuteNonQuery();
        }
        tx.Commit();
    }
}