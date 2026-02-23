using MediatR;
using TippingApp.Application.Common;
using TippingApp.Application.DTOs;
using TippingApp.Domain.Entities;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Tips.Commands;

public record CreateTipEntryCommand(
    DateOnly Date,
    decimal Amount,
    string Source) : IRequest<Result<TipEntryReadDto>>;

internal sealed class CreateTipEntryHandler(ITipEntryRepository tipRepo)
    : IRequestHandler<CreateTipEntryCommand, Result<TipEntryReadDto>>
{
    public async Task<Result<TipEntryReadDto>> Handle(
        CreateTipEntryCommand request, CancellationToken ct)
    {
        if (request.Amount <= 0)
            return Result<TipEntryReadDto>.Failure("Amount must be positive");

        var entry = new TipEntry
        {
            Date = request.Date,
            Amount = request.Amount,
            Source = request.Source
        };

        await tipRepo.AddAsync(entry, ct);
        await tipRepo.SaveChangesAsync(ct);

        return Result<TipEntryReadDto>.Success(
            new TipEntryReadDto(entry.Id, entry.Date, entry.Amount, entry.Source));
    }
}
