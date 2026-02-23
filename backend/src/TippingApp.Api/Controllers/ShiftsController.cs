using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippingApp.Api.Data;
using TippingApp.Api.DTOs;
using TippingApp.Api.Models;

namespace TippingApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShiftReadDto>>> GetByWeek([FromQuery] DateOnly weekStart)
    {
        var weekEnd = weekStart.AddDays(6);

        var shifts = await db.Shifts
            .Include(s => s.Employee)
            .Where(s => s.Date >= weekStart && s.Date <= weekEnd)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .Select(s => new ShiftReadDto(
                s.Id, s.EmployeeId, s.Employee.Name,
                s.Date, s.StartTime, s.EndTime))
            .ToListAsync();

        return Ok(shifts);
    }

    [HttpPost]
    public async Task<ActionResult<ShiftReadDto>> Create([FromBody] ShiftCreateDto dto)
    {
        var employee = await db.Employees.FindAsync(dto.EmployeeId);
        if (employee is null)
            return BadRequest("Employee not found");

        if (dto.EndTime <= dto.StartTime)
            return BadRequest("End time must be after start time");

        var shift = new Shift
        {
            EmployeeId = dto.EmployeeId,
            Date = dto.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime
        };

        db.Shifts.Add(shift);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByWeek), null,
            new ShiftReadDto(shift.Id, shift.EmployeeId, employee.Name,
                shift.Date, shift.StartTime, shift.EndTime));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ShiftReadDto>> Update(int id, [FromBody] ShiftUpdateDto dto)
    {
        var shift = await db.Shifts.Include(s => s.Employee).FirstOrDefaultAsync(s => s.Id == id);
        if (shift is null) return NotFound();

        if (dto.EndTime <= dto.StartTime)
            return BadRequest("End time must be after start time");

        var employee = await db.Employees.FindAsync(dto.EmployeeId);
        if (employee is null)
            return BadRequest("Employee not found");

        shift.EmployeeId = dto.EmployeeId;
        shift.Date = dto.Date;
        shift.StartTime = dto.StartTime;
        shift.EndTime = dto.EndTime;

        await db.SaveChangesAsync();

        return Ok(new ShiftReadDto(shift.Id, shift.EmployeeId, employee.Name,
            shift.Date, shift.StartTime, shift.EndTime));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var shift = await db.Shifts.FindAsync(id);
        if (shift is null) return NotFound();

        db.Shifts.Remove(shift);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
