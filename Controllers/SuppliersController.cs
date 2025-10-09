using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SuppliersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Suppliers
    [HttpGet]
    public async Task<ActionResult<object>> GetSuppliers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.Suppliers
                .Where(s => s.IsActive);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var suppliers = await query
                .Select(s => new
                {
                    s.Id,
                    s.Code,
                    s.SupplierName,
                    s.Address,
                    s.Phone,
                    s.Email,
                    s.ContactPerson,
                    s.IsActive,
                    s.CreatedAt,
                    s.UpdatedAt
                })
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                suppliers,
                pagination = new
                {
                    currentPage = page,
                    pageSize,
                    totalCount,
                    totalPages,
                    hasNextPage = page < totalPages,
                    hasPreviousPage = page > 1
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching suppliers", error = ex.Message });
        }
    }

    // GET: api/Suppliers/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetSupplier(int id)
    {
        try
        {
            var supplier = await _context.Suppliers
                .Where(s => s.Id == id && s.IsActive)
                .Select(s => new
                {
                    s.Id,
                    s.Code,
                    s.SupplierName,
                    s.Address,
                    s.Phone,
                    s.Email,
                    s.ContactPerson,
                    s.IsActive,
                    s.CreatedAt,
                    s.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (supplier == null)
            {
                return NotFound(new { message = "Supplier not found" });
            }

            return Ok(supplier);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching supplier", error = ex.Message });
        }
    }

    // POST: api/Suppliers
    [HttpPost]
    public async Task<ActionResult<object>> CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrEmpty(request.SupplierName))
            {
                return BadRequest(new { message = "SupplierName is required" });
            }

            // Check if supplier name already exists
            var existingSupplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierName == request.SupplierName);
            
            if (existingSupplier != null)
            {
                return BadRequest(new { message = "Supplier name already exists" });
            }

            // Get the first available company ID
            var firstCompany = await _context.Companies
                .Where(c => c.IsActive)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
            
            if (firstCompany == 0)
            {
                return BadRequest(new { message = "No active company found" });
            }

            var supplier = new Supplier
            {
                CompanyId = firstCompany,
                Code = request.SupplierName.ToUpper().Replace(" ", "_"), // Generate code from name
                SupplierName = request.SupplierName,
                CompanyName = "Default Company", // You might want to get this from company
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email,
                ContactPerson = request.ContactPerson,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            // Return the created supplier
            var createdSupplier = await _context.Suppliers
                .Where(s => s.Id == supplier.Id)
                .Select(s => new
                {
                    s.Id,
                    s.Code,
                    s.SupplierName,
                    s.Address,
                    s.Phone,
                    s.Email,
                    s.ContactPerson,
                    s.IsActive,
                    s.CreatedAt,
                    s.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, createdSupplier);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating supplier", error = ex.Message });
        }
    }

    // PUT: api/Suppliers/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<object>> UpdateSupplier(int id, [FromBody] UpdateSupplierRequest request)
    {
        try
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound(new { message = "Supplier not found" });
            }

            // Check if supplier name already exists (excluding current supplier)
            if (!string.IsNullOrEmpty(request.SupplierName) && request.SupplierName != supplier.SupplierName)
            {
                var existingSupplier = await _context.Suppliers
                    .FirstOrDefaultAsync(s => s.SupplierName == request.SupplierName && s.Id != id);
                
                if (existingSupplier != null)
                {
                    return BadRequest(new { message = "Supplier name already exists" });
                }
            }

            // Update supplier properties
            if (!string.IsNullOrEmpty(request.SupplierName))
                supplier.SupplierName = request.SupplierName;
            if (request.Address != null)
                supplier.Address = request.Address;
            if (request.Phone != null)
                supplier.Phone = request.Phone;
            if (request.Email != null)
                supplier.Email = request.Email;
            if (request.ContactPerson != null)
                supplier.ContactPerson = request.ContactPerson;
            if (request.IsActive.HasValue)
                supplier.IsActive = request.IsActive.Value;

            supplier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Return the updated supplier
            var updatedSupplier = await _context.Suppliers
                .Where(s => s.Id == supplier.Id)
                .Select(s => new
                {
                    s.Id,
                    s.Code,
                    s.SupplierName,
                    s.Address,
                    s.Phone,
                    s.Email,
                    s.ContactPerson,
                    s.IsActive,
                    s.CreatedAt,
                    s.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return Ok(updatedSupplier);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating supplier", error = ex.Message });
        }
    }

    // DELETE: api/Suppliers/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSupplier(int id)
    {
        try
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound(new { message = "Supplier not found" });
            }

            // Soft delete - set IsActive to false
            supplier.IsActive = false;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Supplier deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting supplier", error = ex.Message });
        }
    }
}

// Request DTOs
public class CreateSupplierRequest
{
    public string SupplierName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ContactPerson { get; set; }
}

public class UpdateSupplierRequest
{
    public string? SupplierName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ContactPerson { get; set; }
    public bool? IsActive { get; set; }
}