using MediatR;
using TippingApp.Application.Common;
using TippingApp.Application.DTOs;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.WeeklySummary.Queries;

public record GetWeeklySummaryQuery(DateOnly WeekStart) : IRequest<WeeklySummaryDto>;

internal sealed class GetWeeklySummaryHandler(
    IShiftRepository shiftRepo,
    ITipEntryRepository tipRepo,
    IEmployeeRepository employeeRepo)
    : IRequestHandler<GetWeeklySummaryQuery, WeeklySummaryDto>
{
    public async Task<WeeklySummaryDto> Handle(
        GetWeeklySummaryQuery request, CancellationToken ct)
    {
        var weekEnd = request.WeekStart.AddDays(6);

        var shifts = await shiftRepo.GetByWeekWithEmployeeAsync(request.WeekStart, weekEnd, ct);
        var tips = await tipRepo.GetByWeekAsync(request.WeekStart, weekEnd, ct);
        var employees = await employeeRepo.GetAllAsync(ct);

        var totalTips = tips.Sum(t => t.Amount);
        var shiftsList = shifts.ToList();

        var employeeHours = employees
            .OrderBy(e => e.Name)
            .Select(e =>
            {
                var empShifts = shiftsList.Where(s => s.EmployeeId == e.Id);
                var hours = TipCalculator.CalculateHoursForShifts(empShifts);
                return (e.Id, e.Name, e.Role, hours);
            })
            .ToList()
            .AsReadOnly();

        var splitResult = TipCalculator.CalculateTipSplit(totalTips, employeeHours);
        var totalHours = employeeHours.Sum(e => e.hours);

        return new WeeklySummaryDto(request.WeekStart, weekEnd, totalTips, totalHours, splitResult);
    }
}
