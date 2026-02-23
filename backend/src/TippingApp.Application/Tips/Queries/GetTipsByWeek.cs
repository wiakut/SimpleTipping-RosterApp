using MediatR;
using TippingApp.Application.DTOs;
using TippingApp.Domain.Interfaces;

namespace TippingApp.Application.Tips.Queries;

public record GetTipsByWeekQuery(DateOnly WeekStart) : IRequest<IEnumerable<TipEntryReadDto>>;

internal sealed class GetTipsByWeekHandler(ITipEntryRepository tipRepo)
    : IRequestHandler<GetTipsByWeekQuery, IEnumerable<TipEntryReadDto>>
{
    public async Task<IEnumerable<TipEntryReadDto>> Handle(
        GetTipsByWeekQuery request, CancellationToken ct)
    {
        var weekEnd = request.WeekStart.AddDays(6);
        var tips = await tipRepo.GetByWeekAsync(request.WeekStart, weekEnd, ct);

        return tips
            .OrderBy(t => t.Date)
            .ThenBy(t => t.Source)
            .Select(t => new TipEntryReadDto(t.Id, t.Date, t.Amount, t.Source));
    }
}
