namespace SistemaVeterinario.Dominio;

public abstract class Persona
{
    protected Persona(string nombre, string dpi, string telefono, string email, string direccion)
    {
        Nombre = Validar(nombre, "El nombre es obligatorio.");
        Dpi = Validar(dpi, "El DPI es obligatorio.");
        Telefono = telefono?.Trim() ?? "";
        Email = email?.Trim() ?? "";
        Direccion = direccion?.Trim() ?? "";
    }

    public int Id { get; set; }
    public string Nombre { get; }
    public string Dpi { get; }
    public string Telefono { get; }
    public string Email { get; }
    public string Direccion { get; }

    public abstract string ObtenerRol();
    public virtual string ObtenerInformacion() =>
        $"{ObtenerRol()}: {Nombre} | DPI: {Dpi} | Tel: {Telefono}";

    protected static string Validar(string valor, string msg) =>
        string.IsNullOrWhiteSpace(valor) ? throw new ArgumentException(msg) : valor.Trim();
}