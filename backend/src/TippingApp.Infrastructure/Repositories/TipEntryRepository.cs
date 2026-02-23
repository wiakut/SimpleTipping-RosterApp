using Microsoft.EntityFrameworkCore;
using TippingApp.Domain.Entities;
using TippingApp.Domain.Interfaces;
using TippingApp.Infrastructure.Data;

namespace TippingApp.Infrastructure.Repositories;

internal sealed class TipEntryRepository(AppDbContext db) : ITipEntryRepository
{
    public async Task<IEnumerable<TipEntry>> GetByWeekAsync(
        DateOnly start, DateOnly end, CancellationToken ct)
        => await db.TipEntries
            .Where(t => t.Date >= start && t.Date <= end)
            .ToListAsync(ct);

    public async Task<TipEntry?> GetByIdAsync(int id, CancellationToken ct)
        => await db.TipEntries.FindAsync([id], ct);

    public async Task AddAsync(TipEntry entry, CancellationToken ct)
        => await db.TipEntries.AddAsync(entry, ct);

    public Task DeleteAsync(TipEntry entry, CancellationToken ct)
    {
        db.TipEntries.Remove(entry);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
        => await db.SaveChangesAsync(ct);
}
