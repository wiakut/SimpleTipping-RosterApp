using MediatR;
using TippingApp.Application.DTOs;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Employees.Queries;

public record GetAllEmployeesQuery : IRequest<IEnumerable<EmployeeReadDto>>;

internal sealed class GetAllEmployeesHandler(IEmployeeRepository employeeRepo)
    : IRequestHandler<GetAllEmployeesQuery, IEnumerable<EmployeeReadDto>>
{
    public async Task<IEnumerable<EmployeeReadDto>> Handle(
        GetAllEmployeesQuery request, CancellationToken ct)
    {
        var employees = await employeeRepo.GetAllAsync(ct);
        return employees.Select(e => new EmployeeReadDto(e.Id, e.Name, e.Role));
    }
}
