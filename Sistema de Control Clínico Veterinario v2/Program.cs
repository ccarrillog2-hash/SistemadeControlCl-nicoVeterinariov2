using SistemaVeterinario.Aplicacion;
using SistemaVeterinario.Persistencia;

namespace SistemaVeterinario;

internal static class Program
{
    private static void Main()
    {
        try
        {
            var ini = new InicializadorBaseDatos();
            ini.Inicializar();
            var repo = new RepositorioVeterinario(ini.CadenaConexion);
            new MenuVeterinario(repo).Ejecutar();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"No fue posible iniciar el sistema: {ex.Message}");
            Console.ResetColor();
        }
    }
}