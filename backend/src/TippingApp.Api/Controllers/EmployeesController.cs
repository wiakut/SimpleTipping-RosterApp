using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippingApp.Api.Data;
using TippingApp.Api.DTOs;

namespace TippingApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetAll()
    {
        var employees = await db.Employees
            .OrderBy(e => e.Name)
            .Select(e => new EmployeeReadDto(e.Id, e.Name, e.Role))
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeReadDto>> GetById(int id)
    {
        var emp = await db.Employees.FindAsync(id);
        if (emp is null) return NotFound();
        return Ok(new EmployeeReadDto(emp.Id, emp.Name, emp.Role));
    }
}
