using TippingApp.Application;
using TippingApp.Infrastructure;
using TippingApp.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.Services.MigrateAndSeed();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.MapEmployeeEndpoints();
app.MapShiftEndpoints();
app.MapTipEndpoints();
app.MapWeeklySummaryEndpoints();

app.Run();

public partial class Program { }
