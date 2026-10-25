namespace SistemaVeterinario.Dominio;

public sealed class Cita
{
    public Cita(DateTime fecha, string hora, string motivo,
                int clienteId, int mascotaId, int veterinarioId)
    {
        if (clienteId <= 0 || mascotaId <= 0 || veterinarioId <= 0)
            throw new ArgumentException("Debe seleccionar cliente, mascota y veterinario.");
        Fecha = fecha;
        Hora = string.IsNullOrWhiteSpace(hora)
            ? throw new ArgumentException("La hora es obligatoria.") : hora.Trim();
        Motivo = motivo?.Trim() ?? "";
        ClienteId = clienteId;
        MascotaId = mascotaId;
        VeterinarioId = veterinarioId;
        Estado = "Programada";
    }

    public int Id { get; set; }
    public DateTime Fecha { get; }
    public string Hora { get; }
    public string Motivo { get; }
    public string Estado { get; private set; }
    public int ClienteId { get; }
    public int MascotaId { get; }
    public int VeterinarioId { get; }

    public void Confirmar() => Estado = "Confirmada";
    public void Cancelar() => Estado = "Cancelada";
    public void Completar() => Estado = "Completada";
    public void EnAtencion() => Estado = "En atención";
}