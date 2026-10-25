using SistemaVeterinario.Dominio;
using SistemaVeterinario.Persistencia;

namespace SistemaVeterinario.Aplicacion;

internal sealed class MenuVeterinario
{
    private readonly RepositorioVeterinario _repo;
    private bool _continuar = true;

    public MenuVeterinario(RepositorioVeterinario repo) => _repo = repo;

    public void Ejecutar()
    {

        while (_continuar)
        {
            MostrarMenu();
            try
            {
                var op = Console.ReadLine()?.Trim();
                switch (op)
                {
                    case "1":  RegistrarCliente();     break;
                    case "2":  RegistrarMascota();     break;
                    case "3":  VerUsuarios();          break; // Ahora es la 3
                    case "4":  CrearCita();            break;
                    case "5":  ConsultarCitas();       break;
                    case "6":  CancelarCita();         break;
                    case "7":  AtenderCita();          break;
                    case "8":  ConsultarHistorial();   break;
                    case "9":  GenerarFactura();       break;
                    case "10": GenerarResumen();       break;
                    case "11": ListarFacturas();       break;
                    case "0":  _continuar = false;     break;
                    default:   Console.WriteLine("Opción inválida."); break;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Operación controlada: {ex.Message}");
                Console.ResetColor();
            }
            if (_continuar) Pausar();
        }
    }

    private void MostrarMenu()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("========================================================");
        Console.WriteLine("   SISTEMA DE CONTROL CLÍNICO VETERINARIO");
        Console.WriteLine("========================================================");
        Console.ResetColor();
        Console.WriteLine("  1. Registrar cliente");
        Console.WriteLine("  2. Registrar mascota");
        Console.WriteLine("  3. Ver usuarios");
        Console.WriteLine("  4. Crear cita");
        Console.WriteLine("  5. Ver citas");
        Console.WriteLine("  6. Cancelar cita");
        Console.WriteLine("  7. Atender cita (consulta médica)");
        Console.WriteLine("  8. Consultar historial clínico");
        Console.WriteLine("  9. Facturación y pago");
        Console.WriteLine(" 10. Generar resumen de control");
        Console.WriteLine(" 11. Listar facturas");
        Console.WriteLine("  0. Salir");
        Console.Write("Opción: ");
    }

    private void VerUsuarios()
    {
        MostrarTabla(_repo.ObtenerVeterinarios(), "VETERINARIOS");
        MostrarTabla(_repo.ObtenerClientes(), "CLIENTES");
        MostrarTabla(_repo.ObtenerMascotas(), "MASCOTAS");
    }

    private void RegistrarCliente()
    {
        Console.WriteLine("\n=== REGISTRAR CLIENTE ===");
        var c = new Cliente(
            LeerTexto("Nombre"), LeerTexto("DPI"), LeerTexto("Teléfono"),
            LeerTexto("Email"), LeerTexto("Dirección"), LeerTexto("NIT"));
        _repo.CrearCliente(c);
        Console.WriteLine($"✔ Cliente '{c.Nombre}' registrado.");
    }

    private void RegistrarMascota()
    {
        MostrarTabla(_repo.ObtenerClientes(), "CLIENTES");
        var propId = LeerEntero("Id del propietario");
        var m = new Mascota(
            LeerTexto("Nombre mascota"), LeerTexto("Especie"), LeerTexto("Raza"),
            LeerTexto("Sexo"), LeerFecha("Fecha nacimiento (dd/MM/yyyy)"),
            LeerDouble("Peso (kg)"), propId);
        _repo.CrearMascota(m);
        Console.WriteLine($"✔ Mascota '{m.Nombre}' registrada.");
    }

      private void CrearCita()
    {
        MostrarTabla(_repo.ObtenerClientes(), "CLIENTES");
        var cliId = LeerEntero("Id del cliente");
        
        MostrarTabla(_repo.ObtenerMascotasPorCliente(cliId), "MASCOTAS DEL CLIENTE");
        var masId = LeerEntero("Id de la mascota");


        int vetId = 1; 
        Console.WriteLine("\n>> Cita asignada automáticamente al Veterinario General.");

        var cita = new Cita(
            LeerFecha("Fecha (dd/MM/yyyy)"),
            LeerTexto("Hora (HH:mm)"),
            LeerTexto("Motivo"), cliId, masId, vetId);

        _repo.CrearCita(cita);
        Console.WriteLine("✔ Cita creada.");
    }

    private void ConsultarCitas() => MostrarTabla(_repo.ObtenerCitas(), "CITAS");

    private void CancelarCita()
    {
        MostrarTabla(_repo.ObtenerCitas(), "CITAS");
        _repo.CancelarCita(LeerEntero("Id de la cita a cancelar"));
        Console.WriteLine("✔ Cita cancelada.");
    }

    private void AtenderCita()
    {
        MostrarTabla(_repo.ObtenerCitas(), "CITAS");
        var id = LeerEntero("Id de la cita a atender");

        var datosCita = _repo.ObtenerCitaPorId(id);
        if (datosCita is null) throw new InvalidOperationException("La cita no existe.");
        int mascotaId = Convert.ToInt32(datosCita["MascotaId"]);

        var consulta = new Consulta(
            LeerTexto("Motivo consulta"), LeerTexto("Diagnóstico"),
            LeerTexto("Gravedad (Leve/Moderado/Grave)"),
            LeerTexto("Tratamiento"), LeerEntero("Duración (días)"),
            LeerTexto("Medicamentos"), mascotaId, id);

        _repo.CrearConsulta(consulta);
        _repo.CompletarCita(id);
        Console.WriteLine("✔ Consulta guardada y cita completada.");
    }

    private void ConsultarHistorial()
    {
        MostrarTabla(_repo.ObtenerMascotas(), "MASCOTAS");
        var id = LeerEntero("Id de la mascota");
        MostrarTabla(_repo.ObtenerHistorial(id), "HISTORIAL CLÍNICO");
    }

    private void GenerarFactura()
    {
        MostrarTabla(_repo.ObtenerClientes(), "CLIENTES");
        var cliId = LeerEntero("Id del cliente");
        var sub = LeerDouble("Subtotal (Q)");
        Console.Write("¿Aplica descuento? (s/n): ");
        double desc = 0;
        if ((Console.ReadLine() ?? "").ToLower() == "s")
            desc = LeerDouble("Porcentaje (%)") * sub / 100;

        var f = new Factura(cliId, sub, desc);
        Console.WriteLine($"Total a pagar: Q {f.Total:F2}");

        var metodo = LeerTexto("Método de pago (Efectivo/Tarjeta/Transferencia)");
        Console.Write("¿Pago aprobado? (s/n): ");
        if ((Console.ReadLine() ?? "").ToLower() != "s")
        {
            Console.WriteLine("Pago cancelado.");
            return;
        }
        _repo.CrearFactura(f);
        _repo.RegistrarPago(f.Id, f.Total, metodo);
        Console.WriteLine("✔ Factura emitida y pago registrado.");
    }

    private void ListarFacturas() => MostrarTabla(_repo.ObtenerFacturas(), "FACTURAS");

    private void GenerarResumen()
    {
        Console.WriteLine("\n=== RESUMEN ===");
        MostrarTabla(_repo.ObtenerClientes(), "Clientes");
        MostrarTabla(_repo.ObtenerMascotas(), "Mascotas");
        MostrarTabla(_repo.ObtenerCitas(), "Citas");
        MostrarTabla(_repo.ObtenerFacturas(), "Facturas");
    }

    private void VerPacientes()
    {
        MostrarTabla(_repo.ObtenerClientes(), "CLIENTES");
        MostrarTabla(_repo.ObtenerMascotas(), "MASCOTAS");
    }

    private static void MostrarTabla(System.Data.DataTable t, string titulo)
    {
        Console.WriteLine($"\n=== {titulo} ===");
        if (t.Rows.Count == 0) { Console.WriteLine("Sin registros."); return; }
        foreach (System.Data.DataColumn col in t.Columns)
            Console.Write($"{col.ColumnName,-20}");
        Console.WriteLine();
        foreach (System.Data.DataRow fila in t.Rows)
        {
            foreach (var v in fila.ItemArray)
                Console.Write($"{v,-20}");
            Console.WriteLine();
        }
    }

    private static string LeerTexto(string msg)
    {
        Console.Write($"{msg}: ");
        var v = Console.ReadLine();
        return string.IsNullOrWhiteSpace(v)
            ? throw new ArgumentException($"'{msg}' obligatorio.")
            : v.Trim();
    }

    private static int LeerEntero(string msg)
    {
        Console.Write($"{msg}: ");
        return int.TryParse(Console.ReadLine(), out var v)
            ? v : throw new ArgumentException("Número inválido.");
    }

    private static double LeerDouble(string msg)
    {
        Console.Write($"{msg}: ");
        return double.TryParse(Console.ReadLine(), out var v)
            ? v : throw new ArgumentException("Número inválido.");
    }

    private static DateTime LeerFecha(string msg)
    {
        Console.Write($"{msg}: ");
        return DateTime.TryParse(Console.ReadLine(), out var v)
            ? v : throw new ArgumentException("Fecha inválida.");
    }

    private static void Pausar()
    {
        Console.WriteLine("\nPresione ENTER para continuar...");
        Console.ReadLine();
    }
}