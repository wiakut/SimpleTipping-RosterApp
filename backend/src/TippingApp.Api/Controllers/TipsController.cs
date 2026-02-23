using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippingApp.Api.Data;
using TippingApp.Api.DTOs;
using TippingApp.Api.Models;

namespace TippingApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipEntryReadDto>>> GetByWeek([FromQuery] DateOnly weekStart)
    {
        var weekEnd = weekStart.AddDays(6);

        var tips = await db.TipEntries
            .Where(t => t.Date >= weekStart && t.Date <= weekEnd)
            .OrderBy(t => t.Date)
            .ThenBy(t => t.Source)
            .Select(t => new TipEntryReadDto(t.Id, t.Date, t.Amount, t.Source))
            .ToListAsync();

        return Ok(tips);
    }

    [HttpPost]
    public async Task<ActionResult<TipEntryReadDto>> Create([FromBody] TipEntryCreateDto dto)
    {
        if (dto.Amount <= 0)
            return BadRequest("Amount must be positive");

        var entry = new TipEntry
        {
            Date = dto.Date,
            Amount = dto.Amount,
            Source = dto.Source
        };

        db.TipEntries.Add(entry);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByWeek), null,
            new TipEntryReadDto(entry.Id, entry.Date, entry.Amount, entry.Source));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TipEntryReadDto>> Update(int id, [FromBody] TipEntryUpdateDto dto)
    {
        var entry = await db.TipEntries.FindAsync(id);
        if (entry is null) return NotFound();

        if (dto.Amount <= 0)
            return BadRequest("Amount must be positive");

        entry.Date = dto.Date;
        entry.Amount = dto.Amount;
        entry.Source = dto.Source;

        await db.SaveChangesAsync();

        return Ok(new TipEntryReadDto(entry.Id, entry.Date, entry.Amount, entry.Source));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await db.TipEntries.FindAsync(id);
        if (entry is null) return NotFound();

        db.TipEntries.Remove(entry);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
