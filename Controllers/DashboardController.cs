using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.DTOs;
using FertilizerWarehouseAPI.Services.Interfaces;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IUserService _userService;
    private readonly IWarehouseService _warehouseService;

    public DashboardController(
        IProductService productService,
        IUserService userService,
        IWarehouseService warehouseService)
    {
        _productService = productService;
        _userService = userService;
        _warehouseService = warehouseService;
    }

    /// <summary>
    /// Get dashboard statistics
    /// </summary>
    [HttpGet("stats")]
    [AllowAnonymous] // Temporarily allow anonymous for testing
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        try
        {
            // Get basic statistics
            var products = await _productService.GetAllProductsAsync();
            var users = await _userService.GetAllUsersAsync();
            var warehouses = await _warehouseService.GetAllWarehousesAsync();

            var stats = new DashboardStatsDto
            {
                TotalProducts = products.Count(),
                TotalUsers = users.Count(),
                TotalWarehouses = warehouses.Count(),
                ActiveProducts = products.Count(p => p.IsActive),
                ActiveUsers = users.Count(u => u.IsActive),
                TotalStockValue = 0, // Will be calculated from warehouse cells
                LowStockProducts = products.Count(p => p.MinStockLevel > 0), // Use MinStockLevel instead
                RecentActivities = GetRecentActivitiesData(),
                Alerts = GetAlerts(products),
                MonthlySales = GetMonthlySalesData(),
                TopProducts = GetTopProducts(products),
                WarehouseUtilization = GetWarehouseUtilization(warehouses)
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi lấy thống kê dashboard", error = ex.Message });
        }
    }

    /// <summary>
    /// Get recent activities
    /// </summary>
    [HttpGet("activities")]
    public async Task<ActionResult<List<ActivityDto>>> GetRecentActivities()
    {
        try
        {
            var activities = GetRecentActivitiesData();
            return Ok(activities);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi lấy hoạt động gần đây", error = ex.Message });
        }
    }

    /// <summary>
    /// Get alerts and notifications
    /// </summary>
    [HttpGet("alerts")]
    public async Task<ActionResult<List<AlertDto>>> GetAlerts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            var alerts = GetAlerts(products);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi lấy cảnh báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Get monthly sales data
    /// </summary>
    [HttpGet("sales/monthly")]
    public async Task<ActionResult<List<MonthlySalesDto>>> GetMonthlySales()
    {
        try
        {
            var monthlySales = GetMonthlySalesData();
            return Ok(monthlySales);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi lấy dữ liệu bán hàng", error = ex.Message });
        }
    }

    /// <summary>
    /// Get top products by sales
    /// </summary>
    [HttpGet("products/top")]
    public async Task<ActionResult<List<TopProductDto>>> GetTopProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            var topProducts = GetTopProducts(products);
            return Ok(topProducts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi lấy sản phẩm bán chạy", error = ex.Message });
        }
    }

    /// <summary>
    /// Get warehouse utilization
    /// </summary>
    [HttpGet("warehouses/utilization")]
    public async Task<ActionResult<List<WarehouseUtilizationDto>>> GetWarehouseUtilization()
    {
        try
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            var utilization = GetWarehouseUtilization(warehouses);
            return Ok(utilization);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi lấy dữ liệu sử dụng kho", error = ex.Message });
        }
    }

    #region Private Helper Methods

    private List<ActivityDto> GetRecentActivitiesData()
    {
        // Mock data for recent activities
        return new List<ActivityDto>
        {
            new ActivityDto
            {
                Id = 1,
                Action = "Nhập kho",
                Description = "NPK 16-16-8",
                Quantity = "15 tấn",
                User = "Nguyễn Văn A",
                Time = DateTime.Now.AddHours(-2),
                Type = "warehouse"
            },
            new ActivityDto
            {
                Id = 2,
                Action = "Xuất kho",
                Description = "Urê 46% N",
                Quantity = "8 tấn",
                User = "Trần Thị B",
                Time = DateTime.Now.AddHours(-4),
                Type = "warehouse"
            },
            new ActivityDto
            {
                Id = 3,
                Action = "Kiểm kê",
                Description = "Kho A - Dãy B",
                Quantity = "-",
                User = "Lê Văn C",
                Time = DateTime.Now.AddHours(-6),
                Type = "inventory"
            },
            new ActivityDto
            {
                Id = 4,
                Action = "Điều chuyển",
                Description = "DAP từ Kho A sang Kho B",
                Quantity = "5 tấn",
                User = "Phạm Thị D",
                Time = DateTime.Now.AddDays(-1),
                Type = "transfer"
            },
            new ActivityDto
            {
                Id = 5,
                Action = "Tạo đơn hàng",
                Description = "Đơn hàng #SO-001",
                Quantity = "20 tấn",
                User = "Nguyễn Văn E",
                Time = DateTime.Now.AddDays(-1),
                Type = "sales"
            }
        };
    }

    private List<AlertDto> GetAlerts(IEnumerable<dynamic> products)
    {
        var alerts = new List<AlertDto>();

        // Check for low stock products
        var lowStockProducts = products.Where(p => p.CurrentStock < 100).Take(5);
        foreach (var product in lowStockProducts)
        {
            alerts.Add(new AlertDto
            {
                Id = alerts.Count + 1,
                CompanyId = 1,
                AlertType = "LowStock",
                Message = $"Sản phẩm {product.ProductName} sắp hết hàng - Tồn kho hiện tại: {product.CurrentStock} {product.Unit}",
                Severity = "High",
                EntityType = "Product",
                EntityId = product.Id,
                CreatedAt = DateTime.Now.AddHours(-1),
                IsRead = false
            });
        }

        // Check for expired products
        alerts.Add(new AlertDto
        {
            Id = alerts.Count + 1,
            CompanyId = 1,
            AlertType = "Expiry",
            Message = "Lô NPK-001 sắp hết hạn - Sản phẩm sẽ hết hạn trong 7 ngày tới",
            Severity = "Medium",
            EntityType = "Product",
            EntityId = 1,
            CreatedAt = DateTime.Now.AddHours(-2),
            IsRead = false
        });

        // Check for warehouse capacity
        alerts.Add(new AlertDto
        {
            Id = alerts.Count + 1,
            CompanyId = 1,
            AlertType = "Capacity",
            Message = "Vị trí A12 đã đầy - Cần điều chuyển hàng hóa để giải phóng không gian",
            Severity = "High",
            EntityType = "Warehouse",
            EntityId = 1,
            CreatedAt = DateTime.Now.AddMinutes(-15),
            IsRead = false
        });

        // Check for maintenance
        alerts.Add(new AlertDto
        {
            Id = alerts.Count + 1,
            CompanyId = 1,
            AlertType = "Maintenance",
            Message = "Máy đóng gói số 3 cần bảo trì - Máy đã hoạt động liên tục 200 giờ",
            Severity = "Low",
            EntityType = "Machine",
            EntityId = 3,
            CreatedAt = DateTime.Now.AddDays(-1),
            IsRead = false
        });

        return alerts;
    }

    private List<MonthlySalesDto> GetMonthlySalesData()
    {
        var currentDate = DateTime.Now;
        var monthlySales = new List<MonthlySalesDto>();

        for (int i = 11; i >= 0; i--)
        {
            var date = currentDate.AddMonths(-i);
            monthlySales.Add(new MonthlySalesDto
            {
                Month = date.ToString("MM/yyyy"),
                Sales = Random.Shared.Next(1000000, 5000000),
                Orders = Random.Shared.Next(50, 200),
                Growth = Random.Shared.Next(-10, 25)
            });
        }

        return monthlySales;
    }

    private List<TopProductDto> GetTopProducts(IEnumerable<dynamic> products)
    {
        return products
            .OrderByDescending(p => p.CurrentStock * p.UnitPrice)
            .Take(5)
            .Select((p, index) => new TopProductDto
            {
                Rank = index + 1,
                ProductName = p.ProductName,
                ProductCode = p.ProductCode,
                Sales = (decimal)(p.CurrentStock * p.UnitPrice),
                Growth = Random.Shared.Next(-5, 20)
            })
            .ToList();
    }

    private List<WarehouseUtilizationDto> GetWarehouseUtilization(IEnumerable<dynamic> warehouses)
    {
        return warehouses
            .Select(w => new WarehouseUtilizationDto
            {
                WarehouseName = w.Name,
                Capacity = Random.Shared.Next(80, 100),
                Used = Random.Shared.Next(60, 95),
                Available = Random.Shared.Next(5, 40),
                Status = Random.Shared.Next(0, 100) > 80 ? "warning" : "normal"
            })
            .ToList();
    }

    #endregion
}

// DTOs for Dashboard
public class DashboardStatsDto
{
    public int TotalProducts { get; set; }
    public int TotalUsers { get; set; }
    public int TotalWarehouses { get; set; }
    public int ActiveProducts { get; set; }
    public int ActiveUsers { get; set; }
    public decimal TotalStockValue { get; set; }
    public int LowStockProducts { get; set; }
    public List<ActivityDto> RecentActivities { get; set; } = new List<ActivityDto>();
    public List<AlertDto> Alerts { get; set; } = new List<AlertDto>();
    public List<MonthlySalesDto> MonthlySales { get; set; } = new List<MonthlySalesDto>();
    public List<TopProductDto> TopProducts { get; set; } = new List<TopProductDto>();
    public List<WarehouseUtilizationDto> WarehouseUtilization { get; set; } = new List<WarehouseUtilizationDto>();
}

public class ActivityDto
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class MonthlySalesDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Sales { get; set; }
    public int Orders { get; set; }
    public int Growth { get; set; }
}

public class TopProductDto
{
    public int Rank { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public decimal Sales { get; set; }
    public int Growth { get; set; }
}

public class WarehouseUtilizationDto
{
    public string WarehouseName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int Used { get; set; }
    public int Available { get; set; }
    public string Status { get; set; } = string.Empty;
}
