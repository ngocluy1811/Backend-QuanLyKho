using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models;

namespace FertilizerWarehouseAPI.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/overview
        [HttpGet("overview")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOverview()
    {
        try
        {
                // Tổng sản phẩm
                var totalProducts = await _context.Products.CountAsync();
                
                // Tồn kho (tính theo giá trị)
                var inventoryValue = await _context.ProductBatches
                    .Where(pb => pb.Quantity > 0)
                    .Join(_context.Products, pb => pb.ProductId, p => p.Id, (pb, p) => new { pb.Quantity, p.Price })
                    .SumAsync(x => x.Quantity * x.Price);

                // Tổng đơn hàng
                var totalOrders = await _context.ImportOrders.CountAsync();

                // Tổng nhân viên
                var totalEmployees = await _context.Users.CountAsync();

                // Tính tăng trưởng (so với tháng trước)
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
                var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;

                // Sản phẩm tháng trước
                var lastMonthProducts = await _context.Products
                    .Where(p => p.CreatedAt.Month == lastMonth && p.CreatedAt.Year == lastMonthYear)
                    .CountAsync();

                // Đơn hàng tháng trước
                var lastMonthOrders = await _context.ImportOrders
                    .Where(o => o.CreatedAt.Month == lastMonth && o.CreatedAt.Year == lastMonthYear)
                    .CountAsync();

                // Tính phần trăm tăng trưởng
                var productGrowth = lastMonthProducts > 0 ? 
                    Math.Round(((double)(totalProducts - lastMonthProducts) / lastMonthProducts) * 100, 1) : 0;
                
                var orderGrowth = lastMonthOrders > 0 ? 
                    Math.Round(((double)(totalOrders - lastMonthOrders) / lastMonthOrders) * 100, 1) : 0;

                var inventoryGrowth = -3.0; // Mock data cho tồn kho
                var employeeGrowth = 2; // Mock data cho nhân viên

                var overview = new
                {
                    totalProducts = new
                    {
                        value = totalProducts,
                        growth = $"+{productGrowth}%",
                        trend = "up"
                    },
                    inventory = new
                    {
                        value = $"{inventoryValue / 1000000:F1}M VNĐ",
                        growth = $"{inventoryGrowth}%",
                        trend = "down"
                    },
                    totalOrders = new
                    {
                        value = totalOrders,
                        growth = $"+{orderGrowth}%",
                        trend = "up"
                    },
                    totalEmployees = new
                    {
                        value = totalEmployees,
                        growth = $"+{employeeGrowth}",
                        trend = "up"
                    }
                };

                return Ok(new { success = true, data = overview });
        }
        catch (Exception ex)
        {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy dữ liệu tổng quan", error = ex.Message });
            }
        }

        // GET: api/dashboard/recent-activities
        [HttpGet("recent-activities")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecentActivities()
    {
        try
        {
                var activities = new List<object>();

                // Lấy hoạt động nhập kho gần đây (tạm thời comment vì chưa có Type)
                var recentImports = new List<object>();
                /*
                var recentImports = await _context.ImportOrders
                    .Where(o => o.Type == "Import")
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(3)
                    .Select(o => new
                    {
                        type = "Nhập kho",
                        description = $"{o.ProductName}",
                        user = o.CreatedBy,
                        date = o.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                    })
                    .ToListAsync();
                */

                // Lấy hoạt động xuất kho gần đây (tạm thời comment vì chưa có Type)
                var recentExports = new List<object>();
                /*
                var recentExports = await _context.ExportOrders
                    .Where(o => o.Type == "Export")
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(3)
                    .Select(o => new
                    {
                        type = "Xuất kho",
                        description = $"{o.ProductName}",
                        user = o.CreatedBy,
                        date = o.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                    })
                    .ToListAsync();
                */

                // Lấy hoạt động kiểm kê gần đây
                var recentInventoryChecks = await _context.InventoryChecks
                    .OrderByDescending(ic => ic.CreatedAt)
                    .Take(2)
                    .Select(ic => new
                    {
                        type = "Kiểm kê",
                        description = $"Kho", // Tạm thời hardcode
                        user = ic.CreatedBy,
                        date = ic.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                    })
                    .ToListAsync();

                // Lấy hoạt động chấm công gần đây
                var recentAttendance = await _context.AttendanceRecords
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(2)
                    .Select(a => new
                    {
                        type = "Chấm công",
                        description = $"Check-in/out",
                        user = a.UserId.ToString(),
                        date = a.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                    })
                    .ToListAsync();

                activities.AddRange(recentImports);
                activities.AddRange(recentExports);
                activities.AddRange(recentInventoryChecks);
                activities.AddRange(recentAttendance);

                // Sắp xếp theo thời gian
                var sortedActivities = activities
                    .OrderByDescending(a => DateTime.Parse(a.GetType().GetProperty("date").GetValue(a).ToString()))
                    .Take(5)
                    .ToList();

                return Ok(new { success = true, data = sortedActivities });
        }
        catch (Exception ex)
        {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy hoạt động gần đây", error = ex.Message });
            }
        }

        // GET: api/dashboard/alerts
        [HttpGet("alerts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAlerts()
    {
        try
        {
                // Lấy sản phẩm có tồn kho thấp
                var lowStockProducts = await _context.ProductBatches
                    .Where(pb => pb.Quantity <= 10) // Tồn kho <= 10
                    .Join(_context.Products, pb => pb.ProductId, p => p.Id, (pb, p) => new
                    {
                        productName = p.ProductName,
                        currentStock = pb.Quantity,
                        unit = p.Unit,
                        alertType = "LowStock"
                    })
                    .ToListAsync();

                // Lấy sản phẩm hết hạn sử dụng
                var expiredProducts = await _context.ProductBatches
                    .Where(pb => pb.ExpiryDate <= DateTime.Now.AddDays(30)) // Hết hạn trong 30 ngày
                    .Join(_context.Products, pb => pb.ProductId, p => p.Id, (pb, p) => new
                    {
                        productName = p.ProductName,
                        expiryDate = pb.ExpiryDate,
                        alertType = "Expiry"
                    })
                    .ToListAsync();

                var alerts = new List<object>();
                alerts.AddRange(lowStockProducts);
                alerts.AddRange(expiredProducts);

                return Ok(new { success = true, data = alerts });
        }
        catch (Exception ex)
        {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy cảnh báo", error = ex.Message });
            }
        }

        // GET: api/dashboard/best-selling-products
        [HttpGet("best-selling-products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBestSellingProducts()
        {
            try
            {
                // Lấy sản phẩm bán chạy nhất (tạm thời comment vì chưa có Type)
                var bestSellingProducts = new List<object>();
                /*
                var bestSellingProducts = await _context.ImportOrders
                    .Where(o => o.Type == "Export")
                    .GroupBy(o => new { o.ProductId, o.ProductName })
                    .Select(g => new
                    {
                        productId = g.Key.ProductId,
                        productName = g.Key.ProductName,
                        totalQuantity = g.Sum(o => o.Quantity),
                        totalRevenue = g.Sum(o => o.Quantity * o.UnitPrice),
                        orderCount = g.Count()
                    })
                    .OrderByDescending(p => p.totalQuantity)
                    .Take(5)
                    .ToListAsync();
                */

                var result = bestSellingProducts.Select((p, index) => new
                {
                    rank = index + 1,
                    productName = "Sản phẩm mẫu", // Mock data
                    productId = index + 1,
                    revenue = $"{new Random().Next(1000000, 10000000):N0} ₫", // Mock revenue
                    growth = $"+{new Random().Next(5, 20)}%", // Mock growth data
                    totalQuantity = new Random().Next(10, 100) // Mock quantity
                }).ToList();

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy sản phẩm bán chạy", error = ex.Message });
            }
        }

        // GET: api/dashboard/sales-analytics
        [HttpGet("sales-analytics")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSalesAnalytics()
        {
            try
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;

                // Doanh thu tháng này (tạm thời comment vì chưa có Type)
                var currentMonthRevenue = 0m;
                /*
                var currentMonthRevenue = await _context.ImportOrders
                    .Where(o => o.Type == "Export" && o.CreatedAt.Month == currentMonth && o.CreatedAt.Year == currentYear)
                    .SumAsync(o => o.Quantity * o.UnitPrice);
                */

                // Doanh thu tháng trước (tạm thời comment vì chưa có Type)
                var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
                var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;
                
                var lastMonthRevenue = 0m;
                /*
                var lastMonthRevenue = await _context.ImportOrders
                    .Where(o => o.Type == "Export" && o.CreatedAt.Month == lastMonth && o.CreatedAt.Year == lastMonthYear)
                    .SumAsync(o => o.Quantity * o.UnitPrice);
                */

                // Tính tăng trưởng doanh thu
                var revenueGrowth = lastMonthRevenue > 0 ? 
                    Math.Round(((double)(currentMonthRevenue - lastMonthRevenue) / (double)lastMonthRevenue) * 100, 1) : 0;

                var analytics = new
                {
                    currentMonthRevenue = $"{currentMonthRevenue:N0} ₫",
                    lastMonthRevenue = $"{lastMonthRevenue:N0} ₫",
                    growth = $"{revenueGrowth}%",
                    trend = revenueGrowth >= 0 ? "up" : "down"
                };

                return Ok(new { success = true, data = analytics });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy phân tích bán hàng", error = ex.Message });
            }
        }
    }
}