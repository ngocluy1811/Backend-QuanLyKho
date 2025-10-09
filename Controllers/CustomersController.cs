using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Customers
    [HttpGet]
    public async Task<ActionResult<object>> GetCustomers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.Customers.Where(c => c.IsActive);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var customers = await query
                .Select(c => new
                {
                    c.Id,
                    c.CustomerCode,
                    c.CustomerName,
                    c.Phone,
                    c.Email,
                    c.Address,
                    c.ContactPerson,
                    c.TaxCode,
                    c.Notes,
                    c.Status,
                    c.IsActive,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagination = new
            {
                currentPage = page,
                pageSize,
                totalCount,
                totalPages,
                hasNextPage = page < totalPages,
                hasPreviousPage = page > 1
            };

            return Ok(new
            {
                customers,
                pagination
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving customers", error = ex.Message });
        }
    }

    // GET: api/Customers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving customer", error = ex.Message });
        }
    }

    // POST: api/Customers
    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer(CreateCustomerRequest request)
    {
        try
        {
            // Check if customer code already exists
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerCode == request.CustomerCode && c.IsActive);

            if (existingCustomer != null)
            {
                return BadRequest(new { message = "Customer code already exists" });
            }

            var customer = new Customer
            {
                CustomerCode = request.CustomerCode,
                CustomerName = request.CustomerName,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address,
                ContactPerson = request.ContactPerson,
                TaxCode = request.TaxCode,
                Notes = request.Notes,
                Status = request.Status ?? "Active",
                CompanyId = request.CompanyId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating customer", error = ex.Message });
        }
    }

    // PUT: api/Customers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerRequest request)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            // Check if customer code already exists (excluding current customer)
            if (!string.IsNullOrEmpty(request.CustomerCode) && request.CustomerCode != customer.CustomerCode)
            {
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerCode == request.CustomerCode && c.Id != id && c.IsActive);

                if (existingCustomer != null)
                {
                    return BadRequest(new { message = "Customer code already exists" });
                }
            }

            if (!string.IsNullOrEmpty(request.CustomerCode))
                customer.CustomerCode = request.CustomerCode;
            if (!string.IsNullOrEmpty(request.CustomerName))
                customer.CustomerName = request.CustomerName;
            if (request.Phone != null)
                customer.Phone = request.Phone;
            if (request.Email != null)
                customer.Email = request.Email;
            if (request.Address != null)
                customer.Address = request.Address;
            if (request.ContactPerson != null)
                customer.ContactPerson = request.ContactPerson;
            if (request.TaxCode != null)
                customer.TaxCode = request.TaxCode;
            if (request.Notes != null)
                customer.Notes = request.Notes;
            if (!string.IsNullOrEmpty(request.Status))
                customer.Status = request.Status;
            if (request.CompanyId.HasValue)
                customer.CompanyId = request.CompanyId;

            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating customer", error = ex.Message });
        }
    }

    // DELETE: api/Customers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            // Soft delete
            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting customer", error = ex.Message });
        }
    }
}

// Request DTOs
public class CreateCustomerRequest
{
    [Required]
    [MaxLength(100)]
    public string CustomerCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string CustomerName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    [MaxLength(50)]
    public string? TaxCode { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    public int? CompanyId { get; set; }
}

public class UpdateCustomerRequest
{
    [MaxLength(100)]
    public string? CustomerCode { get; set; }

    [MaxLength(255)]
    public string? CustomerName { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    [MaxLength(50)]
    public string? TaxCode { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    public int? CompanyId { get; set; }
}