using MediatR;
using TippingApp.Application.Common;
using TippingApp.Application.DTOs;
using TippingApp.Domain.Entities;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Shifts.Commands;

public record CreateShiftCommand(
    int EmployeeId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime) : IRequest<Result<ShiftReadDto>>;

internal sealed class CreateShiftHandler(IShiftRepository shiftRepo, IEmployeeRepository employeeRepo)
    : IRequestHandler<CreateShiftCommand, Result<ShiftReadDto>>
{
    public async Task<Result<ShiftReadDto>> Handle(
        CreateShiftCommand request, CancellationToken ct)
    {
        var employee = await employeeRepo.GetByIdAsync(request.EmployeeId, ct);
        if (employee is null)
            return Result<ShiftReadDto>.Failure("Employee not found");

        if (request.EndTime <= request.StartTime)
            return Result<ShiftReadDto>.Failure("End time must be after start time");

        var shift = new Shift
        {
            EmployeeId = request.EmployeeId,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        await shiftRepo.AddAsync(shift, ct);
        await shiftRepo.SaveChangesAsync(ct);

        return Result<ShiftReadDto>.Success(new ShiftReadDto(
            shift.Id, shift.EmployeeId, employee.Name,
            shift.Date, shift.StartTime, shift.EndTime));
    }
}
