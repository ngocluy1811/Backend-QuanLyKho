using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CompaniesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Companies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetCompanies()
    {
        try
        {
            var companies = await _context.Companies
                .Where(c => c.IsActive)
                .Select(c => new
                {
                    c.Id,
                    c.CompanyName,
                    Description = (string?)null, // Company doesn't have Description field
                    c.IsActive,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .ToListAsync();

            return Ok(companies);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching companies", error = ex.Message });
        }
    }
}