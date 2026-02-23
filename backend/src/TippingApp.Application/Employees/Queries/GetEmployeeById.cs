using MediatR;
using TippingApp.Application.Common;
using TippingApp.Application.DTOs;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Employees.Queries;

public record GetEmployeeByIdQuery(int Id) : IRequest<Result<EmployeeReadDto>>;

internal sealed class GetEmployeeByIdHandler(IEmployeeRepository employeeRepo)
    : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeReadDto>>
{
    public async Task<Result<EmployeeReadDto>> Handle(
        GetEmployeeByIdQuery request, CancellationToken ct)
    {
        var emp = await employeeRepo.GetByIdAsync(request.Id, ct);
        if (emp is null) return Result<EmployeeReadDto>.NotFound();
        return Result<EmployeeReadDto>.Success(new EmployeeReadDto(emp.Id, emp.Name, emp.Role));
    }
}
