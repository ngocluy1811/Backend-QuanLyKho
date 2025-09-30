using Microsoft.AspNetCore.Mvc;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("login")]
    public IActionResult TestLogin()
    {
        var result = new
        {
            success = true,
            message = "Login successful",
            token = "mock-jwt-token-12345",
            refreshToken = "mock-refresh-token-67890",
            tokenExpiry = DateTime.UtcNow.AddHours(1),
            user = new
            {
                id = 1,
                username = "admin",
                fullName = "Administrator",
                email = "admin@example.com",
                role = "Admin",
                department = "IT"
            }
        };
        return Ok(result);
    }

    [HttpGet("dashboard")]
    public IActionResult TestDashboard()
    {
        var result = new
        {
            totalProducts = 1245,
            totalUsers = 45,
            totalWarehouses = 3,
            activeProducts = 1200,
            activeUsers = 42,
            totalStockValue = 2500000000,
            lowStockProducts = 15,
            recentActivities = new object[]
            {
                new { id = 1, description = "User 'admin' logged in", timestamp = DateTime.Now.AddMinutes(-30), type = "Login" },
                new { id = 2, description = "Product 'Urea' stock updated", timestamp = DateTime.Now.AddHours(-2), type = "StockUpdate" },
                new { id = 3, description = "New production order #PO001 created", timestamp = DateTime.Now.AddDays(-1), type = "ProductionOrder" }
            },
            alerts = new object[]
            {
                new { id = 1, alertType = "LowStock", message = "Sản phẩm NPK 16-16-8 sắp hết hàng - Tồn kho hiện tại: 50 tấn", severity = "High", entityType = "Product", entityId = 1, createdAt = DateTime.Now.AddHours(-1), isRead = false },
                new { id = 2, alertType = "Expiry", message = "Lô NPK-001 sắp hết hạn - Sản phẩm sẽ hết hạn trong 7 ngày tới", severity = "Medium", entityType = "Product", entityId = 2, createdAt = DateTime.Now.AddHours(-2), isRead = false },
                new { id = 3, alertType = "Capacity", message = "Vị trí A12 đã đầy - Cần điều chuyển hàng hóa để giải phóng không gian", severity = "High", entityType = "Warehouse", entityId = 1, createdAt = DateTime.Now.AddMinutes(-15), isRead = false }
            },
            monthlySales = new object[]
            {
                new { month = "Tháng 1", sales = 1500000000, orders = 45, growth = 12.5 },
                new { month = "Tháng 2", sales = 1800000000, orders = 52, growth = 8.3 },
                new { month = "Tháng 3", sales = 2200000000, orders = 68, growth = 15.2 }
            },
            topProducts = new object[]
            {
                new { rank = 1, productName = "NPK 16-16-8", productCode = "NPK-16-16-8", sales = 450000000, growth = 8.5 },
                new { rank = 2, productName = "Urê 46% N", productCode = "UREA-46", sales = 380000000, growth = 12.3 },
                new { rank = 3, productName = "DAP 18-46", productCode = "DAP-18-46", sales = 320000000, growth = 5.7 }
            },
            warehouseUtilization = new object[]
            {
                new { warehouseName = "Kho A", utilization = 85.5, capacity = 1000, used = 855 },
                new { warehouseName = "Kho B", utilization = 72.3, capacity = 800, used = 578 },
                new { warehouseName = "Kho C", utilization = 91.2, capacity = 600, used = 547 }
            }
        };
        return Ok(result);
    }

    [HttpGet("products")]
    public IActionResult TestProducts()
    {
        var result = new[]
        {
            new
            {
                id = 1,
                productCode = "NPK-16-16-8",
                productName = "NPK 16-16-8",
                description = "Phân bón NPK 16-16-8",
                unit = "tấn",
                unitPrice = 15000000,
                currentStock = 100,
                minStock = 10,
                maxStock = 1000,
                category = "Phân NPK",
                supplier = "Công ty ABC",
                lastUpdated = DateTime.Now.AddDays(-1),
                status = "active"
            },
            new
            {
                id = 2,
                productCode = "UREA-46",
                productName = "Urê 46% N",
                description = "Phân bón Urê 46% N",
                unit = "tấn",
                unitPrice = 12000000,
                currentStock = 80,
                minStock = 10,
                maxStock = 800,
                category = "Phân Urê",
                supplier = "Công ty XYZ",
                lastUpdated = DateTime.Now.AddDays(-2),
                status = "active"
            },
            new
            {
                id = 3,
                productCode = "DAP-18-46",
                productName = "DAP 18-46",
                description = "Phân bón DAP 18-46",
                unit = "tấn",
                unitPrice = 18000000,
                currentStock = 60,
                minStock = 10,
                maxStock = 600,
                category = "Phân DAP",
                supplier = "Công ty DEF",
                lastUpdated = DateTime.Now.AddDays(-3),
                status = "low"
            }
        };
        return Ok(result);
    }
}
