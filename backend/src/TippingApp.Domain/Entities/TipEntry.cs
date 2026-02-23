namespace TippingApp.Domain.Entities;

public class TipEntry
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public required string Source { get; set; }
}
