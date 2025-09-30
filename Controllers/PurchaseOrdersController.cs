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
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all purchase orders
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<object>>> GetPurchaseOrders([FromQuery] string? status = null)
        {
            try
            {
                var query = _context.PurchaseOrders
                    .Include(po => po.Supplier)
                    .Include(po => po.Warehouse)
                    .Where(po => po.IsActive);

                if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
                {
                    query = query.Where(po => po.Status == orderStatus);
                }

                var orders = await query
                    .Select(po => new
                    {
                        po.Id,
                        po.OrderNumber,
                        po.OrderDate,
                        po.DeliveryDate,
                        po.ExpectedDeliveryDate,
                        po.Status,
                        StatusName = po.Status.ToString(),
                        po.TotalAmount,
                        po.TaxAmount,
                        po.DiscountAmount,
                        NetAmount = po.TotalAmount + po.TaxAmount - po.DiscountAmount,
                        po.Notes,
                        SupplierId = po.SupplierId,
                        SupplierName = po.Supplier.SupplierName,
                        WarehouseId = po.WarehouseId,
                        WarehouseName = po.Warehouse.Name,
                        po.CreatedAt,
                        po.IsActive
                    })
                    .OrderByDescending(po => po.CreatedAt)
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving purchase orders", error = ex.Message });
            }
        }

        /// <summary>
        /// Get purchase order by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> GetPurchaseOrder(int id)
        {
            try
            {
                var order = await _context.PurchaseOrders
                    .Include(po => po.Supplier)
                    .Include(po => po.Warehouse)
                    .Include(po => po.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .Where(po => po.Id == id && po.IsActive)
                    .Select(po => new
                    {
                        po.Id,
                        po.OrderNumber,
                        po.OrderDate,
                        po.DeliveryDate,
                        po.ExpectedDeliveryDate,
                        po.Status,
                        StatusName = po.Status.ToString(),
                        po.TotalAmount,
                        po.TaxAmount,
                        po.DiscountAmount,
                        NetAmount = po.TotalAmount + po.TaxAmount - po.DiscountAmount,
                        po.Notes,
                        SupplierId = po.SupplierId,
                        SupplierName = po.Supplier.SupplierName,
                        SupplierContact = po.Supplier.Phone,
                        WarehouseId = po.WarehouseId,
                        WarehouseName = po.Warehouse.Name,
                        po.CreatedAt,
                        po.ApprovedAt,
                        OrderDetails = po.OrderDetails.Where(od => od.IsActive).Select(od => new
                        {
                            od.Id,
                            od.ProductId,
                            ProductCode = od.Product.ProductCode,
                            ProductName = od.Product.ProductName,
                            ProductUnit = od.Product.Unit,
                            od.Quantity,
                            od.UnitPrice,
                            TotalPrice = od.Quantity * od.UnitPrice,
                            od.ReceivedQuantity,
                            RemainingQuantity = od.Quantity - od.ReceivedQuantity,
                            IsCompleted = od.ReceivedQuantity >= od.Quantity,
                            od.Notes
                        })
                    })
                    .FirstOrDefaultAsync();

                if (order == null)
                    return NotFound(new { message = "Purchase order not found" });

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving purchase order", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new purchase order
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<object>> CreatePurchaseOrder([FromBody] CreatePurchaseOrderBasicDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                using var transaction = await _context.Database.BeginTransactionAsync();

                var order = new PurchaseOrder
                {
                    OrderNumber = createDto.OrderNumber,
                    OrderDate = createDto.OrderDate,
                    ExpectedDeliveryDate = createDto.ExpectedDeliveryDate,
                    SupplierId = createDto.SupplierId,
                    WarehouseId = createDto.WarehouseId,
                    Status = OrderStatus.Pending,
                    TaxAmount = createDto.TaxAmount,
                    DiscountAmount = createDto.DiscountAmount,
                    Notes = createDto.Notes,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = GetCurrentUserId(),
                    IsActive = true
                };

                _context.PurchaseOrders.Add(order);
                await _context.SaveChangesAsync();

                decimal totalAmount = 0;
                foreach (var detailDto in createDto.OrderDetails)
                {
                    var detail = new PurchaseOrderDetail
                    {
                        PurchaseOrderId = order.Id,
                        ProductId = detailDto.ProductId,
                        Quantity = detailDto.Quantity,
                        UnitPrice = detailDto.UnitPrice,
                        Notes = detailDto.Notes,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    totalAmount += detailDto.Quantity * detailDto.UnitPrice;
                    _context.PurchaseOrderDetails.Add(detail);
                }

                order.TotalAmount = totalAmount;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetPurchaseOrder), new { id = order.Id }, new
                {
                    order.Id,
                    order.OrderNumber,
                    order.OrderDate,
                    order.Status,
                    order.TotalAmount,
                    order.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating purchase order", error = ex.Message });
            }
        }

        /// <summary>
        /// Update purchase order status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusBasicDto statusDto)
        {
            try
            {
                var order = await _context.PurchaseOrders.FindAsync(id);
                if (order == null || !order.IsActive)
                    return NotFound(new { message = "Purchase order not found" });

                order.Status = statusDto.Status;
                order.Notes = statusDto.Notes;
                order.UpdatedAt = DateTime.UtcNow;

                if (statusDto.Status == OrderStatus.Approved)
                {
                    order.ApprovedAt = DateTime.UtcNow;
                    order.ApprovedBy = GetCurrentUserId();
                }
                else if (statusDto.Status == OrderStatus.Delivered)
                {
                    order.DeliveryDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating order status", error = ex.Message });
            }
        }

        /// <summary>
        /// Get purchase orders summary
        /// </summary>
        [HttpGet("summary")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> GetOrdersSummary()
        {
            try
            {
                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);

                var summary = new
                {
                    TotalOrders = await _context.PurchaseOrders.CountAsync(po => po.IsActive),
                    PendingOrders = await _context.PurchaseOrders.CountAsync(po => po.IsActive && po.Status == OrderStatus.Pending),
                    ApprovedOrders = await _context.PurchaseOrders.CountAsync(po => po.IsActive && po.Status == OrderStatus.Approved),
                    CompletedOrders = await _context.PurchaseOrders.CountAsync(po => po.IsActive && po.Status == OrderStatus.Completed),
                    OrdersToday = await _context.PurchaseOrders.CountAsync(po => po.IsActive && po.CreatedAt.Date == today),
                    OrdersThisMonth = await _context.PurchaseOrders.CountAsync(po => po.IsActive && po.CreatedAt >= thisMonth),
                    TotalValueThisMonth = await _context.PurchaseOrders
                        .Where(po => po.IsActive && po.CreatedAt >= thisMonth)
                        .SumAsync(po => po.TotalAmount),
                    AverageOrderValue = await _context.PurchaseOrders
                        .Where(po => po.IsActive)
                        .AverageAsync(po => (double?)po.TotalAmount) ?? 0
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving summary", error = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 1; // Default to admin
        }
    }

    // Basic DTOs for Purchase Orders
    public class CreatePurchaseOrderBasicDto
    {
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? Notes { get; set; }
        public List<CreatePurchaseOrderDetailBasicDto> OrderDetails { get; set; } = new();
    }

    public class CreatePurchaseOrderDetailBasicDto
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateOrderStatusBasicDto
    {
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}
