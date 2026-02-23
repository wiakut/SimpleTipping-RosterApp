using MediatR;
using TippingApp.Application.Common;
using TippingApp.Application.DTOs;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Employees.Commands;

public record UpdateEmployeeCommand(int Id, string Name, string Role) : IRequest<Result<EmployeeReadDto>>;

internal sealed class UpdateEmployeeHandler(IEmployeeRepository employeeRepo)
    : IRequestHandler<UpdateEmployeeCommand, Result<EmployeeReadDto>>
{
    public async Task<Result<EmployeeReadDto>> Handle(
        UpdateEmployeeCommand request, CancellationToken ct)
    {
        var employee = await employeeRepo.GetByIdAsync(request.Id, ct);
        if (employee is null) return Result<EmployeeReadDto>.NotFound();

        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<EmployeeReadDto>.Failure("Name is required");

        if (string.IsNullOrWhiteSpace(request.Role))
            return Result<EmployeeReadDto>.Failure("Role is required");

        employee.Name = request.Name.Trim();
        employee.Role = request.Role.Trim();

        await employeeRepo.SaveChangesAsync(ct);

        return Result<EmployeeReadDto>.Success(
            new EmployeeReadDto(employee.Id, employee.Name, employee.Role));
    }
}
