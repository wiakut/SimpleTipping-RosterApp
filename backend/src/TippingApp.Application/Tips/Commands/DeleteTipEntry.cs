using MediatR;
using TippingApp.Application.Common;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Tips.Commands;

public record DeleteTipEntryCommand(int Id) : IRequest<Result>;

internal sealed class DeleteTipEntryHandler(ITipEntryRepository tipRepo)
    : IRequestHandler<DeleteTipEntryCommand, Result>
{
    public async Task<Result> Handle(DeleteTipEntryCommand request, CancellationToken ct)
    {
        var entry = await tipRepo.GetByIdAsync(request.Id, ct);
        if (entry is null) return Result.NotFound();

        await tipRepo.DeleteAsync(entry, ct);
        await tipRepo.SaveChangesAsync(ct);

        return Result.Success();
    }
}
