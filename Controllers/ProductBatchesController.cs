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

        // GET: api/productbatches
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductBatches()
        {
            try
            {
                var batches = await _context.ProductBatches
                    .Include(pb => pb.Product)
                    .Include(pb => pb.Supplier)
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
                        pb.CreatedAt,
                        ProductId = pb.ProductId,
                        ProductName = pb.Product != null ? pb.Product.ProductName : "N/A",
                        ProductCode = pb.Product != null ? pb.Product.ProductCode : "N/A",
                        SupplierId = pb.SupplierId,
                        SupplierName = pb.Supplier != null ? pb.Supplier.SupplierName : "N/A"
                    })
                    .OrderByDescending(pb => pb.CreatedAt)
                    .ToListAsync();

                return Ok(new { success = true, data = batches });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting product batches: {ex.Message}");
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
                    ProductId = request.ProductId,
                    SupplierId = request.SupplierId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    TotalValue = request.Quantity * request.UnitPrice,
                    ProductionDate = request.ProductionDate,
                    ExpiryDate = request.ExpiryDate,
                    Status = request.Status ?? "Active",
                    Notes = request.Notes,
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
        public int ProductId { get; set; }
        public int? SupplierId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
    }
}