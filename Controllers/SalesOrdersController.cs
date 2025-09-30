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
    public class SalesOrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalesOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all sales orders
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "SalesStaff")]
        public async Task<ActionResult<IEnumerable<object>>> GetSalesOrders([FromQuery] string? status = null)
        {
            try
            {
                var query = _context.SalesOrders
                    .Include(so => so.Customer)
                    .Include(so => so.Warehouse)
                    .Where(so => so.IsActive);

                if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
                {
                    query = query.Where(so => so.Status == orderStatus);
                }

                var orders = await query
                    .Select(so => new
                    {
                        so.Id,
                        so.OrderNumber,
                        so.OrderDate,
                        so.DeliveryDate,
                        so.RequestedDeliveryDate,
                        so.Status,
                        StatusName = so.Status.ToString(),
                        so.TotalAmount,
                        so.TaxAmount,
                        so.DiscountAmount,
                        NetAmount = so.TotalAmount + so.TaxAmount - so.DiscountAmount,
                        so.Notes,
                        CustomerId = so.CustomerId,
                        CustomerName = so.Customer.CustomerName,
                        WarehouseId = so.WarehouseId,
                        WarehouseName = so.Warehouse.Name,
                        so.CreatedAt,
                        so.IsActive
                    })
                    .OrderByDescending(so => so.CreatedAt)
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving sales orders", error = ex.Message });
            }
        }

        /// <summary>
        /// Get sales order by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "SalesStaff")]
        public async Task<ActionResult<object>> GetSalesOrder(int id)
        {
            try
            {
                var order = await _context.SalesOrders
                    .Include(so => so.Customer)
                    .Include(so => so.Warehouse)
                    .Include(so => so.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .Where(so => so.Id == id && so.IsActive)
                    .Select(so => new
                    {
                        so.Id,
                        so.OrderNumber,
                        so.OrderDate,
                        so.DeliveryDate,
                        so.RequestedDeliveryDate,
                        so.Status,
                        StatusName = so.Status.ToString(),
                        so.TotalAmount,
                        so.TaxAmount,
                        so.DiscountAmount,
                        NetAmount = so.TotalAmount + so.TaxAmount - so.DiscountAmount,
                        so.Notes,
                        CustomerId = so.CustomerId,
                        CustomerName = so.Customer.CustomerName,
                        CustomerContact = so.Customer.Phone,
                        WarehouseId = so.WarehouseId,
                        WarehouseName = so.Warehouse.Name,
                        so.CreatedAt,
                        so.ApprovedAt,
                        OrderDetails = so.OrderDetails.Where(od => od.IsActive).Select(od => new
                        {
                            od.Id,
                            od.ProductId,
                            ProductCode = od.Product.ProductCode,
                            ProductName = od.Product.ProductName,
                            ProductUnit = od.Product.Unit,
                            od.Quantity,
                            od.UnitPrice,
                            TotalPrice = od.Quantity * od.UnitPrice,
                            od.ShippedQuantity,
                            RemainingQuantity = od.Quantity - od.ShippedQuantity,
                            IsCompleted = od.ShippedQuantity >= od.Quantity,
                            od.Notes
                        })
                    })
                    .FirstOrDefaultAsync();

                if (order == null)
                    return NotFound(new { message = "Sales order not found" });

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving sales order", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new sales order
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "SalesStaff")]
        public async Task<ActionResult<object>> CreateSalesOrder([FromBody] CreateSalesOrderBasicDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                using var transaction = await _context.Database.BeginTransactionAsync();

                var order = new SalesOrder
                {
                    OrderNumber = createDto.OrderNumber,
                    OrderDate = createDto.OrderDate,
                    RequestedDeliveryDate = createDto.RequestedDeliveryDate,
                    CustomerId = createDto.CustomerId,
                    WarehouseId = createDto.WarehouseId,
                    Status = OrderStatus.Pending,
                    TaxAmount = createDto.TaxAmount,
                    DiscountAmount = createDto.DiscountAmount,
                    Notes = createDto.Notes,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = GetCurrentUserId(),
                    IsActive = true
                };

                _context.SalesOrders.Add(order);
                await _context.SaveChangesAsync();

                decimal totalAmount = 0;
                foreach (var detailDto in createDto.OrderDetails)
                {
                    var detail = new SalesOrderDetail
                    {
                        SalesOrderId = order.Id,
                        ProductId = detailDto.ProductId,
                        Quantity = detailDto.Quantity,
                        UnitPrice = detailDto.UnitPrice,
                        Notes = detailDto.Notes,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    totalAmount += detailDto.Quantity * detailDto.UnitPrice;
                    _context.SalesOrderDetails.Add(detail);
                }

                order.TotalAmount = totalAmount;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetSalesOrder), new { id = order.Id }, new
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
                return StatusCode(500, new { message = "An error occurred while creating sales order", error = ex.Message });
            }
        }

        /// <summary>
        /// Update sales order status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Policy = "SalesStaff")]
        public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusBasicDto statusDto)
        {
            try
            {
                var order = await _context.SalesOrders.FindAsync(id);
                if (order == null || !order.IsActive)
                    return NotFound(new { message = "Sales order not found" });

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
        /// Get sales orders summary
        /// </summary>
        [HttpGet("summary")]
        [Authorize(Policy = "SalesStaff")]
        public async Task<ActionResult<object>> GetOrdersSummary()
        {
            try
            {
                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);

                var summary = new
                {
                    TotalOrders = await _context.SalesOrders.CountAsync(so => so.IsActive),
                    PendingOrders = await _context.SalesOrders.CountAsync(so => so.IsActive && so.Status == OrderStatus.Pending),
                    ApprovedOrders = await _context.SalesOrders.CountAsync(so => so.IsActive && so.Status == OrderStatus.Approved),
                    CompletedOrders = await _context.SalesOrders.CountAsync(so => so.IsActive && so.Status == OrderStatus.Completed),
                    OrdersToday = await _context.SalesOrders.CountAsync(so => so.IsActive && so.CreatedAt.Date == today),
                    OrdersThisMonth = await _context.SalesOrders.CountAsync(so => so.IsActive && so.CreatedAt >= thisMonth),
                    TotalValueThisMonth = await _context.SalesOrders
                        .Where(so => so.IsActive && so.CreatedAt >= thisMonth)
                        .SumAsync(so => so.TotalAmount),
                    AverageOrderValue = await _context.SalesOrders
                        .Where(so => so.IsActive)
                        .AverageAsync(so => (double?)so.TotalAmount) ?? 0
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

    // Basic DTOs for Sales Orders
    public class CreateSalesOrderBasicDto
    {
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? RequestedDeliveryDate { get; set; }
        public int CustomerId { get; set; }
        public int WarehouseId { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? Notes { get; set; }
        public List<CreateSalesOrderDetailBasicDto> OrderDetails { get; set; } = new();
    }

    public class CreateSalesOrderDetailBasicDto
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Notes { get; set; }
    }
}
