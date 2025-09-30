using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Enums;
using TaskStatus = FertilizerWarehouseAPI.Models.Enums.TaskStatus;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get dashboard overview data
        /// </summary>
        [HttpGet("dashboard")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> GetDashboardData()
        {
            try
            {
                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                var startOfYear = new DateTime(today.Year, 1, 1);

                var dashboard = new
                {
                    // Overview Stats
                    TotalWarehouses = await _context.Warehouses.CountAsync(w => w.IsActive),
                    TotalProducts = await _context.Products.CountAsync(p => p.IsActive),
                    TotalEmployees = await _context.Users.CountAsync(u => u.IsActive),
                    TotalOrders = await _context.PurchaseOrders.CountAsync(po => po.IsActive) + 
                                 await _context.SalesOrders.CountAsync(so => so.IsActive),

                    // Today's Activity
                    OrdersToday = await _context.PurchaseOrders.CountAsync(po => po.IsActive && po.CreatedAt.Date == today) +
                                 await _context.SalesOrders.CountAsync(so => so.IsActive && so.CreatedAt.Date == today),
                    TasksCompleted = await _context.Tasks.CountAsync(t => t.IsActive && 
                                                                         t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today),
                    ActiveUsers = await _context.Users.CountAsync(u => u.IsActive && 
                                                                      u.LastLoginAt.HasValue && u.LastLoginAt.Value.Date == today),

                    // Monthly Stats
                    OrdersThisMonth = await _context.PurchaseOrders.CountAsync(po => po.IsActive && po.CreatedAt >= startOfMonth) +
                                     await _context.SalesOrders.CountAsync(so => so.IsActive && so.CreatedAt >= startOfMonth),
                    RevenueThisMonth = await _context.SalesOrders
                        .Where(so => so.IsActive && so.CreatedAt >= startOfMonth)
                        .SumAsync(so => (decimal?)so.TotalAmount) ?? 0,
                    ExpensesThisMonth = await _context.PurchaseOrders
                        .Where(po => po.IsActive && po.CreatedAt >= startOfMonth)
                        .SumAsync(po => (decimal?)po.TotalAmount) ?? 0,

                    // Year Stats
                    RevenueThisYear = await _context.SalesOrders
                        .Where(so => so.IsActive && so.CreatedAt >= startOfYear)
                        .SumAsync(so => (decimal?)so.TotalAmount) ?? 0,
                    ExpensesThisYear = await _context.PurchaseOrders
                        .Where(po => po.IsActive && po.CreatedAt >= startOfYear)
                        .SumAsync(po => (decimal?)po.TotalAmount) ?? 0,

                    // Task Statistics
                    TotalTasks = await _context.Tasks.CountAsync(t => t.IsActive),
                    PendingTasks = await _context.Tasks.CountAsync(t => t.IsActive && t.Status == TaskStatus.Pending),
                    InProgressTasks = await _context.Tasks.CountAsync(t => t.IsActive && t.Status == TaskStatus.InProgress),
                    CompletedTasks = await _context.Tasks.CountAsync(t => t.IsActive && t.Status == TaskStatus.Completed),
                    OverdueTasks = await _context.Tasks.CountAsync(t => t.IsActive && 
                                                                      t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed),

                    // Recent Activities (Last 10)
                    RecentOrders = await _context.PurchaseOrders
                        .Where(po => po.IsActive)
                        .OrderByDescending(po => po.CreatedAt)
                        .Take(5)
                        .Select(po => new
                        {
                            Type = "Purchase Order",
                            po.Id,
                            po.OrderNumber,
                            po.OrderDate,
                            po.Status,
                            po.TotalAmount,
                            Description = $"Purchase order {po.OrderNumber} - {po.Status}"
                        })
                        .ToListAsync(),

                    RecentSalesOrders = await _context.SalesOrders
                        .Where(so => so.IsActive)
                        .OrderByDescending(so => so.CreatedAt)
                        .Take(5)
                        .Select(so => new
                        {
                            Type = "Sales Order",
                            so.Id,
                            so.OrderNumber,
                            so.OrderDate,
                            so.Status,
                            so.TotalAmount,
                            Description = $"Sales order {so.OrderNumber} - {so.Status}"
                        })
                        .ToListAsync(),

                    RecentTasks = await _context.Tasks
                        .Include(t => t.AssignedToUser)
                        .Where(t => t.IsActive)
                        .OrderByDescending(t => t.CreatedAt)
                        .Take(5)
                        .Select(t => new
                        {
                            Type = "Task",
                            t.Id,
                            t.Title,
                            t.Status,
                            t.Priority,
                            t.CreatedAt,
                            AssignedTo = t.AssignedToUser != null ? t.AssignedToUser.FullName : "Unassigned",
                            Description = $"Task: {t.Title} - {t.Status}"
                        })
                        .ToListAsync()
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard data", error = ex.Message });
            }
        }

        /// <summary>
        /// Get inventory report
        /// </summary>
        [HttpGet("inventory")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> GetInventoryReport()
        {
            try
            {
                var inventoryReport = new
                {
                    GeneratedAt = DateTime.UtcNow,
                    Summary = new
                    {
                        TotalProducts = await _context.Products.CountAsync(p => p.IsActive),
                        TotalBatches = await _context.ProductBatches.CountAsync(pb => pb.IsActive),
                        LowStockProducts = await _context.Products.CountAsync(p => p.IsActive), // TODO: Implement low stock logic
                        ExpiredBatches = await _context.ProductBatches.CountAsync(pb => pb.IsActive && pb.ExpiryDate < DateTime.UtcNow),
                        ExpiringBatches = await _context.ProductBatches.CountAsync(pb => pb.IsActive && 
                                                                                      pb.ExpiryDate > DateTime.UtcNow && 
                                                                                      pb.ExpiryDate <= DateTime.UtcNow.AddDays(30))
                    },
                    ProductsByCategory = await _context.ProductCategories
                        .Where(pc => pc.IsActive)
                        .Select(pc => new
                        {
                            CategoryName = pc.CategoryName,
                            ProductCount = _context.Products.Count(p => p.CategoryId == pc.Id && p.IsActive),
                            // TotalValue = _context.StockItems.Where(si => si.Product.CategoryId == pc.Id && si.IsActive)
                            //                              .Sum(si => si.Quantity * si.Product.UnitPrice)
                        })
                        .ToListAsync(),
                    WarehouseCapacity = await _context.Warehouses
                        .Where(w => w.IsActive)
                        .Select(w => new
                        {
                            w.Name,
                            w.TotalPositions,
                            // UsedPositions = _context.StockItems.Count(si => si.WarehouseId == w.Id && si.IsActive),
                            UsedPositions = 0, // TODO: Implement when stock items are ready
                            UtilizationPercentage = w.TotalPositions > 0 ? 0 : 0 // (UsedPositions / w.TotalPositions) * 100
                        })
                        .ToListAsync(),
                    RecentBatches = await _context.ProductBatches
                        .Include(pb => pb.Product)
                        .Where(pb => pb.IsActive)
                        .OrderByDescending(pb => pb.CreatedAt)
                        .Take(10)
                        .Select(pb => new
                        {
                            pb.Id,
                            pb.BatchNumber,
                            ProductName = pb.Product.ProductName,
                            pb.ProductionDate,
                            pb.ExpiryDate,
                            pb.InitialQuantity,
                            pb.CurrentQuantity,
                            pb.QualityStatus,
                            DaysToExpiry = pb.ExpiryDate.HasValue ? (int)(pb.ExpiryDate.Value - DateTime.UtcNow).TotalDays : 0
                        })
                        .ToListAsync()
                };

                return Ok(inventoryReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating inventory report", error = ex.Message });
            }
        }

        /// <summary>
        /// Get sales report
        /// </summary>
        [HttpGet("sales")]
        [Authorize(Policy = "SalesStaff")]
        public async Task<ActionResult<object>> GetSalesReport([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var startDate = fromDate ?? DateTime.UtcNow.AddMonths(-1);
                var endDate = toDate ?? DateTime.UtcNow;

                var salesReport = new
                {
                    ReportPeriod = new { StartDate = startDate, EndDate = endDate },
                    GeneratedAt = DateTime.UtcNow,
                    Summary = new
                    {
                        TotalOrders = await _context.SalesOrders.CountAsync(so => so.IsActive && 
                                                                               so.OrderDate >= startDate && so.OrderDate <= endDate),
                        TotalRevenue = await _context.SalesOrders
                            .Where(so => so.IsActive && so.OrderDate >= startDate && so.OrderDate <= endDate)
                            .SumAsync(so => (decimal?)so.TotalAmount) ?? 0,
                        AverageOrderValue = await _context.SalesOrders
                            .Where(so => so.IsActive && so.OrderDate >= startDate && so.OrderDate <= endDate)
                            .AverageAsync(so => (decimal?)so.TotalAmount) ?? 0,
                        CompletedOrders = await _context.SalesOrders.CountAsync(so => so.IsActive && 
                                                                                    so.OrderDate >= startDate && so.OrderDate <= endDate &&
                                                                                    so.Status == OrderStatus.Completed),
                        PendingOrders = await _context.SalesOrders.CountAsync(so => so.IsActive && 
                                                                                 so.OrderDate >= startDate && so.OrderDate <= endDate &&
                                                                                 so.Status == OrderStatus.Pending)
                    },
                    OrdersByStatus = await _context.SalesOrders
                        .Where(so => so.IsActive && so.OrderDate >= startDate && so.OrderDate <= endDate)
                        .GroupBy(so => so.Status)
                        .Select(g => new
                        {
                            Status = g.Key.ToString(),
                            Count = g.Count(),
                            TotalValue = g.Sum(so => so.TotalAmount)
                        })
                        .ToListAsync(),
                    TopCustomers = await _context.SalesOrders
                        .Include(so => so.Customer)
                        .Where(so => so.IsActive && so.OrderDate >= startDate && so.OrderDate <= endDate)
                        .GroupBy(so => so.Customer)
                        .Select(g => new
                        {
                            CustomerId = g.Key.Id,
                            CustomerName = g.Key.CustomerName,
                            OrderCount = g.Count(),
                            TotalValue = g.Sum(so => so.TotalAmount),
                            AverageOrderValue = g.Average(so => so.TotalAmount)
                        })
                        .OrderByDescending(c => c.TotalValue)
                        .Take(10)
                        .ToListAsync(),
                    DailySales = await _context.SalesOrders
                        .Where(so => so.IsActive && so.OrderDate >= startDate && so.OrderDate <= endDate)
                        .GroupBy(so => so.OrderDate.Date)
                        .Select(g => new
                        {
                            Date = g.Key,
                            OrderCount = g.Count(),
                            TotalValue = g.Sum(so => so.TotalAmount)
                        })
                        .OrderBy(d => d.Date)
                        .ToListAsync()
                };

                return Ok(salesReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating sales report", error = ex.Message });
            }
        }

        /// <summary>
        /// Get purchasing report
        /// </summary>
        [HttpGet("purchasing")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<object>> GetPurchasingReport([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var startDate = fromDate ?? DateTime.UtcNow.AddMonths(-1);
                var endDate = toDate ?? DateTime.UtcNow;

                var purchasingReport = new
                {
                    ReportPeriod = new { StartDate = startDate, EndDate = endDate },
                    GeneratedAt = DateTime.UtcNow,
                    Summary = new
                    {
                        TotalOrders = await _context.PurchaseOrders.CountAsync(po => po.IsActive && 
                                                                                  po.OrderDate >= startDate && po.OrderDate <= endDate),
                        TotalSpent = await _context.PurchaseOrders
                            .Where(po => po.IsActive && po.OrderDate >= startDate && po.OrderDate <= endDate)
                            .SumAsync(po => (decimal?)po.TotalAmount) ?? 0,
                        AverageOrderValue = await _context.PurchaseOrders
                            .Where(po => po.IsActive && po.OrderDate >= startDate && po.OrderDate <= endDate)
                            .AverageAsync(po => (decimal?)po.TotalAmount) ?? 0,
                        ReceivedOrders = await _context.PurchaseOrders.CountAsync(po => po.IsActive && 
                                                                                     po.OrderDate >= startDate && po.OrderDate <= endDate &&
                                                                                     po.Status == OrderStatus.Delivered),
                        PendingOrders = await _context.PurchaseOrders.CountAsync(po => po.IsActive && 
                                                                                    po.OrderDate >= startDate && po.OrderDate <= endDate &&
                                                                                    po.Status == OrderStatus.Pending)
                    },
                    OrdersByStatus = await _context.PurchaseOrders
                        .Where(po => po.IsActive && po.OrderDate >= startDate && po.OrderDate <= endDate)
                        .GroupBy(po => po.Status)
                        .Select(g => new
                        {
                            Status = g.Key.ToString(),
                            Count = g.Count(),
                            TotalValue = g.Sum(po => po.TotalAmount)
                        })
                        .ToListAsync(),
                    TopSuppliers = await _context.PurchaseOrders
                        .Include(po => po.Supplier)
                        .Where(po => po.IsActive && po.OrderDate >= startDate && po.OrderDate <= endDate)
                        .GroupBy(po => po.Supplier)
                        .Select(g => new
                        {
                            SupplierId = g.Key.Id,
                            SupplierName = g.Key.SupplierName,
                            OrderCount = g.Count(),
                            TotalValue = g.Sum(po => po.TotalAmount),
                            AverageOrderValue = g.Average(po => po.TotalAmount)
                        })
                        .OrderByDescending(s => s.TotalValue)
                        .Take(10)
                        .ToListAsync(),
                    MonthlySpending = await _context.PurchaseOrders
                        .Where(po => po.IsActive && po.OrderDate >= startDate && po.OrderDate <= endDate)
                        .GroupBy(po => new { Year = po.OrderDate.Year, Month = po.OrderDate.Month })
                        .Select(g => new
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            OrderCount = g.Count(),
                            TotalSpent = g.Sum(po => po.TotalAmount)
                        })
                        .OrderBy(m => m.Year).ThenBy(m => m.Month)
                        .ToListAsync()
                };

                return Ok(purchasingReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating purchasing report", error = ex.Message });
            }
        }

        /// <summary>
        /// Get employee performance report
        /// </summary>
        [HttpGet("employee-performance")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<object>> GetEmployeePerformanceReport()
        {
            try
            {
                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                var performanceReport = new
                {
                    GeneratedAt = DateTime.UtcNow,
                    Summary = new
                    {
                        TotalEmployees = await _context.Users.CountAsync(u => u.IsActive),
                        ActiveToday = await _context.Users.CountAsync(u => u.IsActive && 
                                                                         u.LastLoginAt.HasValue && u.LastLoginAt.Value.Date == today),
                        TasksCompletedToday = await _context.Tasks.CountAsync(t => t.IsActive && 
                                                                                 t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today),
                        AverageTasksPerEmployee = await _context.Tasks.CountAsync(t => t.IsActive) / 
                                                (double)Math.Max(await _context.Users.CountAsync(u => u.IsActive), 1)
                    },
                    EmployeeStats = await _context.Users
                        .Where(u => u.IsActive)
                        .Select(u => new
                        {
                            u.Id,
                            u.Username,
                            u.FullName,
                            u.Role,
                            RoleName = u.Role.ToString(),
                            u.LastLoginAt,
                            Name = u.Department != null ? u.Department.Name : "No Department",
                            // Task statistics
                            TotalTasks = _context.Tasks.Count(t => t.AssignedTo == u.Id && t.IsActive),
                            CompletedTasks = _context.Tasks.Count(t => t.AssignedTo == u.Id && t.IsActive && t.Status == TaskStatus.Completed),
                            PendingTasks = _context.Tasks.Count(t => t.AssignedTo == u.Id && t.IsActive && t.Status == TaskStatus.Pending),
                            OverdueTasks = _context.Tasks.Count(t => t.AssignedTo == u.Id && t.IsActive && 
                                                               t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed),
                            CompletedThisMonth = _context.Tasks.Count(t => t.AssignedTo == u.Id && t.IsActive && 
                                                                     t.CompletedAt.HasValue && t.CompletedAt.Value >= startOfMonth),
                            AverageCompletionTime = _context.Tasks
                                .Where(t => t.AssignedTo == u.Id && t.IsActive && t.Status == TaskStatus.Completed && t.ActualHours > 0)
                                .Average(t => (double?)t.ActualHours) ?? 0,
                            // Performance score (simple calculation)
                            PerformanceScore = _context.Tasks.Count(t => t.AssignedTo == u.Id && t.IsActive) > 0 ?
                                             (double)_context.Tasks.Count(t => t.AssignedTo == u.Id && t.IsActive && t.Status == TaskStatus.Completed) /
                                             _context.Tasks.Count(t => t.AssignedTo == u.Id && t.IsActive) * 100 : 0
                        })
                        .OrderByDescending(u => u.PerformanceScore)
                        .ToListAsync(),
                    DepartmentStats = await _context.Departments
                        .Where(d => d.IsActive)
                        .Select(d => new
                        {
                            d.Id,
                            d.Name,
                            EmployeeCount = _context.Users.Count(u => u.DepartmentId == d.Id && u.IsActive),
                            TotalTasks = _context.Tasks.Count(t => t.DepartmentId == d.Id && t.IsActive),
                            CompletedTasks = _context.Tasks.Count(t => t.DepartmentId == d.Id && t.IsActive && t.Status == TaskStatus.Completed),
                            OverdueTasks = _context.Tasks.Count(t => t.DepartmentId == d.Id && t.IsActive && 
                                                               t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed),
                            DepartmentPerformance = _context.Tasks.Count(t => t.DepartmentId == d.Id && t.IsActive) > 0 ?
                                                  (double)_context.Tasks.Count(t => t.DepartmentId == d.Id && t.IsActive && t.Status == TaskStatus.Completed) /
                                                  _context.Tasks.Count(t => t.DepartmentId == d.Id && t.IsActive) * 100 : 0
                        })
                        .OrderByDescending(d => d.DepartmentPerformance)
                        .ToListAsync()
                };

                return Ok(performanceReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating employee performance report", error = ex.Message });
            }
        }

        /// <summary>
        /// Export report to CSV format
        /// </summary>
        [HttpGet("export/{reportType}")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult> ExportReport(string reportType)
        {
            try
            {
                string csvContent = "";
                string fileName = $"{reportType}_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

                switch (reportType.ToLower())
                {
                    case "inventory":
                        csvContent = await GenerateInventoryCsv();
                        break;
                    case "sales":
                        csvContent = await GenerateSalesCsv();
                        break;
                    case "purchasing":
                        csvContent = await GeneratePurchasingCsv();
                        break;
                    case "employees":
                        csvContent = await GenerateEmployeeCsv();
                        break;
                    default:
                        return BadRequest(new { message = "Invalid report type" });
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while exporting report", error = ex.Message });
            }
        }

        private async Task<string> GenerateInventoryCsv()
        {
            var products = await _context.Products
                .Include(p => p.CategoryNavigation)
                .Where(p => p.IsActive)
                .Select(p => new
                {
                    p.ProductCode,
                    p.ProductName,
                    CategoryName = p.CategoryNavigation != null ? p.CategoryNavigation.CategoryName : "Unknown",
                    p.Unit,
                    p.UnitPrice,
                    p.MinStockLevel,
                    p.MaxStockLevel
                })
                .ToListAsync();

            var csv = "Product Code,Product Name,Category,Unit,Unit Price,Min Stock,Max Stock\n";
            foreach (var product in products)
            {
                csv += $"{product.ProductCode},{product.ProductName},{product.CategoryName},{product.Unit},{product.UnitPrice},{product.MinStockLevel},{product.MaxStockLevel}\n";
            }

            return csv;
        }

        private async Task<string> GenerateSalesCsv()
        {
            var orders = await _context.SalesOrders
                .Include(so => so.Customer)
                .Where(so => so.IsActive)
                .Select(so => new
                {
                    so.OrderNumber,
                    so.OrderDate,
                    CustomerName = so.Customer.CustomerName,
                    so.Status,
                    so.TotalAmount
                })
                .ToListAsync();

            var csv = "Order Number,Order Date,Customer,Status,Total Amount\n";
            foreach (var order in orders)
            {
                csv += $"{order.OrderNumber},{order.OrderDate:yyyy-MM-dd},{order.CustomerName},{order.Status},{order.TotalAmount}\n";
            }

            return csv;
        }

        private async Task<string> GeneratePurchasingCsv()
        {
            var orders = await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Where(po => po.IsActive)
                .Select(po => new
                {
                    po.OrderNumber,
                    po.OrderDate,
                    SupplierName = po.Supplier.SupplierName,
                    po.Status,
                    po.TotalAmount
                })
                .ToListAsync();

            var csv = "Order Number,Order Date,Supplier,Status,Total Amount\n";
            foreach (var order in orders)
            {
                csv += $"{order.OrderNumber},{order.OrderDate:yyyy-MM-dd},{order.SupplierName},{order.Status},{order.TotalAmount}\n";
            }

            return csv;
        }

        private async Task<string> GenerateEmployeeCsv()
        {
            var employees = await _context.Users
                .Include(u => u.Department)
                .Where(u => u.IsActive)
                .Select(u => new
                {
                    u.Username,
                    u.FullName,
                    u.Email,
                    u.Role,
                    Name = u.Department != null ? u.Department.Name : "No Department",
                    u.CreatedAt,
                    u.LastLoginAt
                })
                .ToListAsync();

            var csv = "Username,Full Name,Email,Role,Department,Created Date,Last Login\n";
            foreach (var emp in employees)
            {
                csv += $"{emp.Username},{emp.FullName},{emp.Email},{emp.Role},{emp.Name},{emp.CreatedAt:yyyy-MM-dd},{emp.LastLoginAt?.ToString("yyyy-MM-dd") ?? "Never"}\n";
            }

            return csv;
        }
    }
}
