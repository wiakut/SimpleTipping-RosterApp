using MediatR;
using TippingApp.Application.Common;
using TippingApp.Application.DTOs;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Tips.Commands;

public record UpdateTipEntryCommand(
    int Id,
    DateOnly Date,
    decimal Amount,
    string Source) : IRequest<Result<TipEntryReadDto>>;

internal sealed class UpdateTipEntryHandler(ITipEntryRepository tipRepo)
    : IRequestHandler<UpdateTipEntryCommand, Result<TipEntryReadDto>>
{
    public async Task<Result<TipEntryReadDto>> Handle(
        UpdateTipEntryCommand request, CancellationToken ct)
    {
        var entry = await tipRepo.GetByIdAsync(request.Id, ct);
        if (entry is null) return Result<TipEntryReadDto>.NotFound();

        if (request.Amount <= 0)
            return Result<TipEntryReadDto>.Failure("Amount must be positive");

        entry.Date = request.Date;
        entry.Amount = request.Amount;
        entry.Source = request.Source;

        await tipRepo.SaveChangesAsync(ct);

        return Result<TipEntryReadDto>.Success(
            new TipEntryReadDto(entry.Id, entry.Date, entry.Amount, entry.Source));
    }
}
