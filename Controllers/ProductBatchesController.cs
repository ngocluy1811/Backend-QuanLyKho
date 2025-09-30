using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductBatchesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductBatchesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/ProductBatches/debug/{id}
    [HttpGet("debug/{id}")]
    public async Task<ActionResult<object>> DebugProductBatch(int id)
    {
        try
        {
            var productBatch = await _context.ProductBatches.FindAsync(id);
            if (productBatch == null)
            {
                return NotFound(new { message = "ProductBatch not found" });
            }

            // Calculate actual quantities
            var totalImportedQuantity = await _context.ImportOrderDetails
                .Where(iod => iod.ProductBatchId == id)
                .SumAsync(iod => iod.Quantity);

            var totalExportedQuantity = await _context.WarehouseActivities
                .Where(wa => wa.BatchNumber == productBatch.BatchNumber && wa.ActivityType == "Export")
                .SumAsync(wa => wa.Quantity);

            var totalClearedQuantity = await _context.WarehouseActivities
                .Where(wa => wa.BatchNumber == productBatch.BatchNumber && wa.ActivityType == "ClearProduct")
                .SumAsync(wa => wa.Quantity);

            var calculatedQuantity = (int)(totalImportedQuantity - totalExportedQuantity - totalClearedQuantity);

            return Ok(new
            {
                BatchId = productBatch.Id,
                BatchNumber = productBatch.BatchNumber,
                BatchName = productBatch.BatchName,
                CurrentQuantity = productBatch.Quantity,
                CurrentQuantityField = productBatch.CurrentQuantity,
                MaxQuantity = productBatch.Quantity, // Using Quantity as MaxQuantity for now
                CalculatedQuantity = calculatedQuantity,
                TotalImported = totalImportedQuantity,
                TotalExported = totalExportedQuantity,
                TotalCleared = totalClearedQuantity,
                ImportOrders = await _context.ImportOrderDetails
                    .Where(iod => iod.ProductBatchId == id)
                    .Select(iod => new { iod.Id, iod.Quantity, iod.ProductId })
                    .ToListAsync(),
                WarehouseActivities = await _context.WarehouseActivities
                    .Where(wa => wa.BatchNumber == productBatch.BatchNumber)
                    .Select(wa => new { wa.Id, wa.ActivityType, wa.Quantity, wa.Timestamp })
                    .ToListAsync()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while debugging product batch", error = ex.Message });
        }
    }

    // GET: api/ProductBatches
    [HttpGet]
    public async Task<ActionResult<object>> GetProductBatches([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.ProductBatches
                .Include(pb => pb.Product)
                .Include(pb => pb.Supplier)
                .Where(pb => pb.IsActive);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var batches = await query
                .Select(pb => new
                {
                    pb.Id,
                    pb.BatchNumber,
                    pb.BatchName,
                    pb.Description,
                    pb.ProductId,
                    ProductName = pb.Product.ProductName,
                    pb.ProductionDate,
                    pb.ExpiryDate,
                    pb.Quantity,
                    pb.RemainingQuantity,
                    pb.InitialQuantity,
                    pb.CurrentQuantity,
                    pb.Status,
                    pb.QualityStatus,
                    pb.SupplierId,
                    // Thêm 3 trường mới
                    pb.NgayVe,
                    pb.SoDotVe,
                    pb.SoXeContainerTungDot,
                    pb.NgayVeDetails, // Lấy raw JSON string
                    SupplierName = pb.Supplier != null ? pb.Supplier.SupplierName : null,
                    pb.UnitPrice,
                    pb.TotalValue,
                    pb.Notes,
                    pb.CreatedAt,
                    pb.UpdatedAt
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Tính toán số lượng thực tế cho mỗi batch
            var processedBatches = new List<object>();
            foreach (var batch in batches)
            {
                // Tính toán số lượng thực tế
                var totalImportedQuantity = await _context.ImportOrderDetails
                    .Where(iod => iod.ProductBatchId == batch.Id)
                    .SumAsync(iod => iod.Quantity);

                var totalExportedQuantity = await _context.WarehouseActivities
                    .Where(wa => wa.BatchNumber == batch.BatchNumber && wa.ActivityType == "Export")
                    .SumAsync(wa => wa.Quantity);

                var totalClearedQuantity = await _context.WarehouseActivities
                    .Where(wa => wa.BatchNumber == batch.BatchNumber && wa.ActivityType == "ClearProduct")
                    .SumAsync(wa => wa.Quantity);

                var calculatedQuantity = (int)(totalImportedQuantity - totalExportedQuantity - totalClearedQuantity);
                if (calculatedQuantity < 0) calculatedQuantity = 0;

                // Cập nhật database với số lượng tính toán thực tế
                var productBatch = await _context.ProductBatches.FindAsync(batch.Id);
                if (productBatch != null)
                {
                    productBatch.CurrentQuantity = calculatedQuantity;
                    productBatch.RemainingQuantity = calculatedQuantity;
                    productBatch.UpdatedAt = DateTime.UtcNow;
                }

                processedBatches.Add(new
                {
                    batch.Id,
                    batch.BatchNumber,
                    batch.BatchName,
                    batch.Description,
                    batch.ProductId,
                    batch.ProductName,
                    batch.ProductionDate,
                    batch.ExpiryDate,
                    Quantity = batch.Quantity, // Giữ nguyên max quantity
                    RemainingQuantity = calculatedQuantity, // Số lượng còn lại thực tế
                    InitialQuantity = batch.InitialQuantity,
                    CurrentQuantity = calculatedQuantity, // Số lượng hiện tại thực tế
                    batch.Status,
                    batch.QualityStatus,
                    batch.SupplierId,
                    batch.NgayVe,
                    batch.SoDotVe,
                    batch.SoXeContainerTungDot,
                    batch.NgayVeDetails,
                    batch.SupplierName,
                    batch.UnitPrice,
                    batch.TotalValue,
                    batch.Notes,
                    batch.CreatedAt,
                    batch.UpdatedAt
                });
            }

            // Lưu tất cả thay đổi vào database
            await _context.SaveChangesAsync();

            // Xử lý JSON deserialization sau khi query
            var finalBatches = processedBatches.Select(b => {
                dynamic batch = b;
                return new
                {
                    Id = batch.Id,
                    BatchNumber = batch.BatchNumber,
                    BatchName = batch.BatchName,
                    Description = batch.Description,
                    ProductId = batch.ProductId,
                    ProductName = batch.ProductName,
                    ProductionDate = batch.ProductionDate,
                    ExpiryDate = batch.ExpiryDate,
                    Quantity = batch.Quantity,
                    RemainingQuantity = batch.RemainingQuantity,
                    InitialQuantity = batch.InitialQuantity,
                    CurrentQuantity = batch.CurrentQuantity,
                    Status = batch.Status,
                    QualityStatus = batch.QualityStatus,
                    SupplierId = batch.SupplierId,
                    NgayVe = batch.NgayVe,
                    SoDotVe = batch.SoDotVe,
                    SoXeContainerTungDot = batch.SoXeContainerTungDot,
                    NgayVeDetails = !string.IsNullOrEmpty(batch.NgayVeDetails) ? System.Text.Json.JsonSerializer.Deserialize<string[]>(batch.NgayVeDetails) : null,
                    SupplierName = batch.SupplierName,
                    UnitPrice = batch.UnitPrice,
                    TotalValue = batch.TotalValue,
                    Notes = batch.Notes,
                    CreatedAt = batch.CreatedAt,
                    UpdatedAt = batch.UpdatedAt
                };
            }).ToList();

            return Ok(new
            {
                batches = finalBatches,
                pagination = new
                {
                    currentPage = page,
                    pageSize,
                    totalCount,
                    totalPages,
                    hasNextPage = page < totalPages,
                    hasPreviousPage = page > 1
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving product batches", error = ex.Message });
        }
    }

    // GET: api/ProductBatches/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetProductBatch(int id)
    {
        try
        {
            var batch = await _context.ProductBatches
                .Include(pb => pb.Product)
                .Include(pb => pb.Supplier)
                .Where(pb => pb.Id == id && pb.IsActive)
                .Select(pb => new
                {
                    pb.Id,
                    pb.BatchNumber,
                    pb.BatchName,
                    pb.Description,
                    pb.ProductId,
                    ProductName = pb.Product.ProductName,
                    pb.ProductionDate,
                    pb.ExpiryDate,
                    pb.Quantity,
                    pb.RemainingQuantity,
                    pb.InitialQuantity,
                    pb.CurrentQuantity,
                    pb.Status,
                    pb.QualityStatus,
                    pb.SupplierId,
                    SupplierName = pb.Supplier != null ? pb.Supplier.SupplierName : null,
                    pb.UnitPrice,
                    pb.TotalValue,
                    pb.Notes,
                    pb.CreatedAt,
                    pb.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (batch == null)
            {
                return NotFound(new { message = "Product batch not found" });
            }

            return Ok(batch);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving product batch", error = ex.Message });
        }
    }

    // POST: api/ProductBatches
    [HttpPost]
    public async Task<ActionResult<object>> CreateProductBatch([FromBody] CreateProductBatchRequest request)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrEmpty(request.BatchNumber) || string.IsNullOrEmpty(request.BatchName))
            {
                return BadRequest(new { message = "BatchNumber and BatchName are required" });
            }

            // Check if batch number already exists
            var existingBatch = await _context.ProductBatches
                .FirstOrDefaultAsync(pb => pb.BatchNumber == request.BatchNumber);
            
            if (existingBatch != null)
            {
                return BadRequest(new { message = "Batch number already exists" });
            }

            // Verify product exists
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return BadRequest(new { message = "Product not found" });
            }

            var batch = new ProductBatch
            {
                BatchNumber = request.BatchNumber,
                BatchName = request.BatchName,
                Description = request.Description,
                ProductId = request.ProductId,
                ProductionDate = request.ProductionDate,
                ExpiryDate = request.ExpiryDate,
                Quantity = request.Quantity,
                RemainingQuantity = request.Quantity,
                InitialQuantity = request.Quantity,
                CurrentQuantity = request.Quantity,
                Status = request.Status ?? "Active",
                QualityStatus = request.QualityStatus ?? 1,
                SupplierId = request.SupplierId,
                UnitPrice = request.UnitPrice,
                TotalValue = request.UnitPrice.HasValue && request.Quantity > 0 ? request.UnitPrice.Value * request.Quantity : null,
                Notes = request.Notes,
                // Thêm 3 trường mới
                NgayVe = request.NgayVe,
                SoDotVe = request.SoDotVe,
                SoXeContainerTungDot = request.SoXeContainerTungDot,
                NgayVeDetails = request.NgayVeDetails != null ? System.Text.Json.JsonSerializer.Serialize(request.NgayVeDetails) : null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductBatches.Add(batch);
            await _context.SaveChangesAsync();

            // Return the created batch with related data
            var createdBatch = await _context.ProductBatches
                .Include(pb => pb.Product)
                .Include(pb => pb.Supplier)
                .Where(pb => pb.Id == batch.Id)
                .Select(pb => new
                {
                    pb.Id,
                    pb.BatchNumber,
                    pb.BatchName,
                    pb.Description,
                    pb.ProductId,
                    ProductName = pb.Product.ProductName,
                    pb.ProductionDate,
                    pb.ExpiryDate,
                    pb.Quantity,
                    pb.RemainingQuantity,
                    pb.InitialQuantity,
                    pb.CurrentQuantity,
                    pb.Status,
                    pb.QualityStatus,
                    pb.SupplierId,
                    SupplierName = pb.Supplier != null ? pb.Supplier.SupplierName : null,
                    pb.UnitPrice,
                    pb.TotalValue,
                    pb.Notes,
                    pb.CreatedAt,
                    pb.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetProductBatch), new { id = batch.Id }, createdBatch);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating product batch", error = ex.Message });
        }
    }

    // PUT: api/ProductBatches/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<object>> UpdateProductBatch(int id, [FromBody] UpdateProductBatchRequest request)
    {
        try
        {
            var batch = await _context.ProductBatches.FindAsync(id);
            if (batch == null)
            {
                return NotFound(new { message = "Product batch not found" });
            }

            // Check if batch number already exists (excluding current batch)
            if (!string.IsNullOrEmpty(request.BatchNumber) && request.BatchNumber != batch.BatchNumber)
            {
                var existingBatch = await _context.ProductBatches
                    .FirstOrDefaultAsync(pb => pb.BatchNumber == request.BatchNumber && pb.Id != id);
                
                if (existingBatch != null)
                {
                    return BadRequest(new { message = "Batch number already exists" });
                }
            }

            // Update batch properties
            if (!string.IsNullOrEmpty(request.BatchNumber))
                batch.BatchNumber = request.BatchNumber;
            if (!string.IsNullOrEmpty(request.BatchName))
                batch.BatchName = request.BatchName;
            if (request.Description != null)
                batch.Description = request.Description;
            if (request.ProductId.HasValue)
                batch.ProductId = request.ProductId.Value;
            if (request.ProductionDate.HasValue)
                batch.ProductionDate = request.ProductionDate;
            if (request.ExpiryDate.HasValue)
                batch.ExpiryDate = request.ExpiryDate;
            if (request.Quantity.HasValue)
            {
                batch.Quantity = request.Quantity.Value;
                batch.RemainingQuantity = request.Quantity.Value;
                batch.InitialQuantity = request.Quantity.Value;
                batch.CurrentQuantity = request.Quantity.Value;
            }
            if (!string.IsNullOrEmpty(request.Status))
                batch.Status = request.Status;
            if (request.QualityStatus.HasValue)
                batch.QualityStatus = request.QualityStatus.Value;
            if (request.SupplierId.HasValue)
                batch.SupplierId = request.SupplierId;
            if (request.UnitPrice.HasValue)
            {
                batch.UnitPrice = request.UnitPrice;
                batch.TotalValue = request.UnitPrice.Value * batch.Quantity;
            }
            if (request.Notes != null)
                batch.Notes = request.Notes;
            if (request.IsActive.HasValue)
                batch.IsActive = request.IsActive.Value;
            
            // Thêm xử lý cho 3 trường mới
            if (request.NgayVe.HasValue)
                batch.NgayVe = request.NgayVe;
            if (request.SoDotVe.HasValue)
                batch.SoDotVe = request.SoDotVe;
            if (request.SoXeContainerTungDot.HasValue)
                batch.SoXeContainerTungDot = request.SoXeContainerTungDot;
            if (request.NgayVeDetails != null)
                batch.NgayVeDetails = System.Text.Json.JsonSerializer.Serialize(request.NgayVeDetails);

            batch.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Return the updated batch with related data
            var updatedBatch = await _context.ProductBatches
                .Include(pb => pb.Product)
                .Include(pb => pb.Supplier)
                .Where(pb => pb.Id == batch.Id)
                .Select(pb => new
                {
                    pb.Id,
                    pb.BatchNumber,
                    pb.BatchName,
                    pb.Description,
                    pb.ProductId,
                    ProductName = pb.Product.ProductName,
                    pb.ProductionDate,
                    pb.ExpiryDate,
                    pb.Quantity,
                    pb.RemainingQuantity,
                    pb.InitialQuantity,
                    pb.CurrentQuantity,
                    pb.Status,
                    pb.QualityStatus,
                    pb.SupplierId,
                    SupplierName = pb.Supplier != null ? pb.Supplier.SupplierName : null,
                    pb.UnitPrice,
                    pb.TotalValue,
                    pb.Notes,
                    pb.CreatedAt,
                    pb.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return Ok(updatedBatch);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating product batch", error = ex.Message });
        }
    }

    // DELETE: api/ProductBatches/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProductBatch(int id)
    {
        try
        {
            var batch = await _context.ProductBatches.FindAsync(id);
            if (batch == null)
            {
                return NotFound(new { message = "Product batch not found" });
            }

            // Soft delete
            batch.IsActive = false;
            batch.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product batch deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting product batch", error = ex.Message });
        }
    }

    // GET: api/ProductBatches/by-product/{productId}
    [HttpGet("by-product/{productId}")]
    public async Task<ActionResult<object>> GetBatchesByProduct(int productId)
    {
        try
        {
            var batches = await _context.ProductBatches
                .Include(pb => pb.Product)
                .Include(pb => pb.Supplier)
                .Where(pb => pb.ProductId == productId && pb.IsActive)
                .Select(pb => new
                {
                    pb.Id,
                    pb.BatchNumber,
                    pb.BatchName,
                    pb.Description,
                    pb.ProductId,
                    ProductName = pb.Product.ProductName,
                    pb.ProductionDate,
                    pb.ExpiryDate,
                    pb.Quantity,
                    pb.RemainingQuantity,
                    pb.InitialQuantity,
                    pb.CurrentQuantity,
                    pb.Status,
                    pb.QualityStatus,
                    pb.SupplierId,
                    SupplierName = pb.Supplier != null ? pb.Supplier.SupplierName : null,
                    pb.UnitPrice,
                    pb.TotalValue,
                    pb.Notes,
                    pb.CreatedAt,
                    pb.UpdatedAt
                })
                .ToListAsync();

            return Ok(batches);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving product batches", error = ex.Message });
        }
    }

    // Seed sample product batches
    [HttpPost("seed")]
    public async Task<ActionResult> SeedProductBatches()
    {
        try
        {
            // Check if batches already exist
            if (await _context.ProductBatches.AnyAsync())
            {
                return Ok(new { message = "Product batches already exist" });
            }

            // Get first few products and suppliers
            var products = await _context.Products.Take(5).ToListAsync();
            var suppliers = await _context.Suppliers.Take(3).ToListAsync();

            if (!products.Any() || !suppliers.Any())
            {
                return BadRequest(new { message = "No products or suppliers found. Please seed products and suppliers first." });
            }

            var sampleBatches = new List<ProductBatch>
            {
                new ProductBatch
                {
                    BatchNumber = "LOT001",
                    BatchName = "Lô NPK 20-10-24 - 2025",
                    Description = "Lô phân NPK chuyên cà phê",
                    ProductId = products[0].Id,
                    ProductionDate = DateTime.UtcNow.AddDays(-30),
                    ExpiryDate = DateTime.UtcNow.AddDays(365),
                    Quantity = 1000,
                    RemainingQuantity = 800,
                    InitialQuantity = 1000,
                    CurrentQuantity = 800,
                    Status = "Active",
                    QualityStatus = 1,
                    SupplierId = suppliers[0].Id,
                    UnitPrice = 85000,
                    TotalValue = 85000000,
                    Notes = "Lô hàng chất lượng cao",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ProductBatch
                {
                    BatchNumber = "LOT002",
                    BatchName = "Lô NPK 15-15-15 - 2025",
                    Description = "Lô phân NPK cân bằng",
                    ProductId = products[1].Id,
                    ProductionDate = DateTime.UtcNow.AddDays(-20),
                    ExpiryDate = DateTime.UtcNow.AddDays(400),
                    Quantity = 2000,
                    RemainingQuantity = 1500,
                    InitialQuantity = 2000,
                    CurrentQuantity = 1500,
                    Status = "Active",
                    QualityStatus = 1,
                    SupplierId = suppliers[1].Id,
                    UnitPrice = 75000,
                    TotalValue = 150000000,
                    Notes = "Lô hàng ổn định",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ProductBatch
                {
                    BatchNumber = "LOT003",
                    BatchName = "Lô HD 302 - 2025",
                    Description = "Lô phân hữu cơ HD 302",
                    ProductId = products[2].Id,
                    ProductionDate = DateTime.UtcNow.AddDays(-15),
                    ExpiryDate = DateTime.UtcNow.AddDays(300),
                    Quantity = 500,
                    RemainingQuantity = 300,
                    InitialQuantity = 500,
                    CurrentQuantity = 300,
                    Status = "Active",
                    QualityStatus = 1,
                    SupplierId = suppliers[2].Id,
                    UnitPrice = 120000,
                    TotalValue = 60000000,
                    Notes = "Lô hàng hữu cơ cao cấp",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ProductBatch
                {
                    BatchNumber = "LOT004",
                    BatchName = "Lô HD Active - 2025",
                    Description = "Lô phân HD Active",
                    ProductId = products[3].Id,
                    ProductionDate = DateTime.UtcNow.AddDays(-10),
                    ExpiryDate = DateTime.UtcNow.AddDays(350),
                    Quantity = 800,
                    RemainingQuantity = 600,
                    InitialQuantity = 800,
                    CurrentQuantity = 600,
                    Status = "Active",
                    QualityStatus = 1,
                    SupplierId = suppliers[0].Id,
                    UnitPrice = 95000,
                    TotalValue = 76000000,
                    Notes = "Lô hàng chất lượng tốt",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ProductBatch
                {
                    BatchNumber = "LOT005",
                    BatchName = "Lô HD Strong - 2025",
                    Description = "Lô phân HD Strong",
                    ProductId = products[4].Id,
                    ProductionDate = DateTime.UtcNow.AddDays(-5),
                    ExpiryDate = DateTime.UtcNow.AddDays(400),
                    Quantity = 1200,
                    RemainingQuantity = 1000,
                    InitialQuantity = 1200,
                    CurrentQuantity = 1000,
                    Status = "Active",
                    QualityStatus = 1,
                    SupplierId = suppliers[1].Id,
                    UnitPrice = 88000,
                    TotalValue = 105600000,
                    Notes = "Lô hàng mới nhập",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.ProductBatches.AddRange(sampleBatches);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Successfully seeded {sampleBatches.Count} product batches" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while seeding product batches", error = ex.Message });
        }
    }
}

// Request DTOs
public class CreateProductBatchRequest
{
    public string BatchNumber { get; set; } = string.Empty;
    public string BatchName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProductId { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public string? Status { get; set; }
    public int? QualityStatus { get; set; }
    public int? SupplierId { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Notes { get; set; }
    // Thêm 3 trường mới
    public DateTime? NgayVe { get; set; }
    public int? SoDotVe { get; set; }
    public int? SoXeContainerTungDot { get; set; }
    public string[]? NgayVeDetails { get; set; } // Mảng ngày về cho từng đợt
}

public class UpdateProductBatchRequest
{
    public string? BatchNumber { get; set; }
    public string? BatchName { get; set; }
    public string? Description { get; set; }
    public int? ProductId { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? Quantity { get; set; }
    public string? Status { get; set; }
    public int? QualityStatus { get; set; }
    public int? SupplierId { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Notes { get; set; }
    public bool? IsActive { get; set; }
    // Thêm 3 trường mới
    public DateTime? NgayVe { get; set; }
    public int? SoDotVe { get; set; }
    public int? SoXeContainerTungDot { get; set; }
    public string[]? NgayVeDetails { get; set; } // Mảng ngày về cho từng đợt
}
