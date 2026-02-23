namespace TippingApp.Domain.Entities;

public class Shift
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public Employee Employee { get; set; } = null!;
}
