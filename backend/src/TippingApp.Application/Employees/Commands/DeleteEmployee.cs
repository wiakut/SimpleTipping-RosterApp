using MediatR;
using TippingApp.Application.Common;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Employees.Commands;

public record DeleteEmployeeCommand(int Id) : IRequest<Result>;

internal sealed class DeleteEmployeeHandler(IEmployeeRepository employeeRepo)
    : IRequestHandler<DeleteEmployeeCommand, Result>
{
    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        var employee = await employeeRepo.GetByIdAsync(request.Id, ct);
        if (employee is null) return Result.NotFound();

        await employeeRepo.DeleteAsync(employee, ct);
        await employeeRepo.SaveChangesAsync(ct);

        return Result.Success();
    }
}
