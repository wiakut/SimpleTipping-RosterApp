using Microsoft.EntityFrameworkCore;
using TippingApp.Api.Data;
using TippingApp.Api.DTOs;
using TippingApp.Api.Models;

namespace TippingApp.Api.Services;

public class TipCalculationService(AppDbContext db) : ITipCalculationService
{
    public async Task<WeeklySummaryDto> GetWeeklySummaryAsync(DateOnly weekStart)
    {
        var weekEnd = weekStart.AddDays(6);

        var shifts = await db.Shifts
            .Include(s => s.Employee)
            .Where(s => s.Date >= weekStart && s.Date <= weekEnd)
            .ToListAsync();

        var tips = await db.TipEntries
            .Where(t => t.Date >= weekStart && t.Date <= weekEnd)
            .ToListAsync();

        var totalTips = tips.Sum(t => t.Amount);

        var employees = await db.Employees.OrderBy(e => e.Name).ToListAsync();

        var employeeHours = employees
            .Select(e =>
            {
                var empShifts = shifts.Where(s => s.EmployeeId == e.Id);
                var hours = CalculateHoursForShifts(empShifts);
                return (e.Id, e.Name, e.Role, hours);
            })
            .ToList()
            .AsReadOnly();

        var splitResult = CalculateTipSplit(totalTips, employeeHours);

        var totalHours = employeeHours.Sum(e => e.hours);

        return new WeeklySummaryDto(weekStart, weekEnd, totalTips, totalHours, splitResult);
    }

    public decimal CalculateHoursForShifts(IEnumerable<Shift> shifts)
    {
        return shifts.Sum(s =>
        {
            var duration = s.EndTime.ToTimeSpan() - s.StartTime.ToTimeSpan();
            return (decimal)duration.TotalHours;
        });
    }

    public IReadOnlyList<EmployeeSummaryDto> CalculateTipSplit(
        decimal totalTips,
        IReadOnlyList<(int EmployeeId, string Name, string Role, decimal Hours)> employeeHours)
    {
        var totalHours = employeeHours.Sum(e => e.Hours);

        if (totalHours == 0)
        {
            return employeeHours
                .Select(e => new EmployeeSummaryDto(e.EmployeeId, e.Name, e.Role, 0, 0, 0))
                .ToList()
                .AsReadOnly();
        }

        var results = employeeHours
            .Select(e =>
            {
                var percentage = totalHours > 0
                    ? Math.Round(e.Hours / totalHours * 100, 1)
                    : 0;
                var tipShare = Math.Round(e.Hours / totalHours * totalTips, 2);
                return new EmployeeSummaryDto(
                    e.EmployeeId, e.Name, e.Role, Math.Round(e.Hours, 2), tipShare, percentage);
            })
            .ToList()
            .AsReadOnly();

        return results;
    }
}
