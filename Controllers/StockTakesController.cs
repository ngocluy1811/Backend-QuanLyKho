using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using System.Security.Claims;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StockTakesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StockTakesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get stock takes with filtering and pagination
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<StockTakeDto>>> GetStockTakes(
            [FromQuery] int? warehouseId = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Mock data for demonstration
                var mockStockTakes = new List<StockTakeDto>
                {
                    new StockTakeDto
                    {
                        Id = 1,
                        CompanyId = 1,
                        WarehouseId = 1,
                        WarehouseName = "Kho A",
                        StockTakeNumber = "IC-2023-001",
                        StockTakeDate = DateTime.Today,
                        Description = "Kiểm kê tháng 5/2023",
                        Status = OrderStatus.Completed,
                        CreatedBy = currentUserId,
                        CreatedByName = "Current User",
                        TotalItems = 120,
                        CheckedItems = 120,
                        Discrepancies = 8,
                        StartedAt = DateTime.Today.AddHours(8),
                        CompletedAt = DateTime.Today.AddHours(17),
                        Notes = "Hoàn thành kiểm kê đúng hạn, phát hiện 8 sai lệch đã điều chỉnh.",
                        Locations = new List<string> { "Kho A", "Kho B" },
                        Categories = new List<string> { "Phân bón", "Hạt giống", "Thuốc BVTV" },
                        AssignedTo = new List<string> { "Trần Thị A", "Lê Văn B", "Phạm Văn C" },
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new StockTakeDto
                    {
                        Id = 2,
                        CompanyId = 1,
                        WarehouseId = 1,
                        WarehouseName = "Kho A",
                        StockTakeNumber = "IC-2023-002",
                        StockTakeDate = DateTime.Today,
                        Description = "Kiểm kê kho A - Phân bón",
                        Status = OrderStatus.Processing,
                        CreatedBy = currentUserId,
                        CreatedByName = "Current User",
                        TotalItems = 85,
                        CheckedItems = 42,
                        Discrepancies = 3,
                        StartedAt = DateTime.Today.AddHours(8),
                        Notes = "Đang tiến hành kiểm kê kho A, tập trung vào các sản phẩm phân bón.",
                        Locations = new List<string> { "Kho A" },
                        Categories = new List<string> { "Phân bón" },
                        AssignedTo = new List<string> { "Trần Thị A", "Hoàng Văn D" },
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new StockTakeDto
                    {
                        Id = 3,
                        CompanyId = 1,
                        WarehouseId = 2,
                        WarehouseName = "Kho B",
                        StockTakeNumber = "IC-2023-003",
                        StockTakeDate = DateTime.Today.AddDays(3),
                        Description = "Kiểm kê đột xuất - Urê",
                        Status = OrderStatus.Pending,
                        CreatedBy = currentUserId,
                        CreatedByName = "Current User",
                        TotalItems = 35,
                        CheckedItems = 0,
                        Discrepancies = 0,
                        Notes = "Kiểm kê đột xuất theo yêu cầu của giám đốc, tập trung vào sản phẩm Urê.",
                        Locations = new List<string> { "Kho A", "Kho B", "Kho C" },
                        Categories = new List<string> { "Phân bón" },
                        AssignedTo = new List<string> { "Phạm Văn C", "Nguyễn Thị E" },
                        CreatedAt = DateTime.UtcNow
                    }
                };

                // Apply filters
                if (warehouseId.HasValue)
                {
                    mockStockTakes = mockStockTakes.Where(st => st.WarehouseId == warehouseId.Value).ToList();
                }

                if (!string.IsNullOrEmpty(status))
                {
                    if (Enum.TryParse<OrderStatus>(status, true, out var statusEnum))
                    {
                        mockStockTakes = mockStockTakes.Where(st => st.Status == statusEnum).ToList();
                    }
                }

                if (startDate.HasValue)
                {
                    mockStockTakes = mockStockTakes.Where(st => st.StockTakeDate >= startDate.Value).ToList();
                }

                if (endDate.HasValue)
                {
                    mockStockTakes = mockStockTakes.Where(st => st.StockTakeDate <= endDate.Value).ToList();
                }

                // Apply pagination
                var pagedStockTakes = mockStockTakes
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(pagedStockTakes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get stock take by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<StockTakeDto>> GetStockTakeById(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Mock data with details
                var stockTake = new StockTakeDto
                {
                    Id = id,
                    CompanyId = 1,
                    WarehouseId = 1,
                    WarehouseName = "Kho A",
                    StockTakeNumber = $"IC-2023-{id:D3}",
                    StockTakeDate = DateTime.Today,
                    Description = "Kiểm kê tháng 5/2023",
                    Status = OrderStatus.Completed,
                    CreatedBy = currentUserId,
                    CreatedByName = "Current User",
                    TotalItems = 3,
                    CheckedItems = 3,
                    Discrepancies = 2,
                    StartedAt = DateTime.Today.AddHours(8),
                    CompletedAt = DateTime.Today.AddHours(17),
                    Notes = "Hoàn thành kiểm kê đúng hạn, phát hiện một số sai lệch.",
                    Locations = new List<string> { "Kho A", "Kho B" },
                    Categories = new List<string> { "Phân bón", "Hạt giống", "Thuốc BVTV" },
                    AssignedTo = new List<string> { "Trần Thị A", "Lê Văn B", "Phạm Văn C" },
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    Details = "Chi tiết kiểm kê sẽ được hiển thị ở đây"
                };

                return Ok(stockTake);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new stock take
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<StockTakeDto>> CreateStockTake([FromBody] CreateStockTakeDto createDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Verify warehouse exists
                var warehouse = await _context.Warehouses
                    .FirstOrDefaultAsync(w => w.Id == createDto.WarehouseId);

                if (warehouse == null)
                {
                    return NotFound(new { message = "Warehouse not found" });
                }

                // Validate stock take number uniqueness (mock validation)
                // In real implementation, check database for duplicate

                // Mock creation
                var stockTakeDto = new StockTakeDto
                {
                    Id = new Random().Next(1000, 9999),
                    CompanyId = 1,
                    WarehouseId = createDto.WarehouseId,
                    WarehouseName = warehouse.Name,
                    StockTakeNumber = createDto.StockTakeNumber,
                    StockTakeDate = createDto.StockTakeDate,
                    Description = createDto.Description,
                    Status = OrderStatus.Pending,
                    CreatedBy = currentUserId,
                    CreatedByName = "Current User",
                    TotalItems = 0,
                    CheckedItems = 0,
                    Discrepancies = 0,
                    Notes = createDto.Notes,
                    Locations = createDto.Locations,
                    Categories = createDto.Categories,
                    AssignedTo = createDto.AssignedTo,
                    CreatedAt = DateTime.UtcNow
                };

                return CreatedAtAction(nameof(GetStockTakeById), new { id = stockTakeDto.Id }, stockTakeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update stock take
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<StockTakeDto>> UpdateStockTake(int id, [FromBody] UpdateStockTakeDto updateDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Mock update
                var updatedStockTake = new StockTakeDto
                {
                    Id = id,
                    CompanyId = 1,
                    WarehouseId = 1,
                    WarehouseName = "Kho A",
                    StockTakeNumber = updateDto.StockTakeNumber ?? $"IC-2023-{id:D3}",
                    StockTakeDate = updateDto.StockTakeDate ?? DateTime.Today,
                    Description = updateDto.Description ?? "Updated description",
                    Status = updateDto.Status ?? OrderStatus.Pending,
                    CreatedBy = currentUserId,
                    CreatedByName = "Current User",
                    TotalItems = 0,
                    CheckedItems = 0,
                    Discrepancies = 0,
                    StartedAt = updateDto.StartedAt,
                    CompletedAt = updateDto.CompletedAt,
                    Notes = updateDto.Notes,
                    Locations = updateDto.Locations,
                    Categories = updateDto.Categories,
                    AssignedTo = updateDto.AssignedTo,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    UpdatedAt = DateTime.UtcNow
                };

                return Ok(updatedStockTake);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Start stock take
        /// </summary>
        [HttpPost("{id}/start")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<StockTakeDto>> StartStockTake(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Mock start operation
                var stockTake = new StockTakeDto
                {
                    Id = id,
                    CompanyId = 1,
                    WarehouseId = 1,
                    WarehouseName = "Kho A",
                    StockTakeNumber = $"IC-2023-{id:D3}",
                    StockTakeDate = DateTime.Today,
                    Description = "Started stock take",
                    Status = OrderStatus.Processing,
                    CreatedBy = currentUserId,
                    CreatedByName = "Current User",
                    TotalItems = 100,
                    CheckedItems = 0,
                    Discrepancies = 0,
                    StartedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    UpdatedAt = DateTime.UtcNow
                };

                return Ok(stockTake);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Complete stock take
        /// </summary>
        [HttpPost("{id}/complete")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<StockTakeDto>> CompleteStockTake(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Mock complete operation
                var stockTake = new StockTakeDto
                {
                    Id = id,
                    CompanyId = 1,
                    WarehouseId = 1,
                    WarehouseName = "Kho A",
                    StockTakeNumber = $"IC-2023-{id:D3}",
                    StockTakeDate = DateTime.Today,
                    Description = "Completed stock take",
                    Status = OrderStatus.Completed,
                    CreatedBy = currentUserId,
                    CreatedByName = "Current User",
                    TotalItems = 100,
                    CheckedItems = 100,
                    Discrepancies = 5,
                    StartedAt = DateTime.UtcNow.AddHours(-8),
                    CompletedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow
                };

                return Ok(stockTake);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete stock take
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteStockTake(int id)
        {
            try
            {
                // Mock deletion
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get stock take statistics
        /// </summary>
        [HttpGet("stats")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<StockTakeStatsDto>> GetStockTakeStats(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.Today.AddMonths(-1);
                var to = toDate ?? DateTime.Today;

                // Mock statistics
                var stats = new StockTakeStatsDto
                {
                    TotalStockTakes = 25,
                    PlannedStockTakes = 3,
                    InProgressStockTakes = 2,
                    CompletedStockTakes = 18,
                    CancelledStockTakes = 2,
                    TotalDiscrepancies = 45,
                    TotalVarianceValue = -1250000,
                    FromDate = from,
                    ToDate = to
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Export stock take report
        /// </summary>
        [HttpGet("{id}/export")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult> ExportStockTakeReport(int id, [FromQuery] string format = "xlsx")
        {
            try
            {
                // Mock file content
                var fileName = $"stock_take_report_{id}_{DateTime.Now:yyyyMMdd}.{format}";
                var content = System.Text.Encoding.UTF8.GetBytes("Mock stock take report data");

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get stock take details
        /// </summary>
        [HttpGet("{id}/details")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<StockTakeDetailDto>>> GetStockTakeDetails(int id)
        {
            try
            {
                // Mock details data
                var details = new List<StockTakeDetailDto>
                {
                    new StockTakeDetailDto
                    {
                        Id = 1,
                        StockTakeId = id,
                        ProductId = 1,
                        ProductName = "Phân NPK 16-16-8",
                        ProductCode = "NPK-16168",
                        BatchId = 1,
                        BatchNumber = "LOT-2023-001",
                        PositionId = 1,
                        PositionCode = "A01",
                        SystemQuantity = 250,
                        ActualQuantity = 248,
                        Variance = -2,
                        UnitPrice = 15000,
                        Notes = "Thiếu 2 bao",
                        CheckedAt = DateTime.Today.AddHours(10),
                        CheckedBy = "Trần Thị A"
                    }
                };

                return Ok(details);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update stock take detail
        /// </summary>
        [HttpPut("{id}/details/{detailId}")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<StockTakeDetailDto>> UpdateStockTakeDetail(
            int id, 
            int detailId, 
            [FromBody] UpdateStockTakeDetailDto updateDto)
        {
            try
            {
                // Mock update detail
                var detail = new StockTakeDetailDto
                {
                    Id = detailId,
                    StockTakeId = id,
                    ProductId = 1,
                    ProductName = "Updated Product",
                    ProductCode = "UPD-001",
                    BatchId = 1,
                    BatchNumber = "LOT-2023-001",
                    PositionId = 1,
                    PositionCode = "A01",
                    SystemQuantity = 250,
                    ActualQuantity = updateDto.ActualQuantity ?? 248,
                    Variance = (updateDto.ActualQuantity ?? 248) - 250,
                    UnitPrice = updateDto.UnitPrice ?? 15000,
                    Notes = updateDto.Notes,
                    CheckedAt = DateTime.UtcNow,
                    CheckedBy = "Current User"
                };

                return Ok(detail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}