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
    public class StockMovementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StockMovementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all stock movements with filtering
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<object>>> GetStockMovements(
            [FromQuery] string? movementType = null,
            [FromQuery] int? warehouseId = null,
            [FromQuery] int? productId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var query = _context.StockMovements
                    .Include(sm => sm.Product)
                    .Include(sm => sm.Warehouse)
                    .Include(sm => sm.User)
                    .Where(sm => sm.IsActive);

                // Apply filters
                if (!string.IsNullOrEmpty(movementType) && Enum.TryParse<MovementType>(movementType, out var moveType))
                {
                    query = query.Where(sm => sm.MovementType == moveType);
                }

                if (warehouseId.HasValue)
                {
                    query = query.Where(sm => sm.WarehouseId == warehouseId.Value);
                }

                if (productId.HasValue)
                {
                    query = query.Where(sm => sm.ProductId == productId.Value);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(sm => sm.MovementDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(sm => sm.MovementDate <= toDate.Value);
                }

                var movements = await query
                    .Select(sm => new
                    {
                        sm.Id,
                        sm.MovementDate,
                        sm.MovementType,
                        MovementTypeName = sm.MovementType.ToString(),
                        sm.Quantity,
                        sm.UnitPrice,
                        TotalValue = sm.Quantity * sm.UnitPrice,
                        sm.ReferenceNumber,
                        sm.Notes,
                        ProductId = sm.ProductId,
                        ProductName = sm.Product.ProductName,
                        ProductCode = sm.Product.ProductCode,
                        WarehouseId = sm.WarehouseId,
                        WarehouseName = sm.Warehouse.Name,
                        UserId = sm.UserId,
                        UserName = sm.User.FullName,
                        sm.CreatedAt
                    })
                    .OrderByDescending(sm => sm.MovementDate)
                    .ToListAsync();

                return Ok(movements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock movements", error = ex.Message });
            }
        }

        /// <summary>
        /// Get stock movement by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> GetStockMovement(int id)
        {
            try
            {
                var movement = await _context.StockMovements
                    .Include(sm => sm.Product)
                    .Include(sm => sm.Warehouse)
                    .Include(sm => sm.User)
                    .Where(sm => sm.Id == id && sm.IsActive)
                    .Select(sm => new
                    {
                        sm.Id,
                        sm.MovementDate,
                        sm.MovementType,
                        MovementTypeName = sm.MovementType.ToString(),
                        sm.Quantity,
                        sm.UnitPrice,
                        TotalValue = sm.Quantity * sm.UnitPrice,
                        sm.ReferenceNumber,
                        sm.Notes,
                        ProductId = sm.ProductId,
                        ProductName = sm.Product.ProductName,
                        ProductCode = sm.Product.ProductCode,
                        ProductUnit = sm.Product.Unit,
                        WarehouseId = sm.WarehouseId,
                        WarehouseName = sm.Warehouse.Name,
                        UserId = sm.UserId,
                        UserName = sm.User.FullName,
                        sm.CreatedAt,
                        sm.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (movement == null)
                    return NotFound(new { message = "Stock movement not found" });

                return Ok(movement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock movement", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new stock movement (Stock In/Out)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> CreateStockMovement([FromBody] CreateStockMovementDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var movement = new StockMovement
                {
                    MovementDate = createDto.MovementDate,
                    MovementType = createDto.MovementType,
                    ProductId = createDto.ProductId,
                    WarehouseId = createDto.WarehouseId,
                    Quantity = createDto.Quantity,
                    UnitPrice = createDto.UnitPrice,
                    ReferenceNumber = createDto.ReferenceNumber,
                    Notes = createDto.Notes,
                    UserId = GetCurrentUserId(),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.StockMovements.Add(movement);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetStockMovement), new { id = movement.Id }, new
                {
                    movement.Id,
                    movement.MovementDate,
                    movement.MovementType,
                    movement.Quantity,
                    movement.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating stock movement", error = ex.Message });
            }
        }

        /// <summary>
        /// Get stock movements summary
        /// </summary>
        [HttpGet("summary")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> GetMovementsSummary()
        {
            try
            {
                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);

                var summary = new
                {
                    TotalMovements = await _context.StockMovements.CountAsync(sm => sm.IsActive),
                    MovementsToday = await _context.StockMovements.CountAsync(sm => sm.IsActive && sm.MovementDate.Date == today),
                    MovementsThisMonth = await _context.StockMovements.CountAsync(sm => sm.IsActive && sm.MovementDate >= thisMonth),
                    StockInToday = await _context.StockMovements.CountAsync(sm => sm.IsActive && 
                                                                               sm.MovementDate.Date == today && 
                                                                               sm.MovementType == MovementType.StockIn),
                    StockOutToday = await _context.StockMovements.CountAsync(sm => sm.IsActive && 
                                                                                sm.MovementDate.Date == today && 
                                                                                sm.MovementType == MovementType.StockOut),
                    TotalValueToday = await _context.StockMovements
                        .Where(sm => sm.IsActive && sm.MovementDate.Date == today)
                        .SumAsync(sm => sm.Quantity * sm.UnitPrice),
                    TotalValueThisMonth = await _context.StockMovements
                        .Where(sm => sm.IsActive && sm.MovementDate >= thisMonth)
                        .SumAsync(sm => sm.Quantity * sm.UnitPrice),
                    MovementsByType = await _context.StockMovements
                        .Where(sm => sm.IsActive && sm.MovementDate >= thisMonth)
                        .GroupBy(sm => sm.MovementType)
                        .Select(g => new
                        {
                            MovementType = g.Key.ToString(),
                            Count = g.Count(),
                            TotalQuantity = g.Sum(sm => sm.Quantity),
                            TotalValue = g.Sum(sm => sm.Quantity * sm.UnitPrice)
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

        /// <summary>
        /// Get stock movements by product
        /// </summary>
        [HttpGet("by-product/{productId}")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<object>>> GetMovementsByProduct(int productId)
        {
            try
            {
                var movements = await _context.StockMovements
                    .Include(sm => sm.Warehouse)
                    .Include(sm => sm.User)
                    .Where(sm => sm.ProductId == productId && sm.IsActive)
                    .Select(sm => new
                    {
                        sm.Id,
                        sm.MovementDate,
                        sm.MovementType,
                        MovementTypeName = sm.MovementType.ToString(),
                        sm.Quantity,
                        sm.UnitPrice,
                        sm.ReferenceNumber,
                        WarehouseName = sm.Warehouse.Name,
                        UserName = sm.User.FullName,
                        sm.Notes
                    })
                    .OrderByDescending(sm => sm.MovementDate)
                    .ToListAsync();

                return Ok(movements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving movements by product", error = ex.Message });
            }
        }

        /// <summary>
        /// Get current stock levels by warehouse
        /// </summary>
        [HttpGet("stock-levels")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<object>>> GetStockLevels([FromQuery] int? warehouseId = null)
        {
            try
            {
                var query = _context.StockMovements
                    .Include(sm => sm.Product)
                    .Include(sm => sm.Warehouse)
                    .Where(sm => sm.IsActive);

                if (warehouseId.HasValue)
                {
                    query = query.Where(sm => sm.WarehouseId == warehouseId.Value);
                }

                var stockLevels = await query
                    .GroupBy(sm => new { sm.ProductId, sm.WarehouseId })
                    .Select(g => new
                    {
                        ProductId = g.Key.ProductId,
                        ProductName = g.First().Product.ProductName,
                        ProductCode = g.First().Product.ProductCode,
                        WarehouseId = g.Key.WarehouseId,
                        WarehouseName = g.First().Warehouse.Name,
                        StockIn = g.Where(sm => sm.MovementType == MovementType.StockIn).Sum(sm => sm.Quantity),
                        StockOut = g.Where(sm => sm.MovementType == MovementType.StockOut).Sum(sm => sm.Quantity),
                        CurrentStock = g.Where(sm => sm.MovementType == MovementType.StockIn).Sum(sm => sm.Quantity) -
                                      g.Where(sm => sm.MovementType == MovementType.StockOut).Sum(sm => sm.Quantity),
                        LastMovementDate = g.Max(sm => sm.MovementDate),
                        AverageUnitPrice = g.Average(sm => sm.UnitPrice)
                    })
                    .Where(sl => sl.CurrentStock != 0) // Only show items with stock
                    .OrderBy(sl => sl.WarehouseName)
                    .ThenBy(sl => sl.ProductName)
                    .ToListAsync();

                return Ok(stockLevels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock levels", error = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 1; // Default to admin
        }
    }

    // DTOs for Stock Movements
    public class CreateStockMovementDto
    {
        public DateTime MovementDate { get; set; }
        public MovementType MovementType { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
    }
}
