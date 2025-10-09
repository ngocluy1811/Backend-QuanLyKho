using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

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
                        totalItems = 0
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
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

            // Validate required fields
                if (string.IsNullOrEmpty(request.BatchNumber) || request.ProductId <= 0)
            {
                    return BadRequest(new { success = false, message = "BatchNumber và ProductId là bắt buộc" });
            }

            // Check if batch number already exists
            var existingBatch = await _context.ProductBatches
                .FirstOrDefaultAsync(pb => pb.BatchNumber == request.BatchNumber);
            
            if (existingBatch != null)
            {
                    return BadRequest(new { success = false, message = "Mã lô hàng đã tồn tại" });
                }

                // Check if product exists
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == request.ProductId);
                
            if (product == null)
            {
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
                    RemainingQuantity = request.RemainingQuantity,
                    InitialQuantity = request.InitialQuantity,
                    CurrentQuantity = request.CurrentQuantity,
                    UnitPrice = request.UnitPrice,
                    TotalValue = request.TotalValue,
                    ProductionDate = request.ProductionDate,
                    ExpiryDate = request.ExpiryDate,
                    Status = request.Status,
                    QualityStatus = request.QualityStatus,
                    Notes = request.Notes,
                    NgayVe = request.NgayVe,
                    SoDotVe = request.SoDotVe,
                    SoXeContainerTungDot = request.SoXeContainerTungDot,
                    NgayVeDetails = request.NgayVeDetails,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.ProductBatches.Add(productBatch);
            await _context.SaveChangesAsync();

            // Return the created batch with related data
            var createdBatch = await _context.ProductBatches
                .Include(pb => pb.Product)
                .Include(pb => pb.Supplier)
                    .Where(pb => pb.Id == productBatch.Id)
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

                return CreatedAtAction(nameof(GetProductBatch), new { id = productBatch.Id }, createdBatch);
        }
        catch (Exception ex)
            {
                Console.WriteLine($"Error creating product batch: {ex.Message}");
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
    }

public class CreateProductBatchRequest
{
    public string BatchNumber { get; set; } = string.Empty;
    public string BatchName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProductId { get; set; }
    public int? SupplierId { get; set; }
    public int Quantity { get; set; }
    public int RemainingQuantity { get; set; }
    public int InitialQuantity { get; set; }
    public int CurrentQuantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalValue { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = "Active";
    public int QualityStatus { get; set; } = 1;
    public string? Notes { get; set; }
    public DateTime? NgayVe { get; set; }
    public int? SoDotVe { get; set; }
    public int? SoXeContainerTungDot { get; set; }
    public string? NgayVeDetails { get; set; }
}
}