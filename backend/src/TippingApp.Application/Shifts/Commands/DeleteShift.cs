using MediatR;
using TippingApp.Application.Common;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Shifts.Commands;

public record DeleteShiftCommand(int Id) : IRequest<Result>;

internal sealed class DeleteShiftHandler(IShiftRepository shiftRepo)
    : IRequestHandler<DeleteShiftCommand, Result>
{
    public async Task<Result> Handle(DeleteShiftCommand request, CancellationToken ct)
    {
        var shift = await shiftRepo.GetByIdAsync(request.Id, ct);
        if (shift is null) return Result.NotFound();

        await shiftRepo.DeleteAsync(shift, ct);
        await shiftRepo.SaveChangesAsync(ct);

        return Result.Success();
    }
}
