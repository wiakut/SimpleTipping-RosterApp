using TippingApp.Domain.Entities;

namespace TippingApp.Domain.Interfaces;

public interface ITipEntryRepository
{
    Task<IEnumerable<TipEntry>> GetByWeekAsync(DateOnly start, DateOnly end, CancellationToken ct);
    Task<TipEntry?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(TipEntry entry, CancellationToken ct);
    Task DeleteAsync(TipEntry entry, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
