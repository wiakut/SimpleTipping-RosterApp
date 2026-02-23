using TippingApp.Domain.Entities;

namespace TippingApp.Domain.Interfaces;

public interface IShiftRepository
{
    Task<IEnumerable<Shift>> GetByWeekWithEmployeeAsync(DateOnly start, DateOnly end, CancellationToken ct);
    Task<Shift?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Shift shift, CancellationToken ct);
    Task DeleteAsync(Shift shift, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
