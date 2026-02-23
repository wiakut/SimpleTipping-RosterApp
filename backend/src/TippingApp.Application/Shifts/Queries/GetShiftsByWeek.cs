using MediatR;
using TippingApp.Application.DTOs;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Shifts.Queries;

public record GetShiftsByWeekQuery(DateOnly WeekStart) : IRequest<IEnumerable<ShiftReadDto>>;

internal sealed class GetShiftsByWeekHandler(IShiftRepository shiftRepo)
    : IRequestHandler<GetShiftsByWeekQuery, IEnumerable<ShiftReadDto>>
{
    public async Task<IEnumerable<ShiftReadDto>> Handle(
        GetShiftsByWeekQuery request, CancellationToken ct)
    {
        var weekEnd = request.WeekStart.AddDays(6);
        var shifts = await shiftRepo.GetByWeekWithEmployeeAsync(request.WeekStart, weekEnd, ct);

        return shifts
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .Select(s => new ShiftReadDto(
                s.Id, s.EmployeeId, s.Employee.Name,
                s.Date, s.StartTime, s.EndTime));
    }
}
