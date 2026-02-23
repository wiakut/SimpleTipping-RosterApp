using TippingApp.Application.DTOs;
using TippingApp.Domain.Entities;

namespace TippingApp.Application.Common;

public static class TipCalculator
{
    public static decimal CalculateHoursForShifts(IEnumerable<Shift> shifts)
    {
        return shifts.Sum(s =>
        {
            var duration = s.EndTime.ToTimeSpan() - s.StartTime.ToTimeSpan();
            return (decimal)duration.TotalHours;
        });
    }

    public static IReadOnlyList<EmployeeSummaryDto> CalculateTipSplit(
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

        return employeeHours
            .Select(e =>
            {
                var percentage = Math.Round(e.Hours / totalHours * 100, 1);
                var tipShare = Math.Round(e.Hours / totalHours * totalTips, 2);
                return new EmployeeSummaryDto(
                    e.EmployeeId, e.Name, e.Role, Math.Round(e.Hours, 2), tipShare, percentage);
            })
            .ToList()
            .AsReadOnly();
    }
}
