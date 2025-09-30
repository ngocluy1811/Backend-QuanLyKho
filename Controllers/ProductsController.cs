using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<object>> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.Products
                .Include(p => p.Company)
                .Include(p => p.SupplierNavigation)
                .Where(p => p.IsActive);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var products = await query
                .Select(p => new
                {
                    p.Id,
                    p.ProductCode,
                    p.ProductName,
                    p.Description,
                    p.Unit,
                    p.UnitPrice,
                    p.Price,
                    p.MinStockLevel,
                    p.MaxStockLevel,
                    p.Status,
                    p.IsActive,
                    p.CreatedAt,
                    p.UpdatedAt,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company != null ? p.Company.CompanyName : null,
                    SupplierId = p.SupplierId,
                    SupplierName = p.SupplierNavigation != null ? p.SupplierNavigation.SupplierName : null,
                    ProductionDate = p.ProductionDate,
                    ExpiryDate = p.ExpiryDate,
                    BatchNumber = p.BatchNumber
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                products,
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
            return StatusCode(500, new { message = "An error occurred while fetching products", error = ex.Message });
        }
    }

    // GET: api/Products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetProduct(int id)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Company)
                .Include(p => p.SupplierNavigation)
                .Where(p => p.Id == id && p.IsActive)
                .Select(p => new
                {
                    p.Id,
                    p.ProductCode,
                    p.ProductName,
                    p.Description,
                    p.Unit,
                    p.UnitPrice,
                    p.Price,
                    p.MinStockLevel,
                    p.MaxStockLevel,
                    p.Status,
                    p.IsActive,
                    p.CreatedAt,
                    p.UpdatedAt,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company != null ? p.Company.CompanyName : null,
                    SupplierId = p.SupplierId,
                    SupplierName = p.SupplierNavigation != null ? p.SupplierNavigation.SupplierName : null,
                    ProductionDate = p.ProductionDate,
                    ExpiryDate = p.ExpiryDate,
                    BatchNumber = p.BatchNumber
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching product", error = ex.Message });
        }
    }

    // POST: api/Products
    [HttpPost]
    public async Task<ActionResult<object>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrEmpty(request.ProductCode) || string.IsNullOrEmpty(request.ProductName))
            {
                return BadRequest(new { message = "ProductCode and ProductName are required" });
            }

            // Check if product code already exists
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductCode == request.ProductCode);
            
            if (existingProduct != null)
            {
                return BadRequest(new { message = "Product code already exists" });
            }

            // Get category name if CategoryId is provided
            string categoryName = string.Empty;
            if (request.CategoryId.HasValue)
            {
                var category = await _context.ProductCategories
                    .Where(c => c.Id == request.CategoryId.Value)
                    .Select(c => c.CategoryName)
                    .FirstOrDefaultAsync();
                categoryName = category ?? string.Empty;
            }

            var product = new Product
            {
                ProductCode = request.ProductCode,
                ProductName = request.ProductName,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Category = categoryName,
                UnitPrice = request.UnitPrice,
                Price = request.Price,
                Unit = request.Unit ?? "kg",
                MinStockLevel = request.MinStockLevel,
                MaxStockLevel = request.MaxStockLevel,
                CompanyId = request.CompanyId,
                SupplierId = request.SupplierId,
                Status = request.Status ?? "Active",
                ProductionDate = request.ProductionDate,
                ExpiryDate = request.ExpiryDate,
                BatchNumber = request.BatchNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Return the created product with related data
            var createdProduct = await _context.Products
                .Include(p => p.Company)
                .Include(p => p.SupplierNavigation)
                .Where(p => p.Id == product.Id)
                .Select(p => new
                {
                    p.Id,
                    p.ProductCode,
                    p.ProductName,
                    p.Description,
                    p.Unit,
                    p.UnitPrice,
                    p.Price,
                    p.MinStockLevel,
                    p.MaxStockLevel,
                    p.Status,
                    p.IsActive,
                    p.CreatedAt,
                    p.UpdatedAt,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company != null ? p.Company.CompanyName : null,
                    SupplierId = p.SupplierId,
                    SupplierName = p.SupplierNavigation != null ? p.SupplierNavigation.SupplierName : null,
                    ProductionDate = p.ProductionDate,
                    ExpiryDate = p.ExpiryDate,
                    BatchNumber = p.BatchNumber
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, createdProduct);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating product", error = ex.Message });
        }
    }

    // PUT: api/Products/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<object>> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Check if product code already exists (excluding current product)
            if (!string.IsNullOrEmpty(request.ProductCode) && request.ProductCode != product.ProductCode)
            {
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductCode == request.ProductCode && p.Id != id);
                
                if (existingProduct != null)
                {
                    return BadRequest(new { message = "Product code already exists" });
                }
            }

            // Update product properties
            if (!string.IsNullOrEmpty(request.ProductCode))
                product.ProductCode = request.ProductCode;
            if (!string.IsNullOrEmpty(request.ProductName))
                product.ProductName = request.ProductName;
            if (request.Description != null)
                product.Description = request.Description;
            if (request.CategoryId.HasValue)
            {
                product.CategoryId = request.CategoryId;
                // Update category name
                var category = await _context.ProductCategories
                    .Where(c => c.Id == request.CategoryId.Value)
                    .Select(c => c.CategoryName)
                    .FirstOrDefaultAsync();
                product.Category = category ?? string.Empty;
            }
            if (request.UnitPrice.HasValue)
                product.UnitPrice = request.UnitPrice.Value;
            if (request.Price.HasValue)
                product.Price = request.Price.Value;
            if (!string.IsNullOrEmpty(request.Unit))
                product.Unit = request.Unit;
            if (request.MinStockLevel.HasValue)
                product.MinStockLevel = request.MinStockLevel.Value;
            if (request.MaxStockLevel.HasValue)
                product.MaxStockLevel = request.MaxStockLevel.Value;
            if (request.CompanyId.HasValue)
                product.CompanyId = request.CompanyId;
            if (request.SupplierId.HasValue)
                product.SupplierId = request.SupplierId;
            if (!string.IsNullOrEmpty(request.Status))
                product.Status = request.Status;
            if (request.IsActive.HasValue)
                product.IsActive = request.IsActive.Value;
            if (request.ProductionDate.HasValue)
                product.ProductionDate = request.ProductionDate;
            if (request.ExpiryDate.HasValue)
                product.ExpiryDate = request.ExpiryDate;
            if (request.BatchNumber != null)
                product.BatchNumber = request.BatchNumber;

            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Return the updated product with related data
            var updatedProduct = await _context.Products
                .Include(p => p.Company)
                .Include(p => p.SupplierNavigation)
                .Where(p => p.Id == product.Id)
                .Select(p => new
                {
                    p.Id,
                    p.ProductCode,
                    p.ProductName,
                    p.Description,
                    p.Unit,
                    p.UnitPrice,
                    p.Price,
                    p.MinStockLevel,
                    p.MaxStockLevel,
                    p.Status,
                    p.IsActive,
                    p.CreatedAt,
                    p.UpdatedAt,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company != null ? p.Company.CompanyName : null,
                    SupplierId = p.SupplierId,
                    SupplierName = p.SupplierNavigation != null ? p.SupplierNavigation.SupplierName : null,
                    ProductionDate = p.ProductionDate,
                    ExpiryDate = p.ExpiryDate,
                    BatchNumber = p.BatchNumber
                })
                .FirstOrDefaultAsync();

            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating product", error = ex.Message });
        }
    }

    // DELETE: api/Products/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Soft delete - set IsActive to false
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting product", error = ex.Message });
        }
    }
}

// Request DTOs
public class CreateProductRequest
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Price { get; set; }
    public string? Unit { get; set; }
    public int MinStockLevel { get; set; }
    public int MaxStockLevel { get; set; }
    public int? CompanyId { get; set; }
    public int? SupplierId { get; set; }
    public string? Status { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? BatchNumber { get; set; }
}

public class UpdateProductRequest
{
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Price { get; set; }
    public string? Unit { get; set; }
    public int? MinStockLevel { get; set; }
    public int? MaxStockLevel { get; set; }
    public int? CompanyId { get; set; }
    public int? SupplierId { get; set; }
    public string? Status { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? BatchNumber { get; set; }
}