using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryCheckController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventoryCheckController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/inventorycheck
        [HttpGet]
        public async Task<IActionResult> GetInventoryChecks([FromQuery] int? warehouseId = null)
        {
            try
            {
                var query = _context.InventoryChecks.AsQueryable();
                
                if (warehouseId.HasValue)
                {
                    query = query.Where(i => i.WarehouseId == warehouseId.Value);
                }

                var checks = await query
                    .Select(i => new
                    {
                        id = i.Id,
                        checkNumber = i.CheckNumber,
                        description = i.Description,
                        status = i.Status,
                        checkDate = i.CheckDate,
                        notes = i.Notes,
                        warehouseId = i.WarehouseId,
                        warehouseName = i.Warehouse.Name,
                        createdBy = i.CreatedBy,
                        createdAt = i.CreatedAt,
                        completedAt = i.CompletedAt,
                        updatedAt = i.UpdatedAt,
                        updatedBy = i.UpdatedBy,
                        itemsCount = i.InventoryCheckItems.Count,
                        items = i.InventoryCheckItems.Select(item => new
                        {
                            id = item.Id,
                            productId = item.ProductId,
                            productName = item.ProductName,
                            productCode = item.ProductCode,
                            warehouseCellId = item.WarehouseCellId,
                            location = item.Location,
                            systemQuantity = item.SystemQuantity,
                            actualQuantity = item.ActualQuantity,
                            difference = item.Difference,
                            status = item.Status,
                            notes = item.Notes,
                            createdAt = item.CreatedAt
                        }).ToList()
                    })
                    .OrderByDescending(i => i.createdAt)
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    data = checks 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy danh sách kiểm kê", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/inventorycheck/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryCheck(int id)
        {
            try
            {
                var check = await _context.InventoryChecks
                    .Where(i => i.Id == id)
                    .Select(i => new
                    {
                        id = i.Id,
                        checkNumber = i.CheckNumber,
                        description = i.Description,
                        status = i.Status,
                        checkDate = i.CheckDate,
                        notes = i.Notes,
                        warehouseId = i.WarehouseId,
                        warehouseName = i.Warehouse.Name,
                        createdBy = i.CreatedBy,
                        createdAt = i.CreatedAt,
                        completedAt = i.CompletedAt,
                        updatedAt = i.UpdatedAt,
                        updatedBy = i.UpdatedBy,
                        items = i.InventoryCheckItems.Select(item => new
                        {
                            id = item.Id,
                            productId = item.ProductId,
                            productName = item.ProductName,
                            productCode = item.ProductCode,
                            warehouseCellId = item.WarehouseCellId,
                            location = item.Location,
                            systemQuantity = item.SystemQuantity,
                            actualQuantity = item.ActualQuantity,
                            difference = item.Difference,
                            status = item.Status,
                            notes = item.Notes,
                            createdAt = item.CreatedAt
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (check == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Không tìm thấy phiếu kiểm kê" 
                    });
                }

                return Ok(new { 
                    success = true, 
                    data = check 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy thông tin kiểm kê", 
                    error = ex.Message 
                });
            }
        }

        // POST: api/inventorycheck
        [HttpPost]
        public async Task<IActionResult> CreateInventoryCheck([FromBody] CreateInventoryCheckDto dto)
        {
            try
            {
                var check = new Models.Entities.InventoryCheck
                {
                    CheckNumber = dto.CheckNumber,
                    Description = dto.Description,
                    Status = "pending",
                    CheckDate = dto.CheckDate,
                    Notes = dto.Notes,
                    WarehouseId = dto.WarehouseId,
                    CreatedBy = dto.CreatedBy ?? "System",
                    CreatedAt = DateTime.UtcNow
                };

                _context.InventoryChecks.Add(check);
                await _context.SaveChangesAsync();

                // Add check items
                foreach (var itemDto in dto.Items)
                {
                    var item = new Models.Entities.InventoryCheckItem
                    {
                        InventoryCheckId = check.Id,
                        ProductId = itemDto.ProductId,
                        WarehouseCellId = itemDto.WarehouseCellId,
                        ProductCode = itemDto.ProductCode,
                        ProductName = itemDto.ProductName,
                        Location = itemDto.Location,
                        SystemQuantity = itemDto.SystemQuantity,
                        ActualQuantity = itemDto.ActualQuantity,
                        Difference = itemDto.ActualQuantity - itemDto.SystemQuantity,
                        Status = itemDto.ActualQuantity == itemDto.SystemQuantity ? "matched" : 
                                itemDto.ActualQuantity > 0 ? "mismatch" : "pending",
                        Notes = itemDto.Notes,
                        CreatedBy = check.CreatedBy,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.InventoryCheckItems.Add(item);
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "Tạo phiếu kiểm kê thành công", 
                    data = new {
                        id = check.Id,
                        checkNumber = check.CheckNumber,
                        description = check.Description,
                        status = check.Status,
                        checkDate = check.CheckDate,
                        notes = check.Notes,
                        warehouseId = check.WarehouseId,
                        createdBy = check.CreatedBy,
                        createdAt = check.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi tạo phiếu kiểm kê", 
                    error = ex.Message 
                });
            }
        }

        // PUT: api/inventorycheck/{id}/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteInventoryCheck(int id, [FromBody] CompleteInventoryCheckDto dto)
        {
            try
            {
                var check = await _context.InventoryChecks
                    .Include(i => i.InventoryCheckItems)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (check == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Không tìm thấy phiếu kiểm kê" 
                    });
                }

                check.Status = "completed";
                check.CompletedAt = DateTime.UtcNow;
                check.Notes = dto.Notes;
                check.UpdatedAt = DateTime.UtcNow;
                check.UpdatedBy = dto.UpdatedBy ?? "System";

                // Update warehouse cell products based on differences
                foreach (var item in check.InventoryCheckItems.Where(i => i.Status == "mismatch"))
                {
                    var cellProduct = await _context.WarehouseCellProducts
                        .FirstOrDefaultAsync(wcp => wcp.WarehouseCellId == item.WarehouseCellId && 
                                                  wcp.ProductId == item.ProductId);

                    if (cellProduct != null)
                    {
                        // Update quantity based on actual count
                        cellProduct.Quantity = item.ActualQuantity;
                        cellProduct.RemainingQuantity = item.ActualQuantity;
                        cellProduct.UpdatedAt = DateTime.UtcNow;
                        cellProduct.UpdatedBy = check.UpdatedBy;
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "Hoàn thành kiểm kê thành công", 
                    data = new {
                        id = check.Id,
                        checkNumber = check.CheckNumber,
                        description = check.Description,
                        status = check.Status,
                        checkDate = check.CheckDate,
                        notes = check.Notes,
                        warehouseId = check.WarehouseId,
                        createdBy = check.CreatedBy,
                        createdAt = check.CreatedAt,
                        completedAt = check.CompletedAt,
                        updatedAt = check.UpdatedAt,
                        updatedBy = check.UpdatedBy
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi hoàn thành kiểm kê", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/inventorycheck/warehouse/{warehouseId}/products
        [HttpGet("warehouse/{warehouseId}/products")]
        public async Task<IActionResult> GetWarehouseProductsForCheck(int warehouseId)
        {
            try
            {
                var products = await _context.WarehouseCellProducts
                    .Where(wcp => wcp.WarehouseCell.WarehouseId == warehouseId && wcp.Quantity > 0)
                    .Include(wcp => wcp.WarehouseCell)
                    .Include(wcp => wcp.Product)
                    .Select(wcp => new
                    {
                        ProductId = wcp.ProductId,
                        WarehouseCellId = wcp.WarehouseCellId,
                        ProductCode = wcp.Product.ProductCode,
                        ProductName = wcp.ProductName,
                        Location = wcp.WarehouseCell.CellCode,
                        SystemQuantity = wcp.Quantity
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    data = products 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy danh sách sản phẩm để kiểm kê", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/inventorycheck/warehouse/{warehouseId}/all-cells
        [HttpGet("warehouse/{warehouseId}/all-cells")]
        public async Task<IActionResult> GetWarehouseAllCells(int warehouseId)
        {
            try
            {
                // Get warehouse info with dimensions
                var warehouse = await _context.Warehouses.FindAsync(warehouseId);
                if (warehouse == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy kho" });
                }

                // Get all existing cells for the warehouse
                var existingCells = await _context.WarehouseCells
                    .Where(wc => wc.WarehouseId == warehouseId)
                    .Select(wc => new
                    {
                        Id = wc.Id,
                        CellCode = wc.CellCode,
                        Row = wc.Row,
                        Column = wc.Column,
                        WarehouseId = wc.WarehouseId
                    })
                    .ToListAsync();

                // Get all cells with products for the warehouse (from WarehouseCells table)
                var productsInCells = await _context.WarehouseCells
                    .Where(wc => wc.WarehouseId == warehouseId && wc.ProductId.HasValue)
                    .Include(wc => wc.Product)
                    .Select(wc => new
                    {
                        ProductId = wc.ProductId.Value,
                        WarehouseCellId = wc.Id,
                        ProductCode = wc.Product.ProductCode,
                        ProductName = !string.IsNullOrEmpty(wc.ProductName) ? wc.ProductName : wc.Product.ProductName,
                        Location = wc.CellCode,
                        SystemQuantity = wc.CurrentAmount,
                        BatchNumber = wc.BatchNumber,
                        ProductionDate = wc.ProductionDate,
                        ExpiryDate = wc.ExpiryDate,
                        Supplier = wc.Supplier,
                        Status = wc.Status,
                        IsEmpty = wc.CurrentAmount <= 0
                    })
                    .ToListAsync();

                var result = new List<object>();

                // Generate all possible cell positions based on warehouse dimensions
                for (int row = 1; row <= warehouse.Height; row++)
                {
                    for (int col = 1; col <= warehouse.Width; col++)
                    {
                        // Generate cell code (e.g., A01, A02, B01, B02, etc.)
                        string cellCode = $"{(char)('A' + row - 1)}{col:D2}";
                        
                        // Check if this cell exists in the database
                        var existingCell = existingCells.FirstOrDefault(ec => ec.Row == row && ec.Column == col);
                        
                        if (existingCell != null)
                        {
                            // Cell exists in database - check if it has products
                            var cellProduct = productsInCells.FirstOrDefault(p => p.WarehouseCellId == existingCell.Id);

                            if (cellProduct != null && !cellProduct.IsEmpty)
                            {
                                // Cell has products
                                result.Add(new
                                {
                                    ProductId = cellProduct.ProductId,
                                    WarehouseCellId = cellProduct.WarehouseCellId,
                                    ProductCode = cellProduct.ProductCode,
                                    ProductName = cellProduct.ProductName,
                                    Location = cellProduct.Location,
                                    SystemQuantity = cellProduct.SystemQuantity,
                                    ActualQuantity = 0, // For inventory check input
                                    Difference = 0, // Will be calculated in frontend
                                    BatchNumber = cellProduct.BatchNumber,
                                    ProductionDate = cellProduct.ProductionDate,
                                    ExpiryDate = cellProduct.ExpiryDate,
                                    Supplier = cellProduct.Supplier,
                                    Status = "pending", // Default status for inventory check
                                    Notes = "", // Empty for user input
                                    IsEmpty = false
                                });
                            }
                            else
                            {
                                // Cell exists but is empty
                                result.Add(new
                                {
                                    ProductId = (int?)null,
                                    WarehouseCellId = existingCell.Id,
                                    ProductCode = "TRỐNG",
                                    ProductName = "Vị trí trống",
                                    Location = cellCode,
                                    SystemQuantity = 0,
                                    ActualQuantity = 0,
                                    Difference = 0,
                                    BatchNumber = (string)null,
                                    ProductionDate = (DateTime?)null,
                                    ExpiryDate = (DateTime?)null,
                                    Supplier = (string)null,
                                    Status = "empty",
                                    Notes = "",
                                    IsEmpty = true
                                });
                            }
                        }
                        else
                        {
                            // Cell doesn't exist in database - it's empty
                            result.Add(new
                            {
                                ProductId = (int?)null,
                                WarehouseCellId = (int?)null,
                                ProductCode = "TRỐNG",
                                ProductName = "Vị trí trống",
                                Location = cellCode,
                                SystemQuantity = 0,
                                ActualQuantity = 0,
                                Difference = 0,
                                BatchNumber = (string)null,
                                ProductionDate = (DateTime?)null,
                                ExpiryDate = (DateTime?)null,
                                Supplier = (string)null,
                                Status = "empty",
                                Notes = "",
                                IsEmpty = true
                            });
                        }
                    }
                }

                return Ok(new { 
                    success = true, 
                    data = result,
                    warehouseInfo = new {
                        id = warehouse.Id,
                        name = warehouse.Name,
                        width = warehouse.Width,
                        height = warehouse.Height,
                        totalPositions = warehouse.TotalPositions
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy danh sách tất cả vị trí", 
                    error = ex.Message 
                });
            }
        }

        // PUT: api/inventorycheck/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateInventoryCheckStatusDto dto)
        {
            try
            {
                var check = await _context.InventoryChecks.FindAsync(id);
                if (check == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Không tìm thấy phiếu kiểm kê" 
                    });
                }

                // Validate status
                var validStatuses = new[] { "pending", "in_progress", "completed", "cancelled" };
                if (!validStatuses.Contains(dto.Status.ToLower()))
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Trạng thái không hợp lệ. Các trạng thái hợp lệ: pending, in_progress, completed, cancelled" 
                    });
                }

                // Update status
                check.Status = dto.Status.ToLower();
                check.UpdatedAt = DateTime.UtcNow;
                
                if (!string.IsNullOrEmpty(dto.Notes))
                {
                    check.Notes = dto.Notes;
                }

                // If completing, set completed date
                if (dto.Status.ToLower() == "completed")
                {
                    check.CompletedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "Cập nhật trạng thái thành công", 
                    data = new {
                        id = check.Id,
                        checkNumber = check.CheckNumber,
                        status = check.Status,
                        updatedAt = check.UpdatedAt,
                        completedAt = check.CompletedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Có lỗi xảy ra khi cập nhật trạng thái", 
                    error = ex.Message 
                });
            }
        }
    }

    // DTOs
    public class CreateInventoryCheckDto
    {
        public string CheckNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CheckDate { get; set; }
        public string? Notes { get; set; }
        public int WarehouseId { get; set; }
        public string? CreatedBy { get; set; }
        public List<CreateInventoryCheckItemDto> Items { get; set; } = new List<CreateInventoryCheckItemDto>();
    }

    public class CreateInventoryCheckItemDto
    {
        public int ProductId { get; set; }
        public int WarehouseCellId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int SystemQuantity { get; set; }
        public int ActualQuantity { get; set; }
        public string? Notes { get; set; }
    }

    public class CompleteInventoryCheckDto
    {
        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class UpdateInventoryCheckStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
