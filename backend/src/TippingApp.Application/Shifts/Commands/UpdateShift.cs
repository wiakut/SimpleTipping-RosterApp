using MediatR;
using TippingApp.Application.Common;
using TippingApp.Application.DTOs;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Shifts.Commands;

public record UpdateShiftCommand(
    int Id,
    int EmployeeId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime) : IRequest<Result<ShiftReadDto>>;

internal sealed class UpdateShiftHandler(IShiftRepository shiftRepo, IEmployeeRepository employeeRepo)
    : IRequestHandler<UpdateShiftCommand, Result<ShiftReadDto>>
{
    public async Task<Result<ShiftReadDto>> Handle(
        UpdateShiftCommand request, CancellationToken ct)
    {
        var shift = await shiftRepo.GetByIdAsync(request.Id, ct);
        if (shift is null) return Result<ShiftReadDto>.NotFound();

        if (request.EndTime <= request.StartTime)
            return Result<ShiftReadDto>.Failure("End time must be after start time");

        var employee = await employeeRepo.GetByIdAsync(request.EmployeeId, ct);
        if (employee is null)
            return Result<ShiftReadDto>.Failure("Employee not found");

        shift.EmployeeId = request.EmployeeId;
        shift.Date = request.Date;
        shift.StartTime = request.StartTime;
        shift.EndTime = request.EndTime;

        await shiftRepo.SaveChangesAsync(ct);

        return Result<ShiftReadDto>.Success(new ShiftReadDto(
            shift.Id, shift.EmployeeId, employee.Name,
            shift.Date, shift.StartTime, shift.EndTime));
    }
}
