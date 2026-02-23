namespace TippingApp.Api.Models;

public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Role { get; set; }

    public ICollection<Shift> Shifts { get; set; } = [];
}
