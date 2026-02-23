using MediatR;
using TippingApp.Application.DTOs;
using TippingApp.Application.WeeklySummary.Queries;

namespace TippingApp.Api.Endpoints;

public static class WeeklySummaryEndpoints
{
    public static IEndpointRouteBuilder MapWeeklySummaryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/weekly-summary").WithTags("WeeklySummary");

        group.MapGet("/", async (DateOnly weekStart, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetWeeklySummaryQuery(weekStart), ct))
            .Produces<WeeklySummaryDto>();

        return app;
    }
}
