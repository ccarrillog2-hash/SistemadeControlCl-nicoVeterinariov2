using System.Reflection;
using Microsoft.Data.Sqlite;

namespace SistemaVeterinario.Persistencia;

internal sealed class InicializadorBaseDatos
{
    public string RutaBaseDatos { get; } =
        Environment.GetEnvironmentVariable("VETERINARIA_DB")
        ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "UMG", "Veterinaria", "veterinaria.db");

    public string CadenaConexion => new SqliteConnectionStringBuilder
    {
        DataSource = RutaBaseDatos,
        ForeignKeys = true
    }.ToString();

    public void Inicializar()
    {
        var dir = Path.GetDirectoryName(RutaBaseDatos);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

        using var conexion = new SqliteConnection(CadenaConexion);
        conexion.Open();
        using var cmd = conexion.CreateCommand();
        cmd.CommandText = LeerScript();
        cmd.ExecuteNonQuery();
    }

    private static string LeerScript()
    {
        var asm = Assembly.GetExecutingAssembly();
        var recurso = asm.GetManifestResourceNames()
            .Single(n => n.EndsWith("crear_tablas.sql", StringComparison.OrdinalIgnoreCase));
        using var stream = asm.GetManifestResourceStream(recurso)
            ?? throw new InvalidOperationException("No se encontró crear_tablas.sql.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}