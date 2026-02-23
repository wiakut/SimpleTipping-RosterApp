using Microsoft.AspNetCore.Mvc;
using TippingApp.Api.DTOs;
using TippingApp.Api.Services;

namespace TippingApp.Api.Controllers;

[ApiController]
[Route("api/weekly-summary")]
public class WeeklySummaryController(ITipCalculationService tipService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<WeeklySummaryDto>> Get([FromQuery] DateOnly weekStart)
    {
        var summary = await tipService.GetWeeklySummaryAsync(weekStart);
        return Ok(summary);
    }
}
