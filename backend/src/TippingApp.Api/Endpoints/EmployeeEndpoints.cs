using MediatR;
using TippingApp.Application.DTOs;
using TippingApp.Application.Employees.Commands;
using TippingApp.Application.Employees.Queries;

namespace TippingApp.Api.Endpoints;

public static class EmployeeEndpoints
{
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees").WithTags("Employees");

        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetAllEmployeesQuery(), ct))
            .Produces<IEnumerable<EmployeeReadDto>>();

        group.MapGet("/{id:int}", async (int id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetEmployeeByIdQuery(id), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound();
        })
        .Produces<EmployeeReadDto>()
        .ProducesProblem(404);

        group.MapPost("/", async (EmployeeCreateDto dto, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new CreateEmployeeCommand(dto.Name, dto.Role), ct);
            return result.IsSuccess
                ? Results.Created($"/api/employees/{result.Value!.Id}", result.Value)
                : Results.BadRequest(result.Error);
        })
        .Produces<EmployeeReadDto>(201)
        .ProducesProblem(400);

        group.MapPut("/{id:int}", async (int id, EmployeeUpdateDto dto, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new UpdateEmployeeCommand(id, dto.Name, dto.Role), ct);
            if (result.IsNotFound) return Results.NotFound();
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .Produces<EmployeeReadDto>()
        .ProducesProblem(400)
        .ProducesProblem(404);

        group.MapDelete("/{id:int}", async (int id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteEmployeeCommand(id), ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound();
        })
        .Produces(204)
        .ProducesProblem(404);

        return app;
    }
}
