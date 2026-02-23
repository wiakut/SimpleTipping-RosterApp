using MediatR;
using TippingApp.Application.DTOs;
using TippingApp.Application.Tips.Commands;
using TippingApp.Application.Tips.Queries;

namespace TippingApp.Api.Endpoints;

public static class TipEndpoints
{
    public static IEndpointRouteBuilder MapTipEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tips").WithTags("Tips");

        group.MapGet("/", async (DateOnly weekStart, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetTipsByWeekQuery(weekStart), ct))
            .Produces<IEnumerable<TipEntryReadDto>>();

        group.MapPost("/", async (TipEntryCreateDto dto, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new CreateTipEntryCommand(dto.Date, dto.Amount, dto.Source), ct);
            return result.IsSuccess
                ? Results.Created($"/api/tips/{result.Value!.Id}", result.Value)
                : Results.BadRequest(result.Error);
        })
        .Produces<TipEntryReadDto>(201)
        .ProducesProblem(400);

        group.MapPut("/{id:int}", async (int id, TipEntryUpdateDto dto, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new UpdateTipEntryCommand(id, dto.Date, dto.Amount, dto.Source), ct);
            if (result.IsNotFound) return Results.NotFound();
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .Produces<TipEntryReadDto>()
        .ProducesProblem(400)
        .ProducesProblem(404);

        group.MapDelete("/{id:int}", async (int id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteTipEntryCommand(id), ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound();
        })
        .Produces(204)
        .ProducesProblem(404);

        return app;
    }
}
