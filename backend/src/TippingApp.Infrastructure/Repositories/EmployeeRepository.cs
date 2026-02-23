using Microsoft.EntityFrameworkCore;
using TippingApp.Domain.Entities;
using TippingApp.Domain.Interfaces;
using TippingApp.Infrastructure.Data;

namespace TippingApp.Infrastructure.Repositories;

internal sealed class EmployeeRepository(AppDbContext db) : IEmployeeRepository
{
    public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct)
        => await db.Employees.OrderBy(e => e.Name).ToListAsync(ct);

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken ct)
        => await db.Employees.FindAsync([id], ct);

    public async Task AddAsync(Employee employee, CancellationToken ct)
        => await db.Employees.AddAsync(employee, ct);

    public Task DeleteAsync(Employee employee, CancellationToken ct)
    {
        db.Employees.Remove(employee);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
        => await db.SaveChangesAsync(ct);
}
