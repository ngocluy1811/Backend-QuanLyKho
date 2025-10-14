using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlertController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/alert/all
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAlerts()
        {
            try
            {
                var alerts = new List<object>();

                // Lấy cảnh báo tồn kho thấp
                var lowStockAlerts = await _context.ProductBatches
                    .Where(pb => pb.Quantity <= 10)
                    .Join(_context.Products, pb => pb.ProductId, p => p.Id, (pb, p) => new
                    {
                        id = $"lowstock_{pb.Id}",
                        type = "LowStock",
                        title = "Tồn kho thấp",
                        message = $"Sản phẩm {p.ProductName} chỉ còn {pb.Quantity} {p.Unit}",
                        priority = "high",
                        productId = p.Id,
                        productName = p.ProductName,
                        currentStock = pb.Quantity,
                        unit = p.Unit,
                        threshold = 10,
                        createdAt = pb.UpdatedAt,
                        isResolved = false
                    })
                    .ToListAsync();

                // Lấy cảnh báo hết hạn
                var expiryAlerts = await _context.ProductBatches
                    .Where(pb => pb.ExpiryDate <= DateTime.Now.AddDays(30) && pb.ExpiryDate > DateTime.Now)
                    .Join(_context.Products, pb => pb.ProductId, p => p.Id, (pb, p) => new
                    {
                        id = $"expiry_{pb.Id}",
                        type = "Expiry",
                        title = "Sắp hết hạn",
                        message = $"Sản phẩm {p.ProductName} sẽ hết hạn vào {pb.ExpiryDate:dd/MM/yyyy}",
                        priority = "medium",
                        productId = p.Id,
                        productName = p.ProductName,
                        expiryDate = pb.ExpiryDate,
                        daysUntilExpiry = pb.ExpiryDate.HasValue ? (int)(pb.ExpiryDate.Value - DateTime.Now).TotalDays : 0,
                        createdAt = pb.UpdatedAt,
                        isResolved = false
                    })
                    .ToListAsync();

                // Lấy cảnh báo đã hết hạn
                var expiredAlerts = await _context.ProductBatches
                    .Where(pb => pb.ExpiryDate <= DateTime.Now)
                    .Join(_context.Products, pb => pb.ProductId, p => p.Id, (pb, p) => new
                    {
                        id = $"expired_{pb.Id}",
                        type = "Expired",
                        title = "Đã hết hạn",
                        message = $"Sản phẩm {p.ProductName} đã hết hạn từ {pb.ExpiryDate:dd/MM/yyyy}",
                        priority = "critical",
                        productId = p.Id,
                        productName = p.ProductName,
                        expiryDate = pb.ExpiryDate,
                        daysOverdue = pb.ExpiryDate.HasValue ? (int)(DateTime.Now - pb.ExpiryDate.Value).TotalDays : 0,
                        createdAt = pb.UpdatedAt,
                        isResolved = false
                    })
                    .ToListAsync();

                // Lấy cảnh báo bảo trì (tạm thời comment vì chưa có MaintenanceTasks)
                var maintenanceAlerts = new List<object>();
                /*
                var maintenanceAlerts = await _context.MaintenanceTasks
                    .Where(mt => mt.Status == "Pending" && mt.ScheduledDate <= DateTime.Now.AddDays(7))
                    .Join(_context.WarehouseCells, mt => mt.CellId, wc => wc.Id, (mt, wc) => new
                    {
                        id = $"maintenance_{mt.Id}",
                        type = "Maintenance",
                        title = "Bảo trì sắp đến hạn",
                        message = $"Khu vực {wc.Name} cần bảo trì vào {mt.ScheduledDate:dd/MM/yyyy}",
                        priority = "medium",
                        taskId = mt.Id,
                        cellId = wc.Id,
                        cellName = wc.Name,
                        scheduledDate = mt.ScheduledDate,
                        daysUntilMaintenance = (int)(mt.ScheduledDate - DateTime.Now).TotalDays,
                        createdAt = mt.CreatedAt,
                        isResolved = false
                    })
                    .ToListAsync();
                */

                // Lấy cảnh báo kiểm kê
                var inventoryCheckAlerts = await _context.InventoryChecks
                    .Where(ic => ic.Status == "pending" && ic.CheckDate <= DateTime.Now.AddDays(3))
                    .Select(ic => new
                    {
                        id = $"inventory_{ic.Id}",
                        type = "InventoryCheck",
                        title = "Kiểm kê sắp đến hạn",
                        message = $"Kho cần kiểm kê vào {ic.CheckDate:dd/MM/yyyy}",
                        priority = "medium",
                        checkId = ic.Id,
                        warehouseName = "Kho", // Tạm thời hardcode
                        scheduledDate = ic.CheckDate,
                        daysUntilCheck = (int)(ic.CheckDate - DateTime.Now).TotalDays,
                        createdAt = ic.CreatedAt,
                        isResolved = false
                    })
                    .ToListAsync();

                alerts.AddRange(lowStockAlerts);
                alerts.AddRange(expiryAlerts);
                alerts.AddRange(expiredAlerts);
                alerts.AddRange(maintenanceAlerts);
                alerts.AddRange(inventoryCheckAlerts);

                // Sắp xếp theo priority và thời gian
                var sortedAlerts = alerts.OrderByDescending(a => 
                    a.GetType().GetProperty("priority").GetValue(a).ToString() == "critical" ? 4 :
                    a.GetType().GetProperty("priority").GetValue(a).ToString() == "high" ? 3 :
                    a.GetType().GetProperty("priority").GetValue(a).ToString() == "medium" ? 2 : 1)
                    .ThenByDescending(a => DateTime.Parse(a.GetType().GetProperty("createdAt").GetValue(a).ToString()))
                    .ToList();

                return Ok(new { success = true, data = sortedAlerts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy danh sách cảnh báo", error = ex.Message });
            }
        }

        // GET: api/alert/stats
        [HttpGet("stats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAlertStats()
        {
            try
            {
                var lowStockCount = await _context.ProductBatches.CountAsync(pb => pb.Quantity <= 10);
                var expiryCount = await _context.ProductBatches.CountAsync(pb => 
                    pb.ExpiryDate <= DateTime.Now.AddDays(30) && pb.ExpiryDate > DateTime.Now);
                var expiredCount = await _context.ProductBatches.CountAsync(pb => pb.ExpiryDate <= DateTime.Now);
                var maintenanceCount = 0; // Tạm thời vì chưa có MaintenanceTasks
                var inventoryCheckCount = await _context.InventoryChecks.CountAsync(ic => 
                    ic.Status == "pending" && ic.CheckDate <= DateTime.Now.AddDays(3));

                var stats = new
                {
                    total = lowStockCount + expiryCount + expiredCount + maintenanceCount + inventoryCheckCount,
                    lowStock = lowStockCount,
                    expiry = expiryCount,
                    expired = expiredCount,
                    maintenance = maintenanceCount,
                    inventoryCheck = inventoryCheckCount,
                    critical = expiredCount,
                    high = lowStockCount,
                    medium = expiryCount + maintenanceCount + inventoryCheckCount
                };

                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy thống kê cảnh báo", error = ex.Message });
            }
        }

        // POST: api/alert/resolve
        [HttpPost("resolve")]
        [AllowAnonymous]
        public async Task<IActionResult> ResolveAlert([FromBody] ResolveAlertRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AlertId))
                {
                    return BadRequest(new { success = false, message = "Alert ID không được để trống" });
                }

                // Tạo bản ghi giải quyết cảnh báo
                var alertResolution = new AlertResolution
                {
                    AlertId = request.AlertId,
                    AlertType = request.AlertType,
                    ResolvedBy = request.ResolvedBy,
                    Resolution = request.Resolution,
                    ResolvedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AlertResolutions.Add(alertResolution);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Đã giải quyết cảnh báo thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi giải quyết cảnh báo", error = ex.Message });
            }
        }

        // GET: api/alert/resolutions
        [HttpGet("resolutions")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAlertResolutions()
        {
            try
            {
                var resolutions = await _context.AlertResolutions
                    .OrderByDescending(ar => ar.ResolvedAt)
                    .Select(ar => new
                    {
                        id = ar.Id,
                        alertId = ar.AlertId,
                        alertType = ar.AlertType,
                        resolvedBy = ar.ResolvedBy,
                        resolution = ar.Resolution,
                        resolvedAt = ar.ResolvedAt,
                        createdAt = ar.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = resolutions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy danh sách giải quyết cảnh báo", error = ex.Message });
            }
        }
    }

    public class ResolveAlertRequest
    {
        public string AlertId { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty;
        public string ResolvedBy { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;
    }
}
