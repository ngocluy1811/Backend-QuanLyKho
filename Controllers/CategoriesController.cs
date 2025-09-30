using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Categories
    [HttpGet]
    public async Task<ActionResult<object>> GetCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.ProductCategories
                .Where(c => c.IsActive);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var categories = await query
                .Select(c => new
                {
                    c.Id,
                    c.Code,
                    c.CategoryName,
                    c.Description,
                    c.IsActive,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                categories,
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
            return StatusCode(500, new { message = "An error occurred while fetching categories", error = ex.Message });
        }
    }

    // GET: api/Categories/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetCategory(int id)
    {
        try
        {
            var category = await _context.ProductCategories
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => new
                {
                    c.Id,
                    c.Code,
                    c.CategoryName,
                    c.Description,
                    c.IsActive,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching category", error = ex.Message });
        }
    }

    // POST: api/Categories
    [HttpPost]
    public async Task<ActionResult<object>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrEmpty(request.CategoryName))
            {
                return BadRequest(new { message = "CategoryName is required" });
            }

            // Check if category name already exists
            var existingCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(c => c.CategoryName == request.CategoryName);
            
            if (existingCategory != null)
            {
                return BadRequest(new { message = "Category name already exists" });
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

            var category = new ProductCategory
            {
                CompanyId = firstCompany,
                Code = request.CategoryName.ToUpper().Replace(" ", "_"), // Generate code from name
                CategoryName = request.CategoryName,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();

            // Return the created category
            var createdCategory = await _context.ProductCategories
                .Where(c => c.Id == category.Id)
                .Select(c => new
                {
                    c.Id,
                    c.Code,
                    c.CategoryName,
                    c.Description,
                    c.IsActive,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, createdCategory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating category", error = ex.Message });
        }
    }

    // PUT: api/Categories/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<object>> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            // Check if category name already exists (excluding current category)
            if (!string.IsNullOrEmpty(request.CategoryName) && request.CategoryName != category.CategoryName)
            {
                var existingCategory = await _context.ProductCategories
                    .FirstOrDefaultAsync(c => c.CategoryName == request.CategoryName && c.Id != id);
                
                if (existingCategory != null)
                {
                    return BadRequest(new { message = "Category name already exists" });
                }
            }

            // Update category properties
            if (!string.IsNullOrEmpty(request.CategoryName))
                category.CategoryName = request.CategoryName;
            if (request.Description != null)
                category.Description = request.Description;
            if (request.IsActive.HasValue)
                category.IsActive = request.IsActive.Value;

            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Return the updated category
            var updatedCategory = await _context.ProductCategories
                .Where(c => c.Id == category.Id)
                .Select(c => new
                {
                    c.Id,
                    c.Code,
                    c.CategoryName,
                    c.Description,
                    c.IsActive,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return Ok(updatedCategory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating category", error = ex.Message });
        }
    }

    // DELETE: api/Categories/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        try
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            // Soft delete - set IsActive to false
            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting category", error = ex.Message });
        }
    }
}

// Request DTOs
public class CreateCategoryRequest
{
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCategoryRequest
{
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
