namespace SistemaVeterinario.Dominio;

public sealed class Factura
{
    public Factura(int clienteId, double subtotal, double descuento = 0)
    {
        if (clienteId <= 0) throw new ArgumentException("Cliente inválido.");
        if (subtotal < 0) throw new ArgumentException("Subtotal inválido.");
        ClienteId = clienteId;
        Subtotal = subtotal;
        Descuento = descuento;
        Impuestos = Math.Round((subtotal - descuento) * 0.16, 2);
        Total = Math.Round(subtotal - descuento + Impuestos, 2);
        Fecha = DateTime.Now;
        Estado = "Pendiente";
    }

    public int Id { get; set; }
    public DateTime Fecha { get; }
    public double Subtotal { get; }
    public double Descuento { get; }
    public double Impuestos { get; }
    public double Total { get; }
    public string Estado { get; private set; }
    public int ClienteId { get; }

    public void MarcarPagada() => Estado = "Pagada";
}