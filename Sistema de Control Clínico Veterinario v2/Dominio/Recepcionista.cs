namespace SistemaVeterinario.Dominio;

public sealed class Recepcionista : Persona
{
    public Recepcionista(string nombre, string dpi, string telefono, string email,
                         string direccion, string usuario, string contrasena,
                         string codigoEmpleado, string turno)
        : base(nombre, dpi, telefono, email, direccion)
    {
        Usuario = Validar(usuario, "El usuario es obligatorio.");
        Contrasena = Validar(contrasena, "La contraseña es obligatoria.");
        CodigoEmpleado = codigoEmpleado?.Trim() ?? "";
        Turno = turno?.Trim() ?? "";
    }

    public string Usuario { get; }
    public string Contrasena { get; }
    public string CodigoEmpleado { get; }
    public string Turno { get; }

    public override string ObtenerRol() => "Recepcionista";
}