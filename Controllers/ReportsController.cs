using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/reports/dashboard
        [HttpGet("dashboard")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var totalProducts = await _context.Products.CountAsync();
                var totalUsers = await _context.Users.CountAsync();
                var totalCompanies = await _context.Companies.CountAsync();
                var totalSuppliers = await _context.Suppliers.CountAsync();

                var dashboardData = new
                {
                    TotalProducts = totalProducts,
                    TotalUsers = totalUsers,
                    TotalCompanies = totalCompanies,
                    TotalSuppliers = totalSuppliers,
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(new { success = true, data = dashboardData });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dashboard data: {ex.Message}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi tải dữ liệu dashboard", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/reports/production-weekly
        [HttpGet("production-weekly")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWeeklyProductionData()
        {
            try
            {
                var startDate = DateTime.UtcNow.AddDays(-7);
                var endDate = DateTime.UtcNow;

                // Mock data for weekly production
                var weeklyData = new
                {
                    Week = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                    TotalProduction = 1000,
                    ProductsProduced = new[]
                    {
                        new { ProductName = "NPK HaiDuong 20-10-24", Quantity = 500 },
                        new { ProductName = "NPK HaiDuong 15-15-15", Quantity = 300 },
                        new { ProductName = "Phân hữu cơ khoáng HD 304", Quantity = 200 }
                    },
                    Efficiency = 85.5
                };

                return Ok(new { success = true, data = weeklyData });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading weekly data: {ex.Message}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi tải dữ liệu tuần", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/reports/production-details
        [HttpGet("production-details")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductionDetails()
        {
            try
            {
                var products = await _context.Products
                    .Select(p => new
                    {
                        p.Id,
                        p.ProductName,
                        p.ProductCode,
                        p.Unit,
                        p.UnitPrice,
                        p.Price,
                        p.Status,
                        p.CreatedAt
                    })
                    .OrderBy(p => p.ProductName)
                    .ToListAsync();

                var productionDetails = new
                {
                    TotalProducts = products.Count,
                    Products = products,
                    GeneratedAt = DateTime.UtcNow
                };

                return Ok(new { success = true, data = productionDetails });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading production details: {ex.Message}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi tải chi tiết sản xuất", 
                    error = ex.Message 
                });
            }
        }
    }
}