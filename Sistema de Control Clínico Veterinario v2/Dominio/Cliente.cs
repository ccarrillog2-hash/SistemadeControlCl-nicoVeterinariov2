namespace SistemaVeterinario.Dominio;

public sealed class Cliente : Persona
{
    public Cliente(string nombre, string dpi, string telefono, string email,
                   string direccion, string nit) : base(nombre, dpi, telefono, email, direccion)
    {
        Nit = nit?.Trim() ?? "";
    }

    public string Nit { get; }
    public override string ObtenerRol() => "Cliente";
}