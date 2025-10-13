using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Temporarily disabled for testing
    public class WarehousesController : ControllerBase
    {
    private readonly ApplicationDbContext _context;

    public WarehousesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Helper method to recalculate ProductBatch quantity
    private async System.Threading.Tasks.Task RecalculateProductBatchQuantity(int productBatchId)
    {
        var productBatch = await _context.ProductBatches.FindAsync(productBatchId);
        if (productBatch != null)
        {
            // Calculate actual quantity based on import orders
            var totalImportedQuantity = await _context.ImportOrderDetails
                .Where(iod => iod.ProductBatchId == productBatchId)
                .SumAsync(iod => iod.Quantity);
            
            // Calculate total exported quantity from warehouse activities
            var totalExportedQuantity = await _context.WarehouseActivities
                .Where(wa => wa.BatchNumber == productBatch.BatchNumber && wa.ActivityType == "Export")
                .SumAsync(wa => wa.Quantity);
            
            // Calculate total cleared quantity from warehouse activities
            var totalClearedQuantity = await _context.WarehouseActivities
                .Where(wa => wa.BatchNumber == productBatch.BatchNumber && wa.ActivityType == "ClearProduct")
                .SumAsync(wa => wa.Quantity);
            
            // Update current quantity = imported - exported - cleared
            var calculatedQuantity = (int)(totalImportedQuantity - totalExportedQuantity - totalClearedQuantity);
            if (calculatedQuantity < 0) calculatedQuantity = 0;
            
            // Only update CurrentQuantity, keep original Quantity unchanged
            productBatch.CurrentQuantity = calculatedQuantity;
            productBatch.UpdatedAt = DateTime.UtcNow;
            
            // Log the calculation
            Console.WriteLine($"Recalculated ProductBatch {productBatch.BatchNumber}: Imported={totalImportedQuantity}, Exported={totalExportedQuantity}, Cleared={totalClearedQuantity}, Current={calculatedQuantity}");
        }
    }

        /// <summary>
        /// Get all warehouses
        /// </summary>
        [HttpGet]
        // [Authorize(Policy = "Warehouse")] // Temporarily disabled for testing
        public async Task<ActionResult<IEnumerable<object>>> GetWarehouses()
        {
            try
            {

                var warehouses = await _context.Warehouses
                    .Where(w => w.IsActive)
                    .Select(w => new
                    {
                        w.Id,
                        Name = w.Name,
                        w.Address,
                        w.Size,
                        w.Width,
                        w.Height,
                        w.TotalPositions,
                        w.Status,
                        w.IsActive,
                        w.CreatedAt,
                        CompanyName = w.Company.CompanyName
                    })
                    .ToListAsync();

                return Ok(warehouses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving warehouses", error = ex.Message });
            }
        }

        /// <summary>
        /// Get warehouse by ID
        /// </summary>
        [HttpGet("{id}")]
        // [Authorize(Policy = "Warehouse")] // Temporarily disabled for testing
        public async Task<ActionResult<object>> GetWarehouse(int id)
        {
            try
            {
                var warehouse = await _context.Warehouses
                    .Include(w => w.Company)
                    .Where(w => w.Id == id && w.IsActive)
                    .Select(w => new
                    {
                        w.Id,
                        Name = w.Name,
                        w.Address,
                        w.Size,
                        w.Width,
                        w.Height,
                        w.TotalPositions,
                        w.CompanyId,
                        CompanyName = w.Company.CompanyName,
                        w.Status,
                        w.IsActive,
                        w.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (warehouse == null)
                    return NotFound(new { message = "Warehouse not found" });

                return Ok(warehouse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving warehouse", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new warehouse
        /// </summary>
        [HttpPost]
        // [Authorize(Policy = "Warehouse")] // Temporarily disabled for testing
        public async Task<ActionResult<object>> CreateWarehouse([FromBody] CreateWarehouseRequest request)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest(new { message = "Warehouse name is required" });
                }

                if (request.Width <= 0 || request.Height <= 0)
                {
                    return BadRequest(new { message = "Width and Height must be greater than 0" });
                }

                // Check if warehouse name already exists
                var existingWarehouse = await _context.Warehouses
                    .FirstOrDefaultAsync(w => w.Name == request.Name && w.IsActive);
                
                if (existingWarehouse != null)
                {
                    return BadRequest(new { message = "Warehouse name already exists" });
                }

                var warehouse = new Warehouse
                {
                    Name = request.Name.Trim(),
                    Description = request.Description?.Trim() ?? string.Empty,
                    Address = request.Address?.Trim() ?? string.Empty,
                    Width = request.Width,
                    Height = request.Height,
                    Status = "Active",
                    CompanyId = null, // Allow null as per database schema
                    ManagerId = null, // Allow null as per database schema
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Warehouses.Add(warehouse);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    warehouse.Id,
                    warehouse.Name,
                    warehouse.Address,
                    warehouse.Size,
                    warehouse.Width,
                    warehouse.Height,
                    warehouse.TotalPositions,
                    warehouse.Status,
                    warehouse.IsActive,
                    warehouse.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating warehouse", error = ex.Message });
            }
        }

        /// <summary>
        /// Get warehouse summary
        /// </summary>
        [HttpGet("{id}/summary")]
        // [Authorize(Policy = "Warehouse")] // Temporarily disabled for testing
        public async Task<ActionResult<object>> GetWarehouseSummary(int id)
        {
            try
            {
                var totalPositions = await _context.WarehousePositions
                    .Where(p => p.WarehouseId == id)
                    .CountAsync();

                var summary = new
                {
                    TotalPositions = totalPositions,
                    AvailablePositions = totalPositions,
                    UtilizationPercentage = 0.0
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving summary", error = ex.Message });
            }
        }

        /// <summary>
        /// Get warehouse positions (cells)
        /// </summary>
        [HttpGet("{warehouseId}/positions")]
        // [Authorize(Policy = "Warehouse")] // Temporarily disabled for testing
        public async Task<ActionResult<IEnumerable<object>>> GetWarehousePositions(int warehouseId)
        {
            try
            {
                var positions = await _context.WarehouseCells
                    .Where(c => c.WarehouseId == warehouseId && c.IsActive)
                    .Select(c => new
                    {
                        c.Id,
                        c.Row,
                        c.Column,
                        c.CellCode,
                        c.CellType,
                        c.MaxCapacity,
                        c.CurrentAmount,
                        c.ProductName,
                        c.BatchNumber,
                        c.ProductionDate,
                        c.ExpiryDate,
                        c.Supplier,
                        c.Status,
                        c.LastMoved,
                        c.ClusterName,
                        c.AssignedStaff
                    })
                    .ToListAsync();

                return Ok(positions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving warehouse positions", error = ex.Message });
            }
        }

        /// <summary>
        /// Get warehouse clusters
        /// </summary>
        [HttpGet("{warehouseId}/clusters")]
        // [Authorize(Policy = "Warehouse")] // Temporarily disabled for testing
        public async Task<ActionResult<IEnumerable<object>>> GetWarehouseClusters(int warehouseId)
        {
            try
            {
                var clusters = await _context.WarehouseClusters
                    .Where(c => c.WarehouseId == warehouseId && c.IsActive)
                    .Select(c => new
                    {
                        c.Id,
                        c.ClusterName,
                        c.ClusterType,
                        c.Color,
                        c.Status,
                        c.IsActive,
                        c.CreatedAt
                    })
                    .ToListAsync();

                return Ok(clusters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving warehouse clusters", error = ex.Message });
            }
        }

        /// <summary>
        /// Get warehouse statistics
        /// </summary>
        [HttpGet("{id}/statistics")]
        // [Authorize(Policy = "Warehouse")] // Temporarily disabled for testing
        public async Task<ActionResult<object>> GetWarehouseStatistics(int id)
        {
            try
            {
                var positions = await _context.WarehousePositions
                    .Where(p => p.WarehouseId == id)
                    .Select(p => new
                    {
                        p.Id,
                        p.CurrentCapacity,
                        p.MaxCapacity,
                        p.Status
                    })
                    .ToListAsync();

                var totalPositions = positions.Count;
                var positionsWithGoods = positions.Where(p => p.CurrentCapacity > 0).ToList();
                var emptyPositions = totalPositions - positionsWithGoods.Count;

                var warnings = positionsWithGoods
                    .Where(p => p.CurrentCapacity >= p.MaxCapacity * 0.9m)
                    .Select(p => new
                    {
                        PositionId = p.Id,
                        CurrentCapacity = p.CurrentCapacity,
                        MaxCapacity = p.MaxCapacity,
                        UtilizationPercentage = (double)p.CurrentCapacity / (double)p.MaxCapacity * 100.0
                    })
                    .ToList();

                return Ok(new
                {
                    totalPositions,
                    positionsWithGoods = positionsWithGoods.Count,
                    emptyPositions,
                    warnings,
                    emptyPositionsCount = totalPositions - positionsWithGoods.Count
                });
            }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving warehouse statistics", error = ex.Message });
        }
    }

        /// <summary>
        /// Update warehouse size
        /// </summary>
        [HttpPut("{id}/size")]
        // [Authorize(Policy = "Warehouse")] // Temporarily disabled for testing
        public async Task<ActionResult<object>> UpdateWarehouseSize(int id, [FromBody] UpdateWarehouseSizeRequest request)
        {
            try
            {
                var warehouse = await _context.Warehouses.FindAsync(id);
                if (warehouse == null)
                {
                    return NotFound(new { message = "Warehouse not found" });
                }

                // Get or create default zone
                var zone = await _context.WarehouseZones
                    .FirstOrDefaultAsync(z => z.WarehouseId == id && z.ZoneName == "Default Zone");

                if (zone == null)
                {
                    zone = new WarehouseZone
                    {
                        WarehouseId = id,
                        ZoneName = "Default Zone",
                        // Description = "Default zone for all positions", // Property not available
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.WarehouseZones.Add(zone);
                    await _context.SaveChangesAsync();
                }

                // Update warehouse size
                warehouse.Width = request.Width;
                warehouse.Height = request.Height;
                // warehouse.TotalPositions = request.Width * request.Height; // Read-only property
                // warehouse.Size = $"{request.Width}x{request.Height}"; // Read-only property
                warehouse.UpdatedAt = DateTime.UtcNow;

                // Get existing cells
                var existingCells = await _context.WarehouseCells
                    .Where(c => c.WarehouseId == id)
                    .ToListAsync();

                var newCells = new List<WarehouseCell>();

                // Create or update cells
                for (int row = 1; row <= request.Height; row++)
                {
                    for (int col = 1; col <= request.Width; col++)
                    {
                        var cellCode = $"{(char)(64 + row)}{col:D2}";
                        var existingCell = existingCells.FirstOrDefault(c => c.Row == row && c.Column == col);

                        if (existingCell != null)
                        {
                            existingCell.CellCode = cellCode;
                            existingCell.UpdatedAt = DateTime.UtcNow;
                        }
                        else
                        {
                            var newCell = new WarehouseCell
                            {
                                WarehouseId = id,
                                Row = row,
                                Column = col,
                                CellCode = cellCode,
                                CellType = "Shelf",
                                MaxCapacity = 1000,
                                CurrentAmount = 0,
                                Status = "Empty",
                                ZoneId = zone.Id,
                                ClusterName = "Khu vực A",
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow
                            };
                            newCells.Add(newCell);
                        }
                    }
                }

                // Check cells that will be removed due to size reduction
                var cellsToRemove = existingCells
                    .Where(c => c.Row > request.Height || c.Column > request.Width)
                    .ToList();

                // Check if any cells to be removed have goods
                var positionsWithGoods = cellsToRemove
                    .Where(c => c.CurrentAmount > 0)
                    .Select(c => new
                    {
                        cellCode = c.CellCode,
                        row = c.Row,
                        column = c.Column,
                        productName = c.ProductName,
                        currentAmount = c.CurrentAmount,
                        maxCapacity = c.MaxCapacity
                    }).ToList();
                
                if (positionsWithGoods.Any())
                {
                    return BadRequest(new
                    {
                        message = "Không thể giảm kích thước kho vì các vị trí sau đang chứa hàng",
                        error = "Cells with goods cannot be removed",
                        positionsWithGoods,
                        totalPositionsWithGoods = positionsWithGoods.Count,
                        suggestion = "Vui lòng chuyển hàng từ các vị trí này trước khi giảm kích thước kho"
                    });
                }
                
                // If no goods, remove cells completely from database
                if (cellsToRemove.Any())
                {
                    _context.WarehouseCells.RemoveRange(cellsToRemove);
                }

                // Add new cells
                if (newCells.Any())
                {
                    _context.WarehouseCells.AddRange(newCells);
                }

                // Update zone capacity
                // zone.Capacity = request.Width * request.Height; // Property not available
                zone.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Warehouse size updated successfully",
                    newSize = $"{request.Width}x{request.Height}",
                    totalPositions = request.Width * request.Height,
                    cellsAdded = newCells.Count,
                    cellsDeleted = cellsToRemove.Count,
                    action = "Cells with goods were checked and cells without goods were permanently deleted"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating warehouse size", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete warehouse
        /// </summary>
        [HttpDelete("{id}")]
        // [Authorize(Policy = "Warehouse")] // Temporarily disabled for testing
        public async Task<ActionResult<object>> DeleteWarehouse(int id)
        {
            try
            {
                var warehouse = await _context.Warehouses.FindAsync(id);
                if (warehouse == null)
                {
                    return NotFound(new { message = "Warehouse not found" });
                }

                // Soft delete
                warehouse.IsActive = false;
                warehouse.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Warehouse deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting warehouse", error = ex.Message });
            }
        }

        /// <summary>
        /// Get warehouse cell details
        /// </summary>
        [HttpGet("{warehouseId}/cells/{cellId}")]
        public async Task<ActionResult<object>> GetCellDetails(int warehouseId, int cellId)
        {
            try
            {
                var cell = await _context.WarehouseCells
                    .Where(c => c.WarehouseId == warehouseId && c.Id == cellId)
                    .Select(c => new
                    {
                        c.Id,
                        c.Row,
                        c.Column,
                        c.CellCode,
                        c.ClusterName,
                        c.MaxCapacity,
                        c.CurrentAmount,
                        c.Status,
                        c.LastMoved,
                        c.ProductName,
                        c.BatchNumber,
                        c.ProductionDate,
                        c.ExpiryDate,
                        c.Supplier,
                        c.AssignedStaff,
                        c.Temperature,
                        c.Humidity,
                        c.Ventilation,
                        c.SensorStatus,
                        c.ElectronicScale,
                        c.Dimensions,
                        RecentActivities = _context.WarehouseActivities
                            .Where(wa => wa.CellId == cellId)
                            .OrderByDescending(wa => wa.Timestamp)
                            .Take(10)
                            .Select(wa => new
                            {
                                wa.Id,
                                wa.ActivityType,
                                wa.Description,
                                wa.UserName,
                                wa.Timestamp,
                                wa.Status
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (cell == null)
                {
                    return NotFound(new { message = "Cell not found" });
                }

                // Debug: Log environment data
                Console.WriteLine($"🔍 Cell {cellId} environment data: Temperature={cell.Temperature}, Humidity={cell.Humidity}, Ventilation={cell.Ventilation}, SensorStatus={cell.SensorStatus}, ElectronicScale={cell.ElectronicScale}, Dimensions={cell.Dimensions}");

                // If no environment data, set some default values for testing
                if (string.IsNullOrEmpty(cell.Temperature) && string.IsNullOrEmpty(cell.Humidity) && string.IsNullOrEmpty(cell.Ventilation))
                {
                    Console.WriteLine($"⚠️ No environment data found for cell {cellId}, setting default values");
                    
                    // Update the cell with default environment data
                    var cellEntity = await _context.WarehouseCells
                        .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == cellId);
                    
                    if (cellEntity != null)
                    {
                        cellEntity.Temperature = "26°C";
                        cellEntity.Humidity = "53%";
                        cellEntity.Ventilation = "Hoạt động";
                        cellEntity.SensorStatus = "Bình thường";
                        cellEntity.ElectronicScale = "Kết nối";
                        cellEntity.Dimensions = "2.5m x 3m x 2.8m";
                        
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"✅ Updated cell {cellId} with default environment data");
                        
                        // Return updated cell data
                        return Ok(new
                        {
                            cell.Id,
                            cell.Row,
                            cell.Column,
                            cell.CellCode,
                            cell.ClusterName,
                            cell.MaxCapacity,
                            cell.CurrentAmount,
                            cell.Status,
                            cell.LastMoved,
                            cell.ProductName,
                            cell.BatchNumber,
                            cell.ProductionDate,
                            cell.ExpiryDate,
                            cell.Supplier,
                            cell.AssignedStaff,
                            Temperature = cellEntity.Temperature,
                            Humidity = cellEntity.Humidity,
                            Ventilation = cellEntity.Ventilation,
                            SensorStatus = cellEntity.SensorStatus,
                            ElectronicScale = cellEntity.ElectronicScale,
                            Dimensions = cellEntity.Dimensions,
                            RecentActivities = cell.RecentActivities
                        });
                    }
                }

                return Ok(cell);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving cell details", error = ex.Message });
            }
        }

        /// <summary>
        /// Update cell details
        /// </summary>
        [HttpPut("{warehouseId}/cells/{cellId}")]
        public async Task<ActionResult<object>> UpdateCell(int warehouseId, int cellId, [FromBody] UpdateCellRequest request)
        {
            try
            {
                var cell = await _context.WarehouseCells
                    .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == cellId);

                if (cell == null)
                {
                    return NotFound(new { message = "Cell not found" });
                }

                // Update cell properties
                if (request.MaxCapacity.HasValue)
                    cell.MaxCapacity = request.MaxCapacity.Value;
                if (request.CurrentCapacity.HasValue)
                    cell.CurrentAmount = request.CurrentCapacity.Value;
                if (!string.IsNullOrEmpty(request.Status))
                    cell.Status = request.Status;
                if (!string.IsNullOrEmpty(request.Zone))
                    cell.ClusterName = request.Zone;
                if (!string.IsNullOrEmpty(request.AssignedStaff))
                    cell.AssignedStaff = request.AssignedStaff;
                
                // Update environment fields
                if (!string.IsNullOrEmpty(request.Temperature))
                    cell.Temperature = request.Temperature;
                if (!string.IsNullOrEmpty(request.Humidity))
                    cell.Humidity = request.Humidity;
                if (!string.IsNullOrEmpty(request.Ventilation))
                    cell.Ventilation = request.Ventilation;
                if (!string.IsNullOrEmpty(request.SensorStatus))
                    cell.SensorStatus = request.SensorStatus;
                if (!string.IsNullOrEmpty(request.ElectronicScale))
                    cell.ElectronicScale = request.ElectronicScale;
                if (!string.IsNullOrEmpty(request.Dimensions))
                    cell.Dimensions = request.Dimensions;

                cell.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cell updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating cell", error = ex.Message });
            }
        }

        /// <summary>
        /// Import goods to cell
        /// </summary>
        [HttpPost("{warehouseId}/cells/{cellId}/import")]
        public async Task<ActionResult<object>> ImportGoods(int warehouseId, int cellId, [FromBody] ImportGoodsRequest request)
        {
            try
            {
                var cell = await _context.WarehouseCells
                    .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == cellId);

                if (cell == null)
                {
                    return NotFound(new { message = "Cell not found" });
                }

                // Check capacity
                if (cell.CurrentAmount + request.Amount > cell.MaxCapacity)
                {
                    return BadRequest(new { message = "Exceeds maximum capacity" });
                }

                // Update cell
                var oldAmount = cell.CurrentAmount;
                cell.CurrentAmount += request.Amount;
                cell.ProductId = request.ProductId ?? cell.ProductId;
                cell.ProductName = request.ProductName ?? cell.ProductName;
                cell.BatchNumber = request.BatchNumber ?? cell.BatchNumber;
                
                // Update additional product information
                if (!string.IsNullOrEmpty(request.ProductionDate) && DateTime.TryParse(request.ProductionDate, out var productionDate))
                {
                    cell.ProductionDate = productionDate;
                }
                if (!string.IsNullOrEmpty(request.ExpiryDate) && DateTime.TryParse(request.ExpiryDate, out var expiryDate))
                {
                    cell.ExpiryDate = expiryDate;
                }
                if (!string.IsNullOrEmpty(request.Supplier))
                {
                    cell.Supplier = request.Supplier;
                }
                
                // Update unit price if provided
                if (request.UnitPrice.HasValue && request.UnitPrice.Value > 0)
                {
                    cell.UnitPrice = request.UnitPrice.Value;
                }
                
                // Update production date if provided
                if (!string.IsNullOrEmpty(request.ProductionDate))
                {
                    if (DateTime.TryParse(request.ProductionDate, out DateTime prodDate))
                    {
                        cell.ProductionDate = prodDate;
                    }
                }
                
                // Update expiry date if provided
                if (!string.IsNullOrEmpty(request.ExpiryDate))
                {
                    if (DateTime.TryParse(request.ExpiryDate, out DateTime expDate))
                    {
                        cell.ExpiryDate = expDate;
                    }
                }
                
                // Update product batch ID if provided
                if (request.ProductBatchId.HasValue && request.ProductBatchId.Value > 0)
                {
                    cell.ProductBatchId = request.ProductBatchId.Value;
                }
                
                
                cell.Status = cell.CurrentAmount > 0 ? "Occupied" : "Empty";
                cell.LastMoved = DateTime.UtcNow;
                cell.UpdatedAt = DateTime.UtcNow;

                // Create activity log
                var activity = new WarehouseActivity
                {
                    WarehouseId = warehouseId,
                    CellId = cellId,
                    ActivityType = "Import",
                    Description = $"Nhập {request.Amount} {request.ProductName ?? "sản phẩm"} vào ô {cell.CellCode}",
                    ProductName = request.ProductName,
                    BatchNumber = request.BatchNumber,
                    Quantity = request.Amount,
                    Unit = "kg",
                    UserName = request.UserName ?? "System",
                    Timestamp = DateTime.UtcNow,
                    Status = "Completed"
                };

                _context.WarehouseActivities.Add(activity);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Goods imported successfully",
                    cellId = cell.Id,
                    oldAmount = oldAmount,
                    newAmount = cell.CurrentAmount,
                    productName = cell.ProductName,
                    batchNumber = cell.BatchNumber,
                    productionDate = cell.ProductionDate?.ToString("yyyy-MM-dd"),
                    expiryDate = cell.ExpiryDate?.ToString("yyyy-MM-dd"),
                    supplier = cell.Supplier
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while importing goods", error = ex.Message });
            }
        }

        /// <summary>
        /// Export goods from cell
        /// </summary>
        [HttpPost("{warehouseId}/cells/{cellId}/export")]
        public async Task<ActionResult<object>> ExportGoods(int warehouseId, int cellId, [FromBody] ExportGoodsRequest request)
        {
            try
            {
                var cell = await _context.WarehouseCells
                    .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == cellId);

                if (cell == null)
                {
                    return NotFound(new { message = "Cell not found" });
                }

                // Check available amount with detailed validation
                if (request.Amount <= 0)
                {
                    return BadRequest(new { message = "Export amount must be greater than 0" });
                }

                if (cell.CurrentAmount <= 0)
                {
                    return BadRequest(new { message = "Cell is empty, no goods to export" });
                }

                // Calculate total exported amount from ImportOrders (export orders) - using Quantity from details
                var totalExportedFromOrders = await _context.ImportOrders
                    .Where(io => io.WarehouseId == warehouseId && io.OrderType == "Export")
                    .Join(_context.ImportOrderDetails, io => io.Id, iod => iod.ImportOrderId, (io, iod) => new { io, iod })
                    .Where(x => x.iod.WarehouseCellId == cellId)
                    .SumAsync(x => x.iod.Quantity);

                // Calculate remaining amount in cell
                var remainingAmount = cell.CurrentAmount - totalExportedFromOrders;

                // Check if this export would exceed remaining amount
                if (request.Amount > remainingAmount)
                {
                    return BadRequest(new { 
                        message = $"Cannot export! Requested: {request.Amount}, Available: {remainingAmount}, Already exported: {totalExportedFromOrders}" 
                    });
                }

                // Update cell
                var oldAmount = cell.CurrentAmount;
                cell.CurrentAmount -= request.Amount;
                cell.Status = cell.CurrentAmount > 0 ? "Occupied" : "Empty";
                cell.LastMoved = DateTime.UtcNow;
                cell.UpdatedAt = DateTime.UtcNow;
                
                // Recalculate ProductBatch quantity if batch exists
                if (!string.IsNullOrEmpty(cell.BatchNumber))
                {
                    var productBatch = await _context.ProductBatches
                        .FirstOrDefaultAsync(pb => pb.BatchNumber == cell.BatchNumber);
                    if (productBatch != null)
                    {
                        await RecalculateProductBatchQuantity(productBatch.Id);
                    }
                }

                // Clear product information if cell is empty
                if (cell.CurrentAmount <= 0)
                {
                    cell.ProductName = null;
                    cell.BatchNumber = null;
                    cell.ProductionDate = null;
                    cell.ExpiryDate = null;
                    cell.Supplier = null;
                }

                // Create activity log
                var activity = new WarehouseActivity
                {
                    WarehouseId = warehouseId,
                    CellId = cellId,
                    ActivityType = "Export",
                    Description = $"Xuất {request.Amount} {cell.ProductName ?? "sản phẩm"} từ ô {cell.CellCode}",
                    ProductName = cell.ProductName,
                    BatchNumber = cell.BatchNumber,
                    Quantity = request.Amount,
                    Unit = "kg",
                    UserName = request.UserName ?? "System",
                    Timestamp = DateTime.UtcNow,
                    Status = "Completed"
                };

                _context.WarehouseActivities.Add(activity);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Goods exported successfully",
                    cellId = cell.Id,
                    oldAmount = oldAmount,
                    newAmount = cell.CurrentAmount,
                    productName = cell.ProductName,
                    batchNumber = cell.BatchNumber
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while exporting goods", error = ex.Message });
            }
        }

        /// <summary>
        /// Assign staff to cell
        /// </summary>
        [HttpPost("{warehouseId}/cells/{cellId}/assign-staff")]
        public async Task<ActionResult<object>> AssignStaff(int warehouseId, int cellId, [FromBody] AssignStaffRequest request)
        {
            try
            {
                var cell = await _context.WarehouseCells
                    .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == cellId);

                if (cell == null)
                {
                    return NotFound(new { message = "Cell not found" });
                }

                // Check if staff is already assigned
                if (!string.IsNullOrEmpty(cell.AssignedStaff) && cell.AssignedStaff.Equals(request.StaffName, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { 
                        message = $"Nhân viên {request.StaffName} đã được gán vào vị trí này rồi!",
                        alreadyAssigned = true
                    });
                }

                // Update cell with staff assignment
                var oldStaff = cell.AssignedStaff;
                cell.AssignedStaff = request.StaffName;
                cell.UpdatedAt = DateTime.UtcNow;

                // Create activity log
                var activity = new WarehouseActivity
                {
                    WarehouseId = warehouseId,
                    CellId = cellId,
                    ActivityType = "StaffAssignment",
                    Description = $"Gán nhân viên {request.StaffName} vào ô {cell.CellCode}",
                    UserName = request.UserName ?? "System",
                    Timestamp = DateTime.UtcNow,
                    Status = "Completed",
                    Notes = request.Notes
                };

                _context.WarehouseActivities.Add(activity);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Staff assigned successfully",
                    cellId = cell.Id,
                    oldStaff = oldStaff,
                    assignedStaff = cell.AssignedStaff
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while assigning staff", error = ex.Message });
            }
        }

        /// <summary>
        /// Update warehouse position
        /// </summary>
        [HttpPut("{warehouseId}/positions/{positionId}")]
        public async Task<ActionResult<object>> UpdatePosition(int warehouseId, int positionId, [FromBody] UpdatePositionRequest request)
        {
            try
            {
                var cell = await _context.WarehouseCells
                    .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == positionId);

                if (cell == null)
                {
                    return NotFound(new { message = "Position not found" });
                }

                // Update cell properties
                if (!string.IsNullOrEmpty(request.AssignedStaff))
                {
                    cell.AssignedStaff = request.AssignedStaff;
                }
                if (request.LastUpdated.HasValue)
                {
                    cell.UpdatedAt = request.LastUpdated.Value;
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Position updated successfully",
                    positionId = cell.Id,
                    assignedStaff = cell.AssignedStaff
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating position", error = ex.Message });
            }
        }

        /// <summary>
        /// Clear product from cell (remove product information completely)
        /// </summary>
        [HttpPost("{warehouseId}/cells/{cellId}/clear-product")]
        public async Task<ActionResult<object>> ClearProduct(int warehouseId, int cellId, [FromBody] ClearProductRequest request)
        {
            try
            {
                var cell = await _context.WarehouseCells
                    .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == cellId);

                if (cell == null)
                {
                    return NotFound(new { message = "Cell not found" });
                }

                // Store product info for activity log
                var productName = cell.ProductName;
                var cellCode = cell.CellCode;
                var batchNumber = cell.BatchNumber;
                var currentAmount = cell.CurrentAmount;

                // Recalculate ProductBatch quantity if batch exists
                if (!string.IsNullOrEmpty(batchNumber) && currentAmount > 0)
                {
                    var productBatch = await _context.ProductBatches
                        .FirstOrDefaultAsync(pb => pb.BatchNumber == batchNumber);
                    if (productBatch != null)
                    {
                        await RecalculateProductBatchQuantity(productBatch.Id);
                    }
                }

                // Clear all product information from cell
                cell.ProductName = null;
                cell.CurrentAmount = 0;
                cell.BatchNumber = null;
                cell.ProductionDate = null;
                cell.ExpiryDate = null;
                cell.Supplier = null;
                cell.UpdatedAt = DateTime.UtcNow;

                // Create activity log for clearing product
                var activity = new WarehouseActivity
                {
                    WarehouseId = warehouseId,
                    CellId = cellId,
                    ActivityType = "ClearProduct",
                    Description = $"Xóa sản phẩm \"{productName}\" khỏi vị trí {cellCode}",
                    ProductName = productName,
                    UserName = request.UserName ?? "System",
                    Timestamp = DateTime.UtcNow,
                    Status = "Completed",
                    Notes = request.Notes ?? "Sản phẩm hết hàng"
                };

                _context.WarehouseActivities.Add(activity);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Product cleared successfully",
                    cellId = cell.Id,
                    clearedProduct = productName,
                    cellCode = cellCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while clearing product", error = ex.Message });
            }
        }

        /// <summary>
        /// Get cell activities
        /// </summary>
        [HttpGet("{warehouseId}/cells/{cellId}/activities")]
        public async Task<ActionResult<IEnumerable<object>>> GetCellActivities(int warehouseId, int cellId)
        {
            try
            {
                var activities = await _context.WarehouseActivities
                    .Where(a => a.WarehouseId == warehouseId && a.CellId == cellId && a.IsActive)
                    .OrderByDescending(a => a.Timestamp)
                    .Take(50) // Get last 50 activities
                    .Select(a => new
                    {
                        Type = a.ActivityType,
                        Description = a.Description,
                        Timestamp = a.Timestamp,
                        User = a.UserName,
                        ProductName = a.ProductName,
                        BatchNumber = a.BatchNumber,
                        Quantity = a.Quantity,
                        Unit = a.Unit,
                        Status = a.Status,
                        Notes = a.Notes
                    })
                    .ToListAsync();

                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving activities", error = ex.Message });
            }
        }

        /// <summary>
        /// Maintenance cell
        /// </summary>
        [HttpPost("{warehouseId}/cells/{cellId}/maintenance")]
        public async Task<ActionResult<object>> MaintenanceCell(int warehouseId, int cellId, [FromBody] MaintenanceRequest request)
        {
            try
            {
                var cell = await _context.WarehousePositions
                    .FirstOrDefaultAsync(p => p.WarehouseId == warehouseId && p.Id == cellId);

                if (cell == null)
                {
                    return NotFound(new { message = "Cell not found" });
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Maintenance completed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while performing maintenance", error = ex.Message });
            }
        }

        /// <summary>
        /// Inventory check cell
        /// </summary>
        [HttpPost("{warehouseId}/cells/{cellId}/inventory-check")]
        public async Task<ActionResult<object>> InventoryCheck(int warehouseId, int cellId, [FromBody] InventoryCheckRequest request)
        {
            try
            {
                var cell = await _context.WarehousePositions
                    .FirstOrDefaultAsync(p => p.WarehouseId == warehouseId && p.Id == cellId);

                if (cell == null)
                {
                    return NotFound(new { message = "Cell not found" });
                }

                // Update actual capacity
                cell.CurrentCapacity = request.ActualAmount;
                cell.Status = cell.CurrentCapacity > 0 ? "Occupied" : "Empty";
                cell.LastUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Inventory check completed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while performing inventory check", error = ex.Message });
            }
        }

        [HttpDelete("{warehouseId}/cells/{cellId}/clear")]
        public async Task<IActionResult> ClearCell(int warehouseId, int cellId)
        {
            try
            {
                // Tìm vị trí kho
                var cell = await _context.WarehouseCells
                    .FirstOrDefaultAsync(wc => wc.Id == cellId && wc.WarehouseId == warehouseId);

                if (cell == null)
                {
                    return NotFound(new { 
                        message = "🔍 KHÔNG TÌM THẤY VỊ TRÍ 🔍\n\n" +
                                 $"Vị trí kho với ID {cellId} không tồn tại trong hệ thống.\n\n" +
                                 "📋 Có thể do:\n" +
                                 "• Vị trí đã bị xóa trước đó\n" +
                                 "• ID vị trí không chính xác\n" +
                                 "• Vị trí không thuộc kho này\n\n" +
                                 "💡 Vui lòng kiểm tra lại thông tin vị trí."
                    });
                }

                // Kiểm tra xem còn sản phẩm nào chưa xuất hết không
                var cellProducts = await _context.WarehouseCellProducts
                    .Where(wcp => wcp.WarehouseCellId == cellId && wcp.IsActive == true)
                    .ToListAsync();

                Console.WriteLine($"🔍 Checking cell {cellId} for remaining products:");
                Console.WriteLine($"Found {cellProducts.Count} active cell products");
                
                // Tính tổng số lượng còn lại trong vị trí (chỉ tính số dương)
                var totalRemainingQuantity = cellProducts
                    .Where(wcp => wcp.Quantity > 0)
                    .Sum(wcp => wcp.Quantity);
                
                Console.WriteLine($"Total remaining quantity: {totalRemainingQuantity}");
                
                // Log chi tiết từng sản phẩm
                foreach (var product in cellProducts)
                {
                    Console.WriteLine($"  - ProductId: {product.ProductId}, Quantity: {product.Quantity}, IsActive: {product.IsActive}");
                }
                
                if (totalRemainingQuantity > 0)
                {
                    Console.WriteLine($"❌ Blocking deletion - {totalRemainingQuantity} kg remaining");
                    return BadRequest(new { 
                        message = $"⚠️ KHÔNG THỂ XÓA VỊ TRÍ ⚠️\n\n" +
                                 $"Vị trí {cell.CellCode} vẫn còn {totalRemainingQuantity} kg hàng hóa chưa xuất hết.\n\n" +
                                 $"📋 Hành động cần thực hiện:\n" +
                                 $"• Xuất hết hàng hóa trong vị trí này\n" +
                                 $"• Sau đó mới có thể xóa vị trí\n\n" +
                                 $"💡 Lưu ý: Chỉ được xóa vị trí khi đã trống hoàn toàn."
                    });
                }
                
                Console.WriteLine($"✅ No remaining products - allowing deletion");

                // Xóa sạch tất cả sản phẩm trong vị trí (nếu có)
                if (cellProducts.Any())
                {
                    _context.WarehouseCellProducts.RemoveRange(cellProducts);
                }

                // Xóa tất cả hoạt động cũ liên quan đến vị trí này
                var oldActivities = await _context.WarehouseActivities
                    .Where(wa => wa.CellId == cellId)
                    .ToListAsync();

                if (oldActivities.Any())
                {
                    _context.WarehouseActivities.RemoveRange(oldActivities);
                    Console.WriteLine($"Removed {oldActivities.Count} old activities for cell {cellId}");
                }

                // Reset thông tin vị trí
                cell.ProductName = null;
                cell.BatchNumber = null;
                cell.ProductionDate = null;
                cell.ExpiryDate = null;
                cell.Supplier = null;
                cell.CurrentAmount = 0;
                cell.Status = "Empty";
                cell.UpdatedAt = DateTime.UtcNow;

                // Tạo hoạt động ghi log - kiểm tra UserId tồn tại
                try
                {
                    // Kiểm tra xem có User nào tồn tại không
                    var existingUser = await _context.Users.FirstOrDefaultAsync();
                    if (existingUser != null)
                    {
                        var activity = new WarehouseActivity
                        {
                            WarehouseId = warehouseId,
                            CellId = cellId,
                            ActivityType = "Clear",
                            Description = $"Xóa sạch vị trí {cell.CellCode}",
                            UserId = existingUser.Id,
                            Timestamp = DateTime.UtcNow,
                            Status = "Completed",
                            IsActive = true
                        };

                        _context.WarehouseActivities.Add(activity);
                        Console.WriteLine($"Created activity with UserId: {existingUser.Id}");
                    }
                    else
                    {
                        Console.WriteLine("No users found in database, skipping activity creation");
                    }
                }
                catch (Exception ex)
                {
                    // Log error but don't fail the operation
                    Console.WriteLine($"Error creating activity: {ex.Message}");
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "✅ XÓA VỊ TRÍ THÀNH CÔNG ✅\n\n" +
                             $"Vị trí {cell.CellCode} đã được xóa sạch hoàn toàn.\n\n" +
                             $"📊 Thông tin đã xóa:\n" +
                             $"• Tất cả sản phẩm trong vị trí\n" +
                             $"• Lịch sử hoạt động cũ\n" +
                             $"• Thông tin chi tiết sản phẩm\n\n" +
                             $"🎯 Vị trí hiện tại: Trống và sẵn sàng sử dụng"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "❌ LỖI KHI XÓA VỊ TRÍ ❌\n\n" +
                             "Đã xảy ra lỗi không mong muốn khi thực hiện xóa vị trí.\n\n" +
                             "🔧 Thông tin lỗi:\n" +
                             $"{ex.Message}\n\n" +
                             "📞 Vui lòng liên hệ quản trị viên để được hỗ trợ.",
                    error = ex.Message 
                });
            }
        }
    }

    // DTOs
    public class UpdateWarehouseSizeRequest
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class CreateWarehouseRequest
    {
        [Required(ErrorMessage = "Warehouse name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;
        
        [Range(1, 50, ErrorMessage = "Width must be between 1 and 50")]
        public int Width { get; set; }
        
        [Range(1, 50, ErrorMessage = "Height must be between 1 and 50")]
        public int Height { get; set; }
    }

    public class UpdateCellRequest
    {
        public int? MaxCapacity { get; set; }
        public int? CurrentCapacity { get; set; }
        public string? Status { get; set; }
        public string? Zone { get; set; }
        public string? AssignedStaff { get; set; }
        
        // Environment fields
        public string? Temperature { get; set; }
        public string? Humidity { get; set; }
        public string? Ventilation { get; set; }
        public string? SensorStatus { get; set; }
        public string? ElectronicScale { get; set; }
        public string? Dimensions { get; set; }
    }

    public class ImportGoodsRequest
    {
        public int Amount { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? BatchNumber { get; set; }
        public string? ProductionDate { get; set; }
        public string? ExpiryDate { get; set; }
        public string? Supplier { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? ProductBatchId { get; set; }
        public string? UserName { get; set; }
        public string? Notes { get; set; }
    }

    public class ExportGoodsRequest
    {
        public int Amount { get; set; }
        public string? Reason { get; set; }
        public string? UserName { get; set; }
        public string? Notes { get; set; }
    }

    public class AssignStaffRequest
    {
        public string StaffName { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdatePositionRequest
    {
        public string? AssignedStaff { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class AssignStaffToPositionRequest
    {
        public string StaffName { get; set; } = string.Empty;
    }

    public class ClearProductRequest
    {
        public string? UserName { get; set; }
        public string? Notes { get; set; }
    }

    public class MaintenanceRequest
    {
        public string Description { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class InventoryCheckRequest
    {
        public int ActualAmount { get; set; }
        public string? Notes { get; set; }
    }
}
