using Microsoft.EntityFrameworkCore;
using TippingApp.Domain.Entities;
using TippingApp.Domain.Interfaces;
using TippingApp.Infrastructure.Data;

namespace TippingApp.Infrastructure.Repositories;

internal sealed class ShiftRepository(AppDbContext db) : IShiftRepository
{
    public async Task<IEnumerable<Shift>> GetByWeekWithEmployeeAsync(
        DateOnly start, DateOnly end, CancellationToken ct)
        => await db.Shifts
            .Include(s => s.Employee)
            .Where(s => s.Date >= start && s.Date <= end)
            .ToListAsync(ct);

    public async Task<Shift?> GetByIdAsync(int id, CancellationToken ct)
        => await db.Shifts.FindAsync([id], ct);

    public async Task AddAsync(Shift shift, CancellationToken ct)
        => await db.Shifts.AddAsync(shift, ct);

    public Task DeleteAsync(Shift shift, CancellationToken ct)
    {
        db.Shifts.Remove(shift);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
        => await db.SaveChangesAsync(ct);
}
