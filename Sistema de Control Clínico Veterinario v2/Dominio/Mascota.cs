namespace SistemaVeterinario.Dominio;

public sealed class Mascota
{
    public Mascota(string nombre, string especie, string raza, string sexo,
                   DateTime fechaNacimiento, double peso, int propietarioId)
    {
        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("El nombre es obligatorio.")
            : nombre.Trim();
        if (peso <= 0) throw new ArgumentException("El peso debe ser mayor a cero.");
        Especie = especie?.Trim() ?? "";
        Raza = raza?.Trim() ?? "";
        Sexo = sexo?.Trim() ?? "";
        FechaNacimiento = fechaNacimiento;
        Peso = peso;
        PropietarioId = propietarioId;
    }

    public int Id { get; set; }
    public string Nombre { get; }
    public string Especie { get; }
    public string Raza { get; }
    public string Sexo { get; }
    public DateTime FechaNacimiento { get; }
    public double Peso { get; }
    public int PropietarioId { get; }
}