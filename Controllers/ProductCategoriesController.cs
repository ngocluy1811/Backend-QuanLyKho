using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all product categories
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductCategories()
        {
            try
            {
                var categories = await _context.ProductCategories
                    .Include(pc => pc.ParentCategory)
                    .Where(pc => pc.IsActive)
                    .Select(pc => new
                    {
                        pc.Id,
                        pc.Code,
                        pc.CategoryName,
                        pc.Description,
                        pc.ParentCategoryId,
                        ParentCategoryName = pc.ParentCategory != null ? pc.ParentCategory.CategoryName : null,
                        pc.IsActive,
                        pc.CreatedAt,
                        // Statistics
                        ProductCount = _context.Products.Count(p => p.CategoryId == pc.Id && p.IsActive),
                        SubCategoryCount = _context.ProductCategories.Count(sc => sc.ParentCategoryId == pc.Id && sc.IsActive)
                    })
                    .OrderBy(pc => pc.CategoryName)
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving product categories", error = ex.Message });
            }
        }

        /// <summary>
        /// Get product category by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> GetProductCategory(int id)
        {
            try
            {
                var category = await _context.ProductCategories
                    .Include(pc => pc.ParentCategory)
                    .Where(pc => pc.Id == id && pc.IsActive)
                    .Select(pc => new
                    {
                        pc.Id,
                        pc.Code,
                        pc.CategoryName,
                        pc.Description,
                        pc.ParentCategoryId,
                        ParentCategoryName = pc.ParentCategory != null ? pc.ParentCategory.CategoryName : null,
                        pc.IsActive,
                        pc.CreatedAt,
                        // Statistics
                        ProductCount = _context.Products.Count(p => p.CategoryId == pc.Id && p.IsActive),
                        SubCategoryCount = _context.ProductCategories.Count(sc => sc.ParentCategoryId == pc.Id && sc.IsActive),
                        // Sub categories
                        SubCategories = _context.ProductCategories
                            .Where(sc => sc.ParentCategoryId == pc.Id && sc.IsActive)
                            .Select(sc => new
                            {
                                sc.Id,
                                sc.CategoryName,
                                sc.Description,
                                ProductCount = _context.Products.Count(p => p.CategoryId == sc.Id && p.IsActive)
                            }),
                        // Recent products
                        RecentProducts = _context.Products
                            .Where(p => p.CategoryId == pc.Id && p.IsActive)
                            .OrderByDescending(p => p.CreatedAt)
                            .Take(5)
                            .Select(p => new
                            {
                                p.Id,
                                p.ProductCode,
                                p.ProductName,
                                p.UnitPrice,
                                p.CreatedAt
                            })
                    })
                    .FirstOrDefaultAsync();

                if (category == null)
                    return NotFound(new { message = "Product category not found" });

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving product category", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new product category
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<object>> CreateProductCategory([FromBody] CreateProductCategoryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Validate parent category exists if specified
                if (createDto.ParentCategoryId.HasValue)
                {
                    var parentExists = await _context.ProductCategories
                        .AnyAsync(pc => pc.Id == createDto.ParentCategoryId.Value && pc.IsActive);
                    
                    if (!parentExists)
                        return BadRequest(new { message = "Parent category not found" });
                }

                var category = new ProductCategory
                {
                    CompanyId = createDto.CompanyId,
                    Code = createDto.Code,
                    CategoryName = createDto.CategoryName,
                    Description = createDto.Description,
                    ParentCategoryId = createDto.ParentCategoryId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.ProductCategories.Add(category);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProductCategory), new { id = category.Id }, new
                {
                    category.Id,
                    category.Code,
                    category.CategoryName,
                    category.Description,
                    category.ParentCategoryId,
                    category.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating product category", error = ex.Message });
            }
        }

        /// <summary>
        /// Update product category
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<object>> UpdateProductCategory(int id, [FromBody] UpdateProductCategoryDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var category = await _context.ProductCategories.FindAsync(id);
                if (category == null || !category.IsActive)
                    return NotFound(new { message = "Product category not found" });

                // Validate parent category if specified
                if (updateDto.ParentCategoryId.HasValue)
                {
                    // Check if trying to set self as parent
                    if (updateDto.ParentCategoryId.Value == id)
                        return BadRequest(new { message = "Category cannot be its own parent" });

                    var parentExists = await _context.ProductCategories
                        .AnyAsync(pc => pc.Id == updateDto.ParentCategoryId.Value && pc.IsActive);
                    
                    if (!parentExists)
                        return BadRequest(new { message = "Parent category not found" });
                }

                category.Code = updateDto.Code;
                category.CategoryName = updateDto.CategoryName;
                category.Description = updateDto.Description;
                category.ParentCategoryId = updateDto.ParentCategoryId;
                category.IsActive = updateDto.IsActive;
                category.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    category.Id,
                    category.CategoryName,
                    category.Description,
                    category.ParentCategoryId,
                    category.IsActive,
                    category.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating product category", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete product category
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult> DeleteProductCategory(int id)
        {
            try
            {
                var category = await _context.ProductCategories.FindAsync(id);
                if (category == null)
                    return NotFound(new { message = "Product category not found" });

                // Check if category has products
                var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id && p.IsActive);
                if (hasProducts)
                    return BadRequest(new { message = "Cannot delete category with existing products" });

                // Check if category has sub-categories
                var hasSubCategories = await _context.ProductCategories.AnyAsync(pc => pc.ParentCategoryId == id && pc.IsActive);
                if (hasSubCategories)
                    return BadRequest(new { message = "Cannot delete category with existing sub-categories" });

                category.IsActive = false;
                category.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting product category", error = ex.Message });
            }
        }

        /// <summary>
        /// Get category tree (hierarchical structure)
        /// </summary>
        [HttpGet("tree")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<object>>> GetCategoryTree()
        {
            try
            {
                var allCategories = await _context.ProductCategories
                    .Where(pc => pc.IsActive)
                    .Select(pc => new
                    {
                        pc.Id,
                        pc.CategoryName,
                        pc.Description,
                        pc.ParentCategoryId,
                        ProductCount = _context.Products.Count(p => p.CategoryId == pc.Id && p.IsActive)
                    })
                    .ToListAsync();

                // Build tree structure
                var categoryList = allCategories.Cast<dynamic>().ToList();
                var rootCategories = allCategories
                    .Where(c => c.ParentCategoryId == null)
                    .Select(c => BuildCategoryTree(c, categoryList))
                    .ToList();

                return Ok(rootCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving category tree", error = ex.Message });
            }
        }

        /// <summary>
        /// Get categories summary
        /// </summary>
        [HttpGet("summary")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> GetCategoriesSummary()
        {
            try
            {
                var summary = new
                {
                    TotalCategories = await _context.ProductCategories.CountAsync(pc => pc.IsActive),
                    RootCategories = await _context.ProductCategories.CountAsync(pc => pc.IsActive && pc.ParentCategoryId == null),
                    SubCategories = await _context.ProductCategories.CountAsync(pc => pc.IsActive && pc.ParentCategoryId != null),
                    CategoriesWithProducts = await _context.ProductCategories
                        .Where(pc => pc.IsActive)
                        .CountAsync(pc => _context.Products.Any(p => p.CategoryId == pc.Id && p.IsActive)),
                    EmptyCategories = await _context.ProductCategories
                        .Where(pc => pc.IsActive)
                        .CountAsync(pc => !_context.Products.Any(p => p.CategoryId == pc.Id && p.IsActive)),
                    TopCategories = await _context.ProductCategories
                        .Where(pc => pc.IsActive)
                        .Select(pc => new
                        {
                            pc.Id,
                            pc.CategoryName,
                            ProductCount = _context.Products.Count(p => p.CategoryId == pc.Id && p.IsActive)
                        })
                        .OrderByDescending(pc => pc.ProductCount)
                        .Take(5)
                        .ToListAsync()
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving categories summary", error = ex.Message });
            }
        }

        private class CategoryTreeNode
        {
            public int Id { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int? ParentCategoryId { get; set; }
            public int ProductCount { get; set; }
            public List<CategoryTreeNode> Children { get; set; } = new List<CategoryTreeNode>();
        }

        private CategoryTreeNode BuildCategoryTree(dynamic category, List<dynamic> allCategories)
        {
            var children = allCategories
                .Where(c => c.ParentCategoryId == category.Id)
                .Select(c => BuildCategoryTree(c, allCategories))
                .Cast<CategoryTreeNode>()
                .ToList();

            return new CategoryTreeNode
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId,
                ProductCount = category.ProductCount,
                Children = children
            };
        }
    }

    // DTOs for Product Categories
    public class CreateProductCategoryDto
    {
        public int CompanyId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
    }

    public class UpdateProductCategoryDto
    {
        public string Code { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}
