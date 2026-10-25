namespace SistemaVeterinario.Dominio;

public sealed class Veterinario : Persona
{
    public Veterinario(string nombre, string dpi, string telefono, string email,
                       string direccion, string usuario, string contrasena,
                       string codigoVet, string especialidad, string horario)
        : base(nombre, dpi, telefono, email, direccion)
    {
        Usuario = Validar(usuario, "El usuario es obligatorio.");
        Contrasena = Validar(contrasena, "La contraseña es obligatoria.");
        CodigoVet = codigoVet?.Trim() ?? "";
        Especialidad = especialidad?.Trim() ?? "";
        Horario = horario?.Trim() ?? "";
    }

    public string Usuario { get; }
    public string Contrasena { get; }
    public string CodigoVet { get; }
    public string Especialidad { get; }
    public string Horario { get; }

    public override string ObtenerRol() => "Veterinario";
}