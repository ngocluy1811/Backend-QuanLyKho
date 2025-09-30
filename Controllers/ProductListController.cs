using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductListController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductListController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/ProductList
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        try
        {
            var products = await _context.Products
                .Include(p => p.CategoryNavigation)
                .Include(p => p.Company)
                .Where(p => p.IsActive)
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
                    p.CreatedAt,
                    p.UpdatedAt,
                    CategoryName = p.CategoryNavigation != null ? p.CategoryNavigation.CategoryName : "Unknown",
                    CompanyName = p.Company != null ? p.Company.CompanyName : "Unknown"
                })
                .ToListAsync();

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching products", error = ex.Message });
        }
    }

    // GET: api/ProductList/names
    [HttpGet("names")]
    public async Task<ActionResult<IEnumerable<string>>> GetProductNames()
    {
        try
        {
            var productNames = await _context.Products
                .Where(p => p.IsActive)
                .Select(p => p.ProductName)
                .ToListAsync();

            return Ok(productNames);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching product names", error = ex.Message });
        }
    }

    // POST: api/ProductList/seed
    [HttpPost("seed")]
    public async Task<ActionResult> SeedProducts()
    {
        try
        {
            // Check if products already exist
            var existingProducts = await _context.Products.CountAsync();
            if (existingProducts > 0)
            {
                return BadRequest(new { message = "Products already exist in database" });
            }

            // Get the first category and company for reference
            var firstCategory = await _context.ProductCategories.FirstOrDefaultAsync();
            var firstCompany = await _context.Companies.FirstOrDefaultAsync();
            
            if (firstCategory == null || firstCompany == null)
            {
                return BadRequest(new { message = "No categories or companies found. Please seed them first." });
            }

            var products = new List<Product>
            {
                new Product { ProductCode = "NPK001", ProductName = "NPK HaiDương 20-10-24+50B (Chuyên Cà Phê)", CategoryId = firstCategory.Id, UnitPrice = 0, Price = 0, Unit = "kg", MinStockLevel = 0, MaxStockLevel = 1000, CompanyId = firstCompany.Id, Status = "Active", CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "NPK002", ProductName = "NPK HaiDương 15-15-15", CategoryId = firstCategory.Id, UnitPrice = 0, Price = 0, Unit = "kg", MinStockLevel = 0, MaxStockLevel = 1000, CompanyId = firstCompany.Id, Status = "Active", CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "NPK003", ProductName = "Phân hữu cơ khoáng HD 304", CategoryId = firstCategory.Id, UnitPrice = 0, Price = 0, Unit = "kg", MinStockLevel = 0, MaxStockLevel = 1000, CompanyId = firstCompany.Id, Status = "Active", CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "NPK004", ProductName = "Hữu cơ HD BIOMIX", CategoryId = firstCategory.Id, UnitPrice = 0, Price = 0, Unit = "kg", MinStockLevel = 0, MaxStockLevel = 1000, CompanyId = firstCompany.Id, Status = "Active", CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "NPK005", ProductName = "HD Strong", CategoryId = firstCategory.Id, UnitPrice = 0, Price = 0, Unit = "kg", MinStockLevel = 0, MaxStockLevel = 1000, CompanyId = firstCompany.Id, Status = "Active", CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "NPK006", ProductName = "HD Active", CategoryId = firstCategory.Id, UnitPrice = 0, Price = 0, Unit = "kg", MinStockLevel = 0, MaxStockLevel = 1000, CompanyId = firstCompany.Id, Status = "Active", CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "NPK007", ProductName = "HD 302", CategoryId = firstCategory.Id, UnitPrice = 0, Price = 0, Unit = "kg", MinStockLevel = 0, MaxStockLevel = 1000, CompanyId = firstCompany.Id, Status = "Active", CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "NPK008", ProductName = "HD GREEN", CategoryId = firstCategory.Id, UnitPrice = 0, Price = 0, Unit = "kg", MinStockLevel = 0, MaxStockLevel = 1000, CompanyId = firstCompany.Id, Status = "Active", CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "NPK009", ProductName = "HD GOLD", CategoryId = firstCategory.Id, UnitPrice = 0, Price = 0, Unit = "kg", MinStockLevel = 0, MaxStockLevel = 1000, CompanyId = firstCompany.Id, Status = "Active", CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "NPK010", ProductName = "Nitrate Calcium - Boronica HaiDuong", CategoryId = firstCategory.Id, UnitPrice = 0, Price = 0, Unit = "kg", MinStockLevel = 0, MaxStockLevel = 1000, CompanyId = firstCompany.Id, Status = "Active", CreatedAt = DateTime.UtcNow }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Successfully seeded {products.Count} products", count = products.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while seeding products", error = ex.Message });
        }
    }

    // POST: api/ProductList/reseed
    [HttpPost("reseed")]
    public async Task<ActionResult> ReseedProducts()
    {
        try
        {
            // Delete all existing products
            var existingProducts = await _context.Products.ToListAsync();
            _context.Products.RemoveRange(existingProducts);
            await _context.SaveChangesAsync();

            // Get the first category and company for reference
            var firstCategory = await _context.ProductCategories.FirstOrDefaultAsync();
            var firstCompany = await _context.Companies.FirstOrDefaultAsync();
            
            if (firstCategory == null || firstCompany == null)
            {
                return BadRequest(new { message = "No categories or companies found. Please seed them first." });
            }

            // Create products array with names
            string[] productNames = {
                "NPK HaiDương 20-10-24+50B (Chuyên Cà Phê)",
                "NPK HaiDương 15-15-15",
                "Phân hữu cơ khoáng HD 304",
                "Hữu cơ HD BIOMIX",
                "HD Strong",
                "HD Active",
                "HD 302",
                "HD GREEN",
                "HD GOLD",
                "Nitrate Calcium - Boronica HaiDuong",
                "NPK HaiDuong 16-6-20+TE (Sầu Riêng)",
                "NPK HaiDuong 19-9-27",
                "NPK HaiDuong 19-19-19",
                "NPK HaiDuong 20-27-12",
                "NPK HaiDuong 12-18-30",
                "Ca.Mg F1 BIO VN",
                "NPK HaiDuong 20-20-15+50B",
                "HD GROW",
                "NPK HaiDuong 7-7-7+TE",
                "NPK HaiDuong 7-7-7+TE (SẦU RIÊNG)",
                "NPK HaiDuong 16-6-20+TE",
                "NPK HaiDuong 22-5-5-9S+50B",
                "HD ORGANIC ( NHẬT BẢN )",
                "NPK HaiDuong 15-15-15",
                "NPK HaiDuong 22-6-6-9S+TE",
                "HD NUTRI",
                "HD BLU",
                "HD ORGANIC",
                "NPK HaiDuong 20-10-24+50B (Chuyên Thanh Long)",
                "NPK HaiDuong 7-7-7+TE (CÀ PHÊ)",
                "Hữu Cơ Vi Sinh HD 301",
                "Hữu cơ khoáng HD 303",
                "NPK HaiDuong 22-5-5-9S+50B (CAT)",
                "NPK HaiDuong 22-20-15+TE",
                "NPK HaiDuong 22-9-24+50B",
                "NPK HaiDuong 7-7-7+TE (CÀ PHÊ)",
                "NPK HaiDuong 7-7-7+TE (CAO SU)",
                "NPK HaiDuong 30-20-5",
                "NPK HaiDuong 20-4-28+50B",
                "NPK HaiDuong 16-6-20+TE (CAT)",
                "NPK HaiDuong 20-20-15+50B",
                "NPK HaiDuong 25-25+50B",
                "NPK HaiDuong 25-25-5",
                "NPK HaiDuong 16-16-8+TE (CAT)",
                "NPK HaiDuong 16-6-20+TE (LÚA)",
                "NPK HaiDuong 16-16-8+TE (LÚA)",
                "NPK HaiDuong 22-5-5-9S+50B (LÚA)",
                "Ca.Mg F1 BIO VN",
                "Nitrate Calcium - Boronica HaiDuong",
                "Phân hữu cơ khoáng HD 303",
                "Hữu cơ Vi sinh HD 301",
                "HD 302",
                "HD Active",
                "HD Strong",
                "HD GREEN",
                "HD GOLD",
                "HD NUTRI",
                "HD ORGANIC",
                "HD BLU",
                "HD GROW",
                "HD GROW (70.OM)",
                "HD GROW 55.OM"
            };

            var products = new List<Product>();
            for (int i = 0; i < productNames.Length; i++)
            {
                products.Add(new Product
                {
                    ProductCode = $"NPK{(i + 1):D3}",
                    ProductName = productNames[i],
                    CategoryId = firstCategory.Id,
                    UnitPrice = 0,
                    Price = 0,
                    Unit = "kg",
                    MinStockLevel = 0,
                    MaxStockLevel = 1000,
                    CompanyId = firstCompany.Id,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                });
            }

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Successfully reseeded {products.Count} products", count = products.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while reseeding products", error = ex.Message });
        }
    }
}