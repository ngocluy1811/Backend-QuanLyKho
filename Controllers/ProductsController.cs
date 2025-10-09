using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using System.Linq;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

        // Calculate current stock for a product based on warehouse cells and import/export records
        private decimal CalculateCurrentStock(int productId)
        {
            try
            {
                // Method 1: Calculate from WarehouseCellProducts (actual physical stock)
                var warehouseStock = _context.WarehouseCellProducts
                    .Where(wcp => wcp.ProductId == productId)
                    .Sum(wcp => wcp.Quantity);

                // Method 2: Calculate from Import/Export records (transactional stock)
                // Import orders (OrderType = "Import")
                var totalImported = _context.ImportOrderDetails
                    .Where(iod => iod.ProductId == productId)
                    .Include(iod => iod.ImportOrder)
                    .Where(iod => iod.ImportOrder.OrderType == "Import")
                    .Sum(iod => iod.Quantity);

                // Export orders (OrderType = "Export") + Sales orders
                var totalExportedFromImportOrders = _context.ImportOrderDetails
                    .Where(iod => iod.ProductId == productId)
                    .Include(iod => iod.ImportOrder)
                    .Where(iod => iod.ImportOrder.OrderType == "Export")
                    .Sum(iod => iod.Quantity);

                var totalExportedFromSales = _context.SalesOrderDetails
                    .Where(sod => sod.ProductId == productId)
                    .Sum(sod => sod.Quantity);

                var totalExported = totalExportedFromImportOrders + totalExportedFromSales;
                var transactionalStock = totalImported - totalExported;

                // Use the higher value between warehouse stock and transactional stock
                // This ensures we get the most accurate current stock
                var currentStock = Math.Max(warehouseStock, transactionalStock);

                // Debug log
                Console.WriteLine($"Product {productId}: WarehouseStock={warehouseStock}, Imported={totalImported}, Exported={totalExported}, TransactionalStock={transactionalStock}, FinalStock={currentStock}");

                return currentStock;
            }
            catch (Exception ex)
            {
                // Log error for debugging
                Console.WriteLine($"Error calculating stock for product {productId}: {ex.Message}");
                return 0;
            }
        }

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<object>> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.Products
                .Include(p => p.Company)
                .Include(p => p.SupplierNavigation)
                .Where(p => p.IsActive);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var products = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Calculate current stock for each product based on import/export records
            var productsWithStock = products.Select(p => new
            {
                p.Id,
                p.ProductCode,
                p.ProductName,
                p.Description,
                p.Unit,
                p.UnitPrice,
                p.Price,
                p.MinStockLevel,
                p.MaxStockLevel,
                p.Status,
                p.IsActive,
                p.CreatedAt,
                p.UpdatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Category,
                CompanyId = p.CompanyId,
                CompanyName = p.Company != null ? p.Company.CompanyName : null,
                SupplierId = p.SupplierId,
                SupplierName = p.SupplierNavigation != null ? p.SupplierNavigation.SupplierName : null,
                ProductionDate = p.ProductionDate,
                ExpiryDate = p.ExpiryDate,
                BatchNumber = p.BatchNumber,
                // Calculate current stock from import/export records
                CurrentStock = CalculateCurrentStock(p.Id)
            }).ToList();

            return Ok(new
            {
                products = productsWithStock,
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
            return StatusCode(500, new { message = "An error occurred while fetching products", error = ex.Message });
        }
    }

    // GET: api/Products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetProduct(int id)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Company)
                .Include(p => p.SupplierNavigation)
                .Where(p => p.Id == id && p.IsActive)
                .Select(p => new
                {
                    p.Id,
                    p.ProductCode,
                    p.ProductName,
                    p.Description,
                    p.Unit,
                    p.UnitPrice,
                    p.Price,
                    p.MinStockLevel,
                    p.MaxStockLevel,
                    p.Status,
                    p.IsActive,
                    p.CreatedAt,
                    p.UpdatedAt,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company != null ? p.Company.CompanyName : null,
                    SupplierId = p.SupplierId,
                    SupplierName = p.SupplierNavigation != null ? p.SupplierNavigation.SupplierName : null,
                    ProductionDate = p.ProductionDate,
                    ExpiryDate = p.ExpiryDate,
                    BatchNumber = p.BatchNumber
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching product", error = ex.Message });
        }
    }

    // POST: api/Products
    [HttpPost]
    public async Task<ActionResult<object>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrEmpty(request.ProductCode) || string.IsNullOrEmpty(request.ProductName))
            {
                return BadRequest(new { message = "ProductCode and ProductName are required" });
            }

            // Check if product code already exists
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductCode == request.ProductCode);
            
            if (existingProduct != null)
            {
                return BadRequest(new { message = "Product code already exists" });
            }

            // Get category name if CategoryId is provided
            string categoryName = string.Empty;
            if (request.CategoryId.HasValue)
            {
                var category = await _context.ProductCategories
                    .Where(c => c.Id == request.CategoryId.Value)
                    .Select(c => c.CategoryName)
                    .FirstOrDefaultAsync();
                categoryName = category ?? string.Empty;
            }

            var product = new Product
            {
                ProductCode = request.ProductCode,
                ProductName = request.ProductName,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Category = categoryName,
                UnitPrice = request.UnitPrice,
                Price = request.Price,
                Unit = request.Unit ?? "kg",
                MinStockLevel = request.MinStockLevel,
                MaxStockLevel = request.MaxStockLevel,
                CompanyId = request.CompanyId,
                SupplierId = request.SupplierId,
                Status = request.Status ?? "Active",
                ProductionDate = request.ProductionDate,
                ExpiryDate = request.ExpiryDate,
                BatchNumber = request.BatchNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Return the created product with related data
            var createdProduct = await _context.Products
                .Include(p => p.Company)
                .Include(p => p.SupplierNavigation)
                .Where(p => p.Id == product.Id)
                .Select(p => new
                {
                    p.Id,
                    p.ProductCode,
                    p.ProductName,
                    p.Description,
                    p.Unit,
                    p.UnitPrice,
                    p.Price,
                    p.MinStockLevel,
                    p.MaxStockLevel,
                    p.Status,
                    p.IsActive,
                    p.CreatedAt,
                    p.UpdatedAt,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company != null ? p.Company.CompanyName : null,
                    SupplierId = p.SupplierId,
                    SupplierName = p.SupplierNavigation != null ? p.SupplierNavigation.SupplierName : null,
                    ProductionDate = p.ProductionDate,
                    ExpiryDate = p.ExpiryDate,
                    BatchNumber = p.BatchNumber
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, createdProduct);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating product", error = ex.Message });
        }
    }

    // PUT: api/Products/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<object>> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Check if product code already exists (excluding current product)
            if (!string.IsNullOrEmpty(request.ProductCode) && request.ProductCode != product.ProductCode)
            {
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductCode == request.ProductCode && p.Id != id);
                
                if (existingProduct != null)
                {
                    return BadRequest(new { message = "Product code already exists" });
                }
            }

            // Update product properties
            if (!string.IsNullOrEmpty(request.ProductCode))
                product.ProductCode = request.ProductCode;
            if (!string.IsNullOrEmpty(request.ProductName))
                product.ProductName = request.ProductName;
            if (request.Description != null)
                product.Description = request.Description;
            if (request.CategoryId.HasValue)
            {
                product.CategoryId = request.CategoryId;
                // Update category name
                var category = await _context.ProductCategories
                    .Where(c => c.Id == request.CategoryId.Value)
                    .Select(c => c.CategoryName)
                    .FirstOrDefaultAsync();
                product.Category = category ?? string.Empty;
            }
            if (request.UnitPrice.HasValue)
                product.UnitPrice = request.UnitPrice.Value;
            if (request.Price.HasValue)
                product.Price = request.Price.Value;
            if (!string.IsNullOrEmpty(request.Unit))
                product.Unit = request.Unit;
            if (request.MinStockLevel.HasValue)
                product.MinStockLevel = request.MinStockLevel.Value;
            if (request.MaxStockLevel.HasValue)
                product.MaxStockLevel = request.MaxStockLevel.Value;
            if (request.CompanyId.HasValue)
                product.CompanyId = request.CompanyId;
            if (request.SupplierId.HasValue)
                product.SupplierId = request.SupplierId;
            if (!string.IsNullOrEmpty(request.Status))
                product.Status = request.Status;
            if (request.IsActive.HasValue)
                product.IsActive = request.IsActive.Value;
            if (request.ProductionDate.HasValue)
                product.ProductionDate = request.ProductionDate;
            if (request.ExpiryDate.HasValue)
                product.ExpiryDate = request.ExpiryDate;
            if (request.BatchNumber != null)
                product.BatchNumber = request.BatchNumber;

            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Return the updated product with related data
            var updatedProduct = await _context.Products
                .Include(p => p.Company)
                .Include(p => p.SupplierNavigation)
                .Where(p => p.Id == product.Id)
                .Select(p => new
                {
                    p.Id,
                    p.ProductCode,
                    p.ProductName,
                    p.Description,
                    p.Unit,
                    p.UnitPrice,
                    p.Price,
                    p.MinStockLevel,
                    p.MaxStockLevel,
                    p.Status,
                    p.IsActive,
                    p.CreatedAt,
                    p.UpdatedAt,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company != null ? p.Company.CompanyName : null,
                    SupplierId = p.SupplierId,
                    SupplierName = p.SupplierNavigation != null ? p.SupplierNavigation.SupplierName : null,
                    ProductionDate = p.ProductionDate,
                    ExpiryDate = p.ExpiryDate,
                    BatchNumber = p.BatchNumber
                })
                .FirstOrDefaultAsync();

            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating product", error = ex.Message });
        }
    }

    // DELETE: api/Products/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Soft delete - set IsActive to false
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting product", error = ex.Message });
        }
    }

    // Get stock details for a product
    [HttpGet("{id}/stock-details")]
    public async Task<IActionResult> GetStockDetails(int id)
    {
        try
        {
            var product = await _context.Products
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Get current stock
            var currentStock = CalculateCurrentStock(id);

            // Debug: Get all orders for this product first
            var allOrders = await _context.ImportOrderDetails
                .Where(iod => iod.ProductId == id)
                .Include(iod => iod.ImportOrder)
                .Select(iod => new
                {
                    orderCode = iod.ImportOrder.OrderNumber,
                    orderType = iod.ImportOrder.OrderType,
                    orderDate = iod.ImportOrder.OrderDate,
                    quantity = iod.Quantity,
                    unitPrice = iod.UnitPrice,
                    status = iod.ImportOrder.Status.ToString()
                })
                .ToListAsync();

            // Debug log
            Console.WriteLine($"Product {id} - All orders: {allOrders.Count}");
            foreach (var order in allOrders)
            {
                Console.WriteLine($"Order: {order.orderCode}, Type: {order.orderType}, Qty: {order.quantity}");
            }

            // Get import orders for this product (OrderType = "Import")
            var importOrders = allOrders.Where(o => o.orderType == "Import").ToList();

            // Get export orders for this product (OrderType = "Export")
            var exportOrders = allOrders.Where(o => o.orderType == "Export").ToList();

            // Get sales orders for this product (Sales)
            var salesOrders = await _context.SalesOrderDetails
                .Where(sod => sod.ProductId == id)
                .Include(sod => sod.SalesOrder)
                .Select(sod => new
                {
                    orderCode = sod.SalesOrder.OrderNumber,
                    orderDate = sod.SalesOrder.OrderDate,
                    quantity = sod.Quantity,
                    unitPrice = sod.UnitPrice,
                    status = sod.SalesOrder.Status.ToString()
                })
                .ToListAsync();

            // Get warehouse cells for this product
            var warehouseCells = await _context.WarehouseCellProducts
                .Where(wcp => wcp.ProductId == id)
                .Include(wcp => wcp.WarehouseCell)
                .ThenInclude(wc => wc.Warehouse)
                .Select(wcp => new
                {
                    cellName = wcp.WarehouseCell.CellCode,
                    warehouseName = wcp.WarehouseCell.Warehouse.Name,
                    location = $"{wcp.WarehouseCell.Row}-{wcp.WarehouseCell.Column}",
                    quantity = wcp.Quantity,
                    cellType = wcp.WarehouseCell.CellType,
                    status = wcp.WarehouseCell.Status
                })
                .ToListAsync();

            // Get import orders with warehouse cell details
            var importOrdersWithCells = await _context.ImportOrderDetails
                .Where(iod => iod.ProductId == id)
                .Include(iod => iod.ImportOrder)
                .Select(iod => new
                {
                    orderCode = iod.ImportOrder.OrderNumber,
                    orderDate = iod.ImportOrder.OrderDate,
                    quantity = iod.Quantity,
                    unitPrice = iod.UnitPrice,
                    status = iod.ImportOrder.Status.ToString(),
                    warehouseCells = _context.WarehouseCellProducts
                        .Where(wcp => wcp.ProductId == id)
                        .Include(wcp => wcp.WarehouseCell)
                        .ThenInclude(wc => wc.Warehouse)
                        .Select(wcp => new
                        {
                            cellName = wcp.WarehouseCell.CellCode,
                            warehouseName = wcp.WarehouseCell.Warehouse.Name,
                            location = $"{wcp.WarehouseCell.Row}-{wcp.WarehouseCell.Column}",
                            quantity = wcp.Quantity
                        })
                        .ToList()
                })
                .ToListAsync();

            // Calculate totals
            var totalImported = importOrders.Sum(io => io.quantity);
            var totalExported = exportOrders.Sum(eo => eo.quantity) + salesOrders.Sum(so => so.quantity);

            var stockDetails = new
            {
                currentStock = currentStock,
                totalImported = totalImported,
                totalExported = totalExported,
                importOrders = importOrders,
                exportOrders = exportOrders,
                importOrdersWithCells = importOrdersWithCells,
                salesOrders = salesOrders,
                warehouseCells = warehouseCells
            };

            return Ok(stockDetails);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while getting stock details", error = ex.Message });
        }
    }

    // POST: api/Products/calculate-export-prices
    [HttpPost("calculate-export-prices")]
    public async Task<ActionResult<object>> CalculateExportPrices()
    {
        try
        {
            var results = new List<object>();
            
            // Get all active products
            var products = await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();

            foreach (var product in products)
            {
                // Get all import orders for this product (FIFO - First In, First Out)
                var importOrders = await _context.ImportOrderDetails
                    .Where(iod => iod.ProductId == product.Id)
                    .Include(iod => iod.ImportOrder)
                    .Where(iod => iod.ImportOrder.OrderType == "Import")
                    .OrderBy(iod => iod.ImportOrder.OrderDate) // FIFO: oldest first
                    .Select(iod => new
                    {
                        orderDate = iod.ImportOrder.OrderDate,
                        quantity = iod.Quantity,
                        unitPrice = iod.UnitPrice,
                        remainingQuantity = iod.Quantity // Initially, all quantity is remaining
                    })
                    .ToListAsync();

                // Get all export orders for this product
                var exportOrders = await _context.ImportOrderDetails
                    .Where(iod => iod.ProductId == product.Id)
                    .Include(iod => iod.ImportOrder)
                    .Where(iod => iod.ImportOrder.OrderType == "Export")
                    .OrderBy(iod => iod.ImportOrder.OrderDate)
                    .Select(iod => new
                    {
                        orderDate = iod.ImportOrder.OrderDate,
                        quantity = iod.Quantity,
                        unitPrice = iod.UnitPrice
                    })
                    .ToListAsync();

                // Get sales orders for this product
                var salesOrders = await _context.SalesOrderDetails
                    .Where(sod => sod.ProductId == product.Id)
                    .Include(sod => sod.SalesOrder)
                    .OrderBy(sod => sod.SalesOrder.OrderDate)
                    .Select(sod => new
                    {
                        orderDate = sod.SalesOrder.OrderDate,
                        quantity = sod.Quantity,
                        unitPrice = sod.UnitPrice
                    })
                    .ToListAsync();

                // Calculate weighted average price for remaining stock
                decimal totalImported = importOrders.Sum(io => io.quantity);
                decimal totalExported = exportOrders.Sum(eo => eo.quantity) + salesOrders.Sum(so => so.quantity);
                decimal remainingStock = totalImported - totalExported;

                if (remainingStock > 0 && totalImported > 0)
                {
                    // Calculate weighted average cost
                    decimal totalCost = importOrders.Sum(io => io.quantity * (io.unitPrice ?? 0m));
                    decimal weightedAverageCost = totalCost / totalImported;

                    // Calculate weighted average selling price
                    decimal totalSalesValue = 0;
                    decimal totalSalesQuantity = 0;

                    // Add export orders to sales calculation
                    foreach (var export in exportOrders)
                    {
                        totalSalesValue += export.quantity * (export.unitPrice ?? 0m);
                        totalSalesQuantity += export.quantity;
                    }

                    // Add sales orders to sales calculation
                    foreach (var sales in salesOrders)
                    {
                        totalSalesValue += sales.quantity * sales.unitPrice;
                        totalSalesQuantity += sales.quantity;
                    }

                    decimal weightedAverageSellingPrice = totalSalesQuantity > 0 ? totalSalesValue / totalSalesQuantity : 0;

                    // Update product price with calculated selling price
                    if (weightedAverageSellingPrice > 0)
                    {
                        product.Price = weightedAverageSellingPrice;
                        product.UpdatedAt = DateTime.UtcNow;
                    }

                    results.Add(new
                    {
                        productId = product.Id,
                        productCode = product.ProductCode,
                        productName = product.ProductName,
                        totalImported = totalImported,
                        totalExported = totalExported,
                        remainingStock = remainingStock,
                        weightedAverageCost = weightedAverageCost,
                        weightedAverageSellingPrice = weightedAverageSellingPrice,
                        oldPrice = product.Price,
                        newPrice = weightedAverageSellingPrice > 0 ? weightedAverageSellingPrice : product.Price
                    });
                }
            }

            // Save changes to database
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đã tính giá xuất kho thành công",
                totalProducts = results.Count,
                results = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi tính giá xuất kho", error = ex.Message });
        }
    }
}

// Request DTOs
public class CreateProductRequest
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Price { get; set; }
    public string? Unit { get; set; }
    public int MinStockLevel { get; set; }
    public int MaxStockLevel { get; set; }
    public int? CompanyId { get; set; }
    public int? SupplierId { get; set; }
    public string? Status { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? BatchNumber { get; set; }
}

public class UpdateProductRequest
{
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Price { get; set; }
    public string? Unit { get; set; }
    public int? MinStockLevel { get; set; }
    public int? MaxStockLevel { get; set; }
    public int? CompanyId { get; set; }
    public int? SupplierId { get; set; }
    public string? Status { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? BatchNumber { get; set; }
}