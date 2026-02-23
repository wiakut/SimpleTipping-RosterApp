using TippingApp.Domain.Entities;

namespace TippingApp.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct);
    Task<Employee?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Employee employee, CancellationToken ct);
    Task DeleteAsync(Employee employee, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
