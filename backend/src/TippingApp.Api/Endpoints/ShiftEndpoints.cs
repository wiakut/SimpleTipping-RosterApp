using MediatR;
using TippingApp.Application.DTOs;
using TippingApp.Application.Shifts.Commands;
using TippingApp.Application.Shifts.Queries;

namespace TippingApp.Api.Endpoints;

public static class ShiftEndpoints
{
    public static IEndpointRouteBuilder MapShiftEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/shifts").WithTags("Shifts");

        group.MapGet("/", async (DateOnly weekStart, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetShiftsByWeekQuery(weekStart), ct))
            .Produces<IEnumerable<ShiftReadDto>>();

        group.MapPost("/", async (ShiftCreateDto dto, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new CreateShiftCommand(dto.EmployeeId, dto.Date, dto.StartTime, dto.EndTime), ct);
            return result.IsSuccess
                ? Results.Created($"/api/shifts/{result.Value!.Id}", result.Value)
                : Results.BadRequest(result.Error);
        })
        .Produces<ShiftReadDto>(201)
        .ProducesProblem(400);

        group.MapPut("/{id:int}", async (int id, ShiftUpdateDto dto, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new UpdateShiftCommand(id, dto.EmployeeId, dto.Date, dto.StartTime, dto.EndTime), ct);
            if (result.IsNotFound) return Results.NotFound();
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .Produces<ShiftReadDto>()
        .ProducesProblem(400)
        .ProducesProblem(404);

        group.MapDelete("/{id:int}", async (int id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteShiftCommand(id), ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound();
        })
        .Produces(204)
        .ProducesProblem(404);

        return app;
    }
}
