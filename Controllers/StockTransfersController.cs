using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StockTransfersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StockTransfersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all stock transfers
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<object>>> GetStockTransfers([FromQuery] string? status = null)
        {
            try
            {
                var query = _context.StockTransfers
                    .Include(st => st.FromWarehouse)
                    .Include(st => st.ToWarehouse)
                    .Include(st => st.RequestedByUser)
                    .Where(st => st.IsActive);

                if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var statusEnum))
                {
                    query = query.Where(st => st.Status == statusEnum);
                }

                var transfers = await query
                    .Select(st => new
                    {
                        st.Id,
                        st.TransferNumber,
                        st.TransferDate,
                        st.Status,
                        st.Reason,
                        FromWarehouseId = st.FromWarehouseId,
                        FromWarehouseName = st.FromWarehouse.Name,
                        ToWarehouseId = st.ToWarehouseId,
                        ToWarehouseName = st.ToWarehouse.Name,
                        RequestedById = st.RequestedBy,
                        RequestedByName = st.RequestedByUser.FullName,
                        st.ApprovedAt,
                        st.CompletedAt,
                        st.CreatedAt,
                        // Count of items
                        ItemCount = st.TransferDetails.Count(td => td.IsActive),
                        TotalValue = st.TransferDetails.Where(td => td.IsActive).Sum(td => td.Quantity * td.UnitPrice)
                    })
                    .OrderByDescending(st => st.CreatedAt)
                    .ToListAsync();

                return Ok(transfers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock transfers", error = ex.Message });
            }
        }

        /// <summary>
        /// Get stock transfer by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> GetStockTransfer(int id)
        {
            try
            {
                var transfer = await _context.StockTransfers
                    .Include(st => st.FromWarehouse)
                    .Include(st => st.ToWarehouse)
                    .Include(st => st.RequestedByUser)
                    .Include(st => st.TransferDetails)
                        .ThenInclude(td => td.Product)
                    .Where(st => st.Id == id && st.IsActive)
                    .Select(st => new
                    {
                        st.Id,
                        st.TransferNumber,
                        st.TransferDate,
                        st.Status,
                        st.Reason,
                        st.Notes,
                        FromWarehouseId = st.FromWarehouseId,
                        FromWarehouseName = st.FromWarehouse.Name,
                        ToWarehouseId = st.ToWarehouseId,
                        ToWarehouseName = st.ToWarehouse.Name,
                        RequestedById = st.RequestedBy,
                        RequestedByName = st.RequestedByUser.FullName,
                        st.ApprovedAt,
                        st.CompletedAt,
                        st.CreatedAt,
                        TransferDetails = st.TransferDetails.Where(td => td.IsActive).Select(td => new
                        {
                            td.Id,
                            td.ProductId,
                            ProductCode = td.Product.ProductCode,
                            ProductName = td.Product.ProductName,
                            ProductUnit = td.Product.Unit,
                            td.Quantity,
                            td.UnitPrice,
                            TotalPrice = td.Quantity * td.UnitPrice,
                            td.TransferredQuantity,
                            RemainingQuantity = td.Quantity - td.TransferredQuantity,
                            IsCompleted = td.TransferredQuantity >= td.Quantity,
                            td.Notes
                        })
                    })
                    .FirstOrDefaultAsync();

                if (transfer == null)
                    return NotFound(new { message = "Stock transfer not found" });

                return Ok(transfer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock transfer", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new stock transfer
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> CreateStockTransfer([FromBody] CreateStockTransferDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Validate warehouses are different
                if (createDto.FromWarehouseId == createDto.ToWarehouseId)
                    return BadRequest(new { message = "Source and destination warehouses must be different" });

                using var transaction = await _context.Database.BeginTransactionAsync();

                var transfer = new StockTransfer
                {
                    TransferNumber = createDto.TransferNumber,
                    TransferDate = createDto.TransferDate,
                    FromWarehouseId = createDto.FromWarehouseId,
                    ToWarehouseId = createDto.ToWarehouseId,
                    Status = OrderStatus.Pending,
                    Reason = createDto.Reason,
                    Notes = createDto.Notes,
                    RequestedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.StockTransfers.Add(transfer);
                await _context.SaveChangesAsync();

                // Add transfer details
                foreach (var detailDto in createDto.TransferDetails)
                {
                    var detail = new StockTransferDetail
                    {
                        StockTransferId = transfer.Id,
                        ProductId = detailDto.ProductId,
                        Quantity = detailDto.Quantity,
                        UnitPrice = detailDto.UnitPrice,
                        Notes = detailDto.Notes,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    _context.StockTransferDetails.Add(detail);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetStockTransfer), new { id = transfer.Id }, new
                {
                    transfer.Id,
                    transfer.TransferNumber,
                    transfer.TransferDate,
                    transfer.Status,
                    transfer.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating stock transfer", error = ex.Message });
            }
        }

        /// <summary>
        /// Update stock transfer status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult> UpdateTransferStatus(int id, [FromBody] UpdateTransferStatusDto statusDto)
        {
            try
            {
                var transfer = await _context.StockTransfers.FindAsync(id);
                if (transfer == null || !transfer.IsActive)
                    return NotFound(new { message = "Stock transfer not found" });

                if (Enum.TryParse<OrderStatus>(statusDto.Status, out var newStatus))
                {
                    transfer.Status = newStatus;
                }
                transfer.Notes = statusDto.Notes;
                transfer.UpdatedAt = DateTime.UtcNow;

                if (Enum.TryParse<OrderStatus>(statusDto.Status, out var status) && status == OrderStatus.Approved)
                {
                    transfer.ApprovedAt = DateTime.UtcNow;
                    transfer.ApprovedBy = GetCurrentUserId();
                }
                else if (status == OrderStatus.Completed)
                {
                    transfer.CompletedAt = DateTime.UtcNow;
                    
                    // Create stock movements for completed transfer
                    await CreateStockMovementsForTransfer(transfer.Id);
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Transfer status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating transfer status", error = ex.Message });
            }
        }

        /// <summary>
        /// Get stock transfers summary
        /// </summary>
        [HttpGet("summary")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> GetTransfersSummary()
        {
            try
            {
                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);

                var summary = new
                {
                    TotalTransfers = await _context.StockTransfers.CountAsync(st => st.IsActive),
                    PendingTransfers = await _context.StockTransfers.CountAsync(st => st.IsActive && st.Status == OrderStatus.Pending),
                    ApprovedTransfers = await _context.StockTransfers.CountAsync(st => st.IsActive && st.Status == OrderStatus.Approved),
                    CompletedTransfers = await _context.StockTransfers.CountAsync(st => st.IsActive && st.Status == OrderStatus.Completed),
                    TransfersToday = await _context.StockTransfers.CountAsync(st => st.IsActive && st.CreatedAt.Date == today),
                    TransfersThisMonth = await _context.StockTransfers.CountAsync(st => st.IsActive && st.CreatedAt >= thisMonth),
                    TransfersByStatus = await _context.StockTransfers
                        .Where(st => st.IsActive)
                        .GroupBy(st => st.Status)
                        .Select(g => new
                        {
                            Status = g.Key,
                            Count = g.Count()
                        })
                        .ToListAsync()
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving summary", error = ex.Message });
            }
        }

        private async System.Threading.Tasks.Task CreateStockMovementsForTransfer(int transferId)
        {
            var transfer = await _context.StockTransfers
                .Include(st => st.TransferDetails)
                .FirstOrDefaultAsync(st => st.Id == transferId);

            if (transfer == null) return;

            var currentUserId = GetCurrentUserId();

            foreach (var detail in transfer.TransferDetails.Where(td => td.IsActive))
            {
                // Stock Out from source warehouse
                var stockOut = new StockMovement
                {
                    MovementDate = DateTime.UtcNow,
                    MovementType = MovementType.StockOut,
                    ProductId = detail.ProductId,
                    WarehouseId = transfer.FromWarehouseId,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    ReferenceNumber = $"TRANSFER-{transfer.TransferNumber}",
                    Notes = $"Transfer to {transfer.ToWarehouse?.Name}",
                    UserId = currentUserId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // Stock In to destination warehouse
                var stockIn = new StockMovement
                {
                    MovementDate = DateTime.UtcNow,
                    MovementType = MovementType.StockIn,
                    ProductId = detail.ProductId,
                    WarehouseId = transfer.ToWarehouseId,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    ReferenceNumber = $"TRANSFER-{transfer.TransferNumber}",
                    Notes = $"Transfer from {transfer.FromWarehouse?.Name}",
                    UserId = currentUserId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.StockMovements.AddRange(stockOut, stockIn);

                // Update transferred quantity
                detail.TransferredQuantity = detail.Quantity;
                detail.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 1; // Default to admin
        }
    }

    // DTOs for Stock Transfers
    public class CreateStockTransferDto
    {
        public string TransferNumber { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; }
        public int FromWarehouseId { get; set; }
        public int ToWarehouseId { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public List<CreateStockTransferDetailDto> TransferDetails { get; set; } = new();
    }

    public class CreateStockTransferDetailDto
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateTransferStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
