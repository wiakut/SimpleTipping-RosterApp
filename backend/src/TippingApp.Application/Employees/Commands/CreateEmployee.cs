using MediatR;
using TippingApp.Application.Common;
using TippingApp.Application.DTOs;
using TippingApp.Domain.Entities;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Employees.Commands;

public record CreateEmployeeCommand(string Name, string Role) : IRequest<Result<EmployeeReadDto>>;

internal sealed class CreateEmployeeHandler(IEmployeeRepository employeeRepo)
    : IRequestHandler<CreateEmployeeCommand, Result<EmployeeReadDto>>
{
    public async Task<Result<EmployeeReadDto>> Handle(
        CreateEmployeeCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<EmployeeReadDto>.Failure("Name is required");

        if (string.IsNullOrWhiteSpace(request.Role))
            return Result<EmployeeReadDto>.Failure("Role is required");

        var employee = new Employee
        {
            Name = request.Name.Trim(),
            Role = request.Role.Trim()
        };

        await employeeRepo.AddAsync(employee, ct);
        await employeeRepo.SaveChangesAsync(ct);

        return Result<EmployeeReadDto>.Success(
            new EmployeeReadDto(employee.Id, employee.Name, employee.Role));
    }
}
