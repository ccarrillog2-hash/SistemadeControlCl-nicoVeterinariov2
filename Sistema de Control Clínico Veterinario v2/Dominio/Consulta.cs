namespace SistemaVeterinario.Dominio;

public sealed class Consulta
{
    public Consulta(string motivo, string diagnostico, string gravedad,
                    string tratamiento, int duracionDias, string medicamentos,
                    int mascotaId, int? citaId)
    {
        Motivo = motivo?.Trim() ?? "";
        Diagnostico = diagnostico?.Trim() ?? "";
        Gravedad = gravedad?.Trim() ?? "";
        Tratamiento = tratamiento?.Trim() ?? "";
        DuracionDias = duracionDias;
        Medicamentos = medicamentos?.Trim() ?? "";
        MascotaId = mascotaId;
        CitaId = citaId;
        Fecha = DateTime.Now;
    }

    public int Id { get; set; }
    public DateTime Fecha { get; }
    public string Motivo { get; }
    public string Diagnostico { get; }
    public string Gravedad { get; }
    public string Tratamiento { get; }
    public int DuracionDias { get; }
    public string Medicamentos { get; }
    public int MascotaId { get; }
    public int? CitaId { get; }
}