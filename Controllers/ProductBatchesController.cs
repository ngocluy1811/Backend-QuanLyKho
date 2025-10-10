using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.DTOs;

namespace FertilizerWarehouseAPI.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class ProductBatchesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductBatchesController(ApplicationDbContext context)
    {
        _context = context;
    }

        // GET: api/productbatches/test
        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult TestProductBatches()
        {
            return Ok(new { 
                success = true, 
                data = new List<object>(),
                currentPage = 1,
                totalPages = 1,
                totalItems = 0,
                message = "Test endpoint working"
            });
        }

        // GET: api/productbatches
    [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductBatches()
    {
        try
        {
                Console.WriteLine("Getting product batches...");
                
                // Check if ProductBatches table exists and has data
                var totalBatches = await _context.ProductBatches.CountAsync();
                Console.WriteLine($"Total product batches: {totalBatches}");

                // If no batches exist, return empty array
                if (totalBatches == 0)
                {
                    Console.WriteLine("No product batches found, returning empty array");
                    return Ok(new { 
                        success = true, 
                        data = new List<object>(),
                        currentPage = 1,
                        totalPages = 1,
                        totalItems = 0,
                        length = 0
                    });
                }

                var batches = await _context.ProductBatches
                .Include(pb => pb.Product)
                .Include(pb => pb.Supplier)
                .Select(pb => new
                {
                    pb.Id,
                    pb.BatchNumber,
                    pb.BatchName,
                    pb.Description,
                    pb.Quantity,
                    pb.RemainingQuantity,
                    pb.InitialQuantity,
                    pb.CurrentQuantity,
                        pb.UnitPrice,
                        pb.TotalValue,
                        pb.ProductionDate,
                        pb.ExpiryDate,
                    pb.Status,
                    pb.QualityStatus,
                        pb.Notes,
                    pb.NgayVe,
                    pb.SoDotVe,
                    pb.SoXeContainerTungDot,
                        pb.NgayVeDetails,
                    pb.CreatedAt,
                        pb.UpdatedAt,
                        ProductId = pb.ProductId,
                        ProductName = pb.Product != null ? pb.Product.ProductName : "N/A",
                        ProductCode = pb.Product != null ? pb.Product.ProductCode : "N/A",
                        SupplierId = pb.SupplierId,
                        SupplierName = pb.Supplier != null ? pb.Supplier.SupplierName : "N/A"
                    })
                    .OrderByDescending(pb => pb.CreatedAt)
                .ToListAsync();

                Console.WriteLine($"Found {batches.Count} product batches");

                return Ok(new { 
                    success = true, 
                    data = batches,
                    currentPage = 1,
                    totalPages = 1,
                    totalItems = batches.Count,
                    length = batches.Count,
                    message = "Lấy danh sách lô hàng thành công"
            });
        }
        catch (Exception ex)
        {
                Console.WriteLine($"Error getting product batches: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy danh sách lô hàng", 
                    error = ex.Message 
                });
            }
        }

        // POST: api/productbatches
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateProductBatch([FromBody] CreateProductBatchRequest request)
    {
        try
        {
                Console.WriteLine("=== CreateProductBatch called ===");
                Console.WriteLine($"Request is null: {request == null}");
                
                if (request == null)
                {
                    Console.WriteLine("Request is null, returning BadRequest");
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                Console.WriteLine($"BatchNumber: '{request.BatchNumber}'");
                Console.WriteLine($"ProductId: {request.ProductId}");
                Console.WriteLine($"BatchName: '{request.BatchName}'");
                Console.WriteLine($"Quantity: {request.Quantity}");

                // Validate required fields
                if (string.IsNullOrEmpty(request.BatchNumber))
                {
                    Console.WriteLine("Validation failed: BatchNumber is empty");
                    return BadRequest(new { success = false, message = "Mã lô hàng là bắt buộc" });
                }
                
                if (request.ProductId <= 0)
                {
                    Console.WriteLine("Validation failed: ProductId is invalid");
                    return BadRequest(new { success = false, message = "Vui lòng chọn sản phẩm" });
                }
                
                if (string.IsNullOrEmpty(request.BatchName))
                {
                    Console.WriteLine("Validation failed: BatchName is empty");
                    return BadRequest(new { success = false, message = "Tên lô hàng là bắt buộc" });
                }
                
                if (request.Quantity <= 0)
                {
                    Console.WriteLine("Validation failed: Quantity is invalid");
                    return BadRequest(new { success = false, message = "Số lượng phải lớn hơn 0" });
            }

            // Check if batch number already exists
            var existingBatch = await _context.ProductBatches
                .FirstOrDefaultAsync(pb => pb.BatchNumber == request.BatchNumber);
            
            if (existingBatch != null)
            {
                    Console.WriteLine("Validation failed: BatchNumber already exists");
                    return BadRequest(new { success = false, message = "Mã lô hàng đã tồn tại" });
                }

                // Check if product exists
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == request.ProductId);
                
            if (product == null)
            {
                    Console.WriteLine("Validation failed: Product does not exist");
                    return BadRequest(new { success = false, message = "Sản phẩm không tồn tại" });
            }

                var productBatch = new ProductBatch
            {
                BatchNumber = request.BatchNumber,
                BatchName = request.BatchName,
                Description = request.Description,
                ProductId = request.ProductId,
                    SupplierId = request.SupplierId,
                Quantity = request.Quantity,
                RemainingQuantity = request.RemainingQuantity ?? request.Quantity,
                InitialQuantity = request.InitialQuantity ?? request.Quantity,
                CurrentQuantity = request.CurrentQuantity ?? request.Quantity,
                    UnitPrice = request.UnitPrice,
                    TotalValue = request.TotalValue,
                    ProductionDate = request.ProductionDate.HasValue ? DateTime.SpecifyKind(request.ProductionDate.Value, DateTimeKind.Utc) : null,
                    ExpiryDate = request.ExpiryDate.HasValue ? DateTime.SpecifyKind(request.ExpiryDate.Value, DateTimeKind.Utc) : null,
                Status = request.Status ?? "Active",
                QualityStatus = request.QualityStatus ?? 1,
                Notes = request.Notes,
                    NgayVe = request.NgayVe.HasValue ? DateTime.SpecifyKind(request.NgayVe.Value, DateTimeKind.Utc) : null,
                SoDotVe = request.SoDotVe,
                SoXeContainerTungDot = request.SoXeContainerTungDot,
                    NgayVeDetails = request.NgayVeDetails,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.ProductBatches.Add(productBatch);
            await _context.SaveChangesAsync();

                // Tính số lượng hiện tại từ các phiếu nhập kho
                await UpdateCurrentQuantityFromImportOrders(productBatch.Id);

            // Return the created batch with related data
            var createdBatch = await _context.ProductBatches
                .Include(pb => pb.Product)
                .Include(pb => pb.Supplier)
                    .Where(pb => pb.Id == productBatch.Id)
                .Select(pb => new
                {
                    pb.Id,
                    pb.BatchNumber,
                    pb.BatchName,
                    pb.Description,
                    pb.Quantity,
                    pb.RemainingQuantity,
                    pb.InitialQuantity,
                    pb.CurrentQuantity,
                        pb.UnitPrice,
                        pb.TotalValue,
                        pb.ProductionDate,
                        pb.ExpiryDate,
                    pb.Status,
                    pb.QualityStatus,
                    pb.Notes,
                        pb.NgayVe,
                        pb.SoDotVe,
                        pb.SoXeContainerTungDot,
                        pb.NgayVeDetails,
                    pb.CreatedAt,
                        pb.UpdatedAt,
                        ProductId = pb.ProductId,
                        ProductName = pb.Product != null ? pb.Product.ProductName : "N/A",
                        ProductCode = pb.Product != null ? pb.Product.ProductCode : "N/A",
                        SupplierId = pb.SupplierId,
                        SupplierName = pb.Supplier != null ? pb.Supplier.SupplierName : "N/A"
                })
                .FirstOrDefaultAsync();

                return Ok(new { success = true, data = createdBatch, message = "Tạo lô hàng thành công" });
        }
        catch (Exception ex)
        {
                Console.WriteLine($"Error creating product batch: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi tạo lô hàng", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/productbatches/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductBatch(int id)
    {
        try
        {
                var batch = await _context.ProductBatches
                .Include(pb => pb.Product)
                .Include(pb => pb.Supplier)
                    .Where(pb => pb.Id == id)
                .Select(pb => new
                {
                    pb.Id,
                    pb.BatchNumber,
                        pb.Quantity,
                        pb.UnitPrice,
                        pb.TotalValue,
                    pb.ProductionDate,
                    pb.ExpiryDate,
                    pb.Status,
                    pb.Notes,
                    pb.CreatedAt,
                        pb.UpdatedAt,
                        ProductId = pb.ProductId,
                        ProductName = pb.Product != null ? pb.Product.ProductName : "N/A",
                        ProductCode = pb.Product != null ? pb.Product.ProductCode : "N/A",
                        SupplierId = pb.SupplierId,
                        SupplierName = pb.Supplier != null ? pb.Supplier.SupplierName : "N/A"
                })
                .FirstOrDefaultAsync();

            if (batch == null)
            {
                    return NotFound(new { success = false, message = "Không tìm thấy lô hàng" });
                }

                return Ok(new { success = true, data = batch });
        }
        catch (Exception ex)
        {
                Console.WriteLine($"Error getting product batch: {ex.Message}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy thông tin lô hàng", 
                    error = ex.Message 
                });
            }
        }

        // Update ProductBatch
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateProductBatch(int id, UpdateProductBatchRequest request)
    {
        try
        {
                var productBatch = await _context.ProductBatches.FindAsync(id);
                if (productBatch == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy lô hàng" });
                }

                // Update fields
                productBatch.BatchName = request.BatchName ?? productBatch.BatchName;
                productBatch.Description = request.Description ?? productBatch.Description;
                productBatch.Status = request.Status ?? productBatch.Status;
                productBatch.QualityStatus = request.QualityStatus ?? productBatch.QualityStatus;
                productBatch.Notes = request.Notes ?? productBatch.Notes;
                productBatch.SoDotVe = request.SoDotVe ?? productBatch.SoDotVe;
                productBatch.SoXeContainerTungDot = request.SoXeContainerTungDot ?? productBatch.SoXeContainerTungDot;
                productBatch.NgayVeDetails = request.NgayVeDetails ?? productBatch.NgayVeDetails;

                if (request.ProductionDate.HasValue)
                {
                    productBatch.ProductionDate = DateTime.SpecifyKind(request.ProductionDate.Value, DateTimeKind.Utc);
                }
                if (request.ExpiryDate.HasValue)
                {
                    productBatch.ExpiryDate = DateTime.SpecifyKind(request.ExpiryDate.Value, DateTimeKind.Utc);
                }
                if (request.NgayVe.HasValue)
                {
                    productBatch.NgayVe = DateTime.SpecifyKind(request.NgayVe.Value, DateTimeKind.Utc);
                }

                productBatch.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Cập nhật lô hàng thành công" });
        }
        catch (Exception ex)
        {
                Console.WriteLine($"Error updating product batch: {ex.Message}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi cập nhật lô hàng", 
                    error = ex.Message 
                });
            }
        }

        // Helper method to update current quantity from import orders
        private async System.Threading.Tasks.Task UpdateCurrentQuantityFromImportOrders(int productBatchId)
    {
        try
        {
                // Tính tổng số lượng từ các phiếu nhập kho
                var totalImportedQuantity = await _context.ImportOrderDetails
                    .Where(iod => iod.ProductBatchId == productBatchId)
                    .SumAsync(iod => iod.ReceivedQuantity ?? iod.Quantity);

                // Cập nhật số lượng hiện tại
                var productBatch = await _context.ProductBatches.FindAsync(productBatchId);
                if (productBatch != null)
                {
                    productBatch.CurrentQuantity = (int)totalImportedQuantity;
                    productBatch.RemainingQuantity = (int)totalImportedQuantity;
                    await _context.SaveChangesAsync();
                }
        }
        catch (Exception ex)
        {
                Console.WriteLine($"Error updating current quantity: {ex.Message}");
            }
        }

        // Debug endpoint to check ProductBatch data
        [HttpGet("debug-batch/{batchId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DebugBatch(int batchId)
    {
        try
        {
                var productBatch = await _context.ProductBatches.FindAsync(batchId);
                if (productBatch == null)
                {
                    return NotFound(new { message = "ProductBatch not found" });
                }

                var importOrderDetails = await _context.ImportOrderDetails
                    .Where(iod => iod.ProductBatchId == batchId)
                    .Select(iod => new {
                        iod.Id,
                        iod.ImportOrderId,
                        iod.ProductId,
                        iod.ProductBatchId,
                        iod.Quantity,
                        iod.ReceivedQuantity,
                        iod.BatchNumber,
                        ImportOrder = new {
                            iod.ImportOrder.OrderNumber,
                            iod.ImportOrder.OrderName,
                            iod.ImportOrder.OrderDate
                        }
                    })
                    .ToListAsync();

                var totalImported = importOrderDetails.Sum(iod => iod.ReceivedQuantity ?? iod.Quantity);

                return Ok(new {
                    ProductBatch = new {
                        productBatch.Id,
                        productBatch.BatchNumber,
                        productBatch.BatchName,
                        productBatch.Quantity,
                        productBatch.CurrentQuantity,
                        productBatch.RemainingQuantity
                    },
                    ImportOrderDetails = importOrderDetails,
                    TotalImported = totalImported,
                    Count = importOrderDetails.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // Force recalculate a specific batch
        [HttpPost("recalculate-batch/{batchId}")]
        [AllowAnonymous]
        public async Task<IActionResult> RecalculateBatch(int batchId)
        {
            try
            {
                await UpdateCurrentQuantityFromImportOrders(batchId);
                return Ok(new { success = true, message = $"Đã cập nhật số lượng cho lô hàng {batchId}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Public method to recalculate all batch quantities
        [HttpPost("recalculate-quantities")]
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<IActionResult> RecalculateAllBatchQuantities()
        {
            try
            {
                var batches = await _context.ProductBatches.ToListAsync();
                
                foreach (var batch in batches)
                {
                    await UpdateCurrentQuantityFromImportOrders(batch.Id);
                }

                return Ok(new { success = true, message = "Đã cập nhật số lượng cho tất cả lô hàng" });
        }
        catch (Exception ex)
        {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật số lượng", error = ex.Message });
        }
    }
}

public class CreateProductBatchRequest
{
    public string BatchNumber { get; set; } = string.Empty;
    public string BatchName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProductId { get; set; }
    public int? SupplierId { get; set; }
    public int Quantity { get; set; }
    public int? RemainingQuantity { get; set; }
    public int? InitialQuantity { get; set; }
    public int? CurrentQuantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalValue { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Status { get; set; }
    public int? QualityStatus { get; set; }
    public string? Notes { get; set; }
    public DateTime? NgayVe { get; set; }
    public int? SoDotVe { get; set; }
    public int? SoXeContainerTungDot { get; set; }
    public string? NgayVeDetails { get; set; }
}
}