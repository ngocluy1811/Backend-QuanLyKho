using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportOrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ImportOrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Helper method to recalculate ProductBatch quantity
    private async System.Threading.Tasks.Task RecalculateProductBatchQuantity(int productBatchId)
    {
        try
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
                
                        // Log the calculation (removed to avoid console spam)
            }
        }
        catch (Exception ex)
        {
            // Continue without recalculating if it fails
        }
    }

    // GET: api/ImportOrders
    [HttpGet]
    public async Task<ActionResult<object>> GetImportOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? orderType = null)
    {
        try
        {
            var query = _context.ImportOrders
                .Include(io => io.Warehouse)
                .Include(io => io.Supplier)
                .Include(io => io.Customer)
                .Include(io => io.ImportOrderDetails)
                .Where(io => io.IsActive);

            // Filter by orderType if provided
            if (!string.IsNullOrEmpty(orderType))
            {
                query = query.Where(io => io.OrderType == orderType);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var orders = await query
                .Select(io => new
                {
                    io.Id,
                    io.OrderNumber,
                    io.OrderName,
                    io.OrderDate,
                    io.ExpectedDeliveryDate,
                    io.ActualDeliveryDate,
                    io.WarehouseId,
                    WarehouseName = io.Warehouse.Name,
                    io.SupplierId,
                    SupplierName = io.Supplier != null ? io.Supplier.SupplierName : null,
                    io.CustomerId,
                    io.CustomerName,
                    io.CustomerPhone,
                    io.CustomerAddress,
                    io.ExportReason,
                    io.Status,
                    io.OrderType,
                    io.TotalAmount,
                    io.TotalValue,
                    io.Notes,
                    io.CreatedBy,
                    io.UpdatedBy,
                    io.IsActive,
                    io.CreatedAt,
                    io.UpdatedAt
                })
                .OrderBy(o => o.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagination = new
            {
                currentPage = page,
                pageSize,
                totalCount,
                totalPages,
                hasNextPage = page < totalPages,
                hasPreviousPage = page > 1
            };

            return Ok(new { orders, pagination });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving import orders", error = ex.Message });
        }
    }

    // GET: api/ImportOrders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetImportOrder(int id)
    {
        try
        {
            var order = await _context.ImportOrders
                .Include(io => io.ImportOrderDetails)
                    .ThenInclude(iod => iod.Product)
                .Include(io => io.ImportOrderDetails)
                    .ThenInclude(iod => iod.ProductBatch)
                .Include(io => io.ImportOrderDetails)
                    .ThenInclude(iod => iod.WarehouseCell)
                .FirstOrDefaultAsync(io => io.Id == id && io.IsActive);

            if (order == null)
            {
                return NotFound();
            }

            // Get warehouse and supplier info separately to avoid circular reference
            var warehouse = await _context.Warehouses.FindAsync(order.WarehouseId);
            var supplier = order.SupplierId.HasValue ? await _context.Suppliers.FindAsync(order.SupplierId.Value) : null;

            var result = new
            {
                order.Id,
                order.OrderNumber,
                order.OrderName,
                order.OrderDate,
                order.ExpectedDeliveryDate,
                order.ActualDeliveryDate,
                order.WarehouseId,
                WarehouseName = warehouse?.Name,
                order.SupplierId,
                SupplierName = supplier?.SupplierName,
                order.CustomerId,
                order.CustomerName,
                order.CustomerPhone,
                order.CustomerAddress,
                order.ExportReason,
                order.Status,
                order.OrderType,
                order.TotalAmount,
                order.TotalValue,
                order.Notes,
                order.CreatedBy,
                order.UpdatedBy,
                order.IsActive,
                order.CreatedAt,
                order.UpdatedAt,
                // Thêm tất cả các trường vận chuyển
                order.ContainerNumber,
                order.VehiclePlateNumber,
                order.DriverName,
                order.TotalWeight,
                order.BatchNumber,
                order.BillOfLadingNumber,
                order.ExportTax,
                order.Discount,
                order.PaymentMethod,
                order.ReceiptCode,
                order.ImportDate,
                order.VoucherCode,
                order.TaxRate,
                order.DiscountPercent,
                Details = order.ImportOrderDetails.Select(iod => new
                {
                    iod.Id,
                    iod.ProductId,
                    ProductName = iod.Product?.ProductName ?? "Unknown",
                    iod.ProductBatchId,
                    BatchNumber = iod.ProductBatch?.BatchNumber ?? iod.BatchNumber,
                    iod.WarehouseCellId,
                    CellName = iod.WarehouseCell?.CellCode ?? "Unknown",
                    iod.Quantity,
                    iod.ReceivedQuantity,
                    iod.RemainingQuantity,
                    iod.TotalPrice,
                    iod.ProductionDate,
                    iod.ExpiryDate,
                    iod.Supplier,
                    iod.Unit,
                    iod.Notes,
                    // Thêm các trường mới từ ImportOrderDetail
                    iod.ArrivalDate,
                    iod.ArrivalBatchNumber,
                    iod.ContainerVehicleNumber
                }).ToList()
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving import order", error = ex.Message });
        }
    }

    // POST: api/ImportOrders
    [HttpPost]
    public async Task<ActionResult<ImportOrder>> CreateImportOrder(CreateImportOrderRequest request)
    {
        try
        {
            Console.WriteLine($"=== CreateImportOrder called ===");
            Console.WriteLine($"Request is null: {request == null}");
            
            if (request == null)
            {
                Console.WriteLine("Request is null, returning BadRequest");
                return BadRequest(new { message = "Request body cannot be null" });
            }
            
            Console.WriteLine($"OrderName: '{request.OrderName}'");
            Console.WriteLine($"WarehouseId: {request.WarehouseId}");
            Console.WriteLine($"Details count: {request.Details?.Count ?? 0}");
            
            // Validation
            if (request.WarehouseId <= 0)
            {
                Console.WriteLine("Validation failed: WarehouseId is required");
                return BadRequest(new { message = "WarehouseId is required" });
            }

            if (request.Details == null || !request.Details.Any())
            {
                return BadRequest(new { message = "At least one detail is required" });
            }

            // Validate warehouse exists
            var warehouse = await _context.Warehouses.FindAsync(request.WarehouseId);
            if (warehouse == null)
            {
                return BadRequest(new { message = "Warehouse not found" });
            }
            // Generate order number
            var orderCount = await _context.ImportOrders.CountAsync();
            var orderType = request.OrderType ?? "Import";
            var orderNumber = orderType == "Export" ? 
                $"EXP{DateTime.Now:yyyyMMdd}{orderCount + 1:D4}" : 
                $"IMP{DateTime.Now:yyyyMMdd}{orderCount + 1:D4}";

            var order = new ImportOrder
            {
                OrderNumber = orderNumber,
                OrderName = request.OrderName,
                OrderDate = request.OrderDate,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate,
                ActualDeliveryDate = request.ActualDeliveryDate,
                WarehouseId = request.WarehouseId,
                SupplierId = request.SupplierId,
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerAddress = request.CustomerAddress,
                ExportReason = request.ExportReason,
                CompanyId = request.CompanyId,
                Status = request.Status ?? "Pending",
                OrderType = request.OrderType ?? "Import",
                TotalAmount = request.TotalAmount,
                TotalValue = request.TotalValue,
                ContainerNumber = request.ContainerNumber,
                VehiclePlateNumber = request.VehiclePlateNumber,
                DriverName = request.DriverName,
                TotalWeight = request.TotalWeight,
                BatchNumber = request.BatchNumber,
                BillOfLadingNumber = request.BillOfLadingNumber,
                ExportTax = request.ExportTax,
                Discount = request.Discount,
                PaymentMethod = request.PaymentMethod,
                ReceiptCode = request.ReceiptCode,
                ImportDate = request.ImportDate,
                Notes = request.Notes,
                CreatedBy = request.CreatedBy,
                VoucherCode = request.VoucherCode,
                TaxRate = request.TaxRate,
                DiscountPercent = request.DiscountPercent,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ImportOrders.Add(order);
            await _context.SaveChangesAsync();

            // Add order details
            if (request.Details != null && request.Details.Any())
            {
                Console.WriteLine($"Processing {request.Details.Count} details...");
                try
                {
                    foreach (var detail in request.Details)
                    {
                        try
                        {
                            Console.WriteLine($"Processing detail - ProductId: {detail.ProductId}, WarehouseCellId: {detail.WarehouseCellId}, Quantity: {detail.Quantity}");
                            
                            // Validate detail
                            if (detail.ProductId <= 0)
                            {
                                Console.WriteLine($"Validation failed: ProductId is required for detail");
                                return BadRequest(new { message = "ProductId is required for all details" });
                            }

                    if (detail.WarehouseCellId <= 0)
                    {
                        return BadRequest(new { message = "WarehouseCellId is required for all details" });
                    }

                    if (detail.Quantity <= 0)
                    {
                        return BadRequest(new { message = "Quantity must be greater than 0" });
                    }

                    // Validate product exists
                    var product = await _context.Products
                        .Include(p => p.SupplierNavigation)
                        .FirstOrDefaultAsync(p => p.Id == detail.ProductId);
                    if (product == null)
                    {
                        return BadRequest(new { message = $"Product with ID {detail.ProductId} not found" });
                    }

                    // Validate warehouse cell exists and belongs to the warehouse
                    var warehouseCell = await _context.WarehouseCells
                        .FirstOrDefaultAsync(wc => wc.Id == detail.WarehouseCellId && wc.WarehouseId == request.WarehouseId);
                    if (warehouseCell == null)
                    {
                        return BadRequest(new { message = $"Warehouse cell with ID {detail.WarehouseCellId} not found or doesn't belong to warehouse {request.WarehouseId}" });
                    }

                    // Validate ProductBatchId if provided
                    if (detail.ProductBatchId.HasValue)
                    {
                        var productBatch = await _context.ProductBatches.FindAsync(detail.ProductBatchId.Value);
                        if (productBatch == null)
                        {
                            return BadRequest(new { message = $"ProductBatch with ID {detail.ProductBatchId.Value} not found" });
                        }
                    }
                    var orderDetail = new ImportOrderDetail
                    {
                        ImportOrderId = order.Id,
                        ProductId = detail.ProductId,
                        ProductBatchId = detail.ProductBatchId,
                        WarehouseCellId = detail.WarehouseCellId,
                        Quantity = detail.Quantity,
                        ReceivedQuantity = detail.ReceivedQuantity ?? detail.Quantity,
                        RemainingQuantity = detail.RemainingQuantity ?? detail.Quantity,
                        TotalPrice = detail.TotalPrice,
                        BatchNumber = detail.BatchNumber,
                        ProductionDate = detail.ProductionDate,
                        ExpiryDate = detail.ExpiryDate,
                        Supplier = detail.Supplier,
                        Unit = detail.Unit ?? "kg",
                        ArrivalDate = detail.ArrivalDate,
                        ArrivalBatchNumber = detail.ArrivalBatchNumber,
                        ContainerVehicleNumber = detail.ContainerVehicleNumber,
                        Notes = detail.Notes,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.ImportOrderDetails.Add(orderDetail);

                    // Update WarehouseCell with product information (warehouseCell already validated above)
                    if (warehouseCell != null)
                    {
                        // Debug log to check WarehouseCell update
                        Console.WriteLine($"Updating WarehouseCell - OrderType: {order.OrderType}, CellId: {detail.WarehouseCellId}, CurrentAmount before: {warehouseCell.CurrentAmount}, Quantity: {detail.Quantity}");
                        
                        // Update quantity - add for import, subtract for export
                        if (order.OrderType == "Export")
                        {
                            warehouseCell.CurrentAmount -= detail.Quantity;
                            // Ensure CurrentAmount doesn't go below 0
                            if (warehouseCell.CurrentAmount < 0) warehouseCell.CurrentAmount = 0;
                            Console.WriteLine($"Export: CurrentAmount after: {warehouseCell.CurrentAmount}");
                        }
                        else
                        {
                            warehouseCell.CurrentAmount += detail.Quantity;
                            Console.WriteLine($"Import: CurrentAmount after: {warehouseCell.CurrentAmount}");
                        }
                        
                        // Update product information
                        warehouseCell.ProductId = detail.ProductId;
                        warehouseCell.ProductName = product.ProductName;
                        warehouseCell.BatchNumber = detail.BatchNumber;
                        warehouseCell.ProductionDate = detail.ProductionDate;
                        warehouseCell.ExpiryDate = detail.ExpiryDate;
                        warehouseCell.Supplier = detail.Supplier;
                        
                        warehouseCell.UpdatedAt = DateTime.UtcNow;
                    }

                    // Create or update WarehouseCellProduct for detailed product tracking
                    if (order.OrderType == "Import")
                    {
                        var existingCellProduct = await _context.WarehouseCellProducts
                            .FirstOrDefaultAsync(wcp => wcp.WarehouseCellId == detail.WarehouseCellId 
                                && wcp.ProductId == detail.ProductId 
                                && wcp.BatchNumber == detail.BatchNumber
                                && wcp.IsActive == true);

                        if (existingCellProduct != null)
                        {
                            // Update existing product in cell
                            existingCellProduct.Quantity += detail.Quantity;
                            existingCellProduct.RemainingQuantity += detail.Quantity;
                            // existingCellProduct.TotalPrice = existingCellProduct.TotalPrice;
                            
                            // Get ProductBatch info for updating missing fields
                            var productBatch = await _context.ProductBatches
                                .Include(pb => pb.Supplier)
                                .FirstOrDefaultAsync(pb => pb.BatchNumber == detail.BatchNumber && pb.ProductId == detail.ProductId);
                            
                            Console.WriteLine($"Update existing - Looking for ProductBatch - BatchNumber: {detail.BatchNumber}, ProductId: {detail.ProductId}");
                            if (productBatch != null)
                            {
                                Console.WriteLine($"Update existing - Found ProductBatch - ProductionDate: {productBatch.ProductionDate}, ExpiryDate: {productBatch.ExpiryDate}, SupplierId: {productBatch.SupplierId}, Supplier: {productBatch.Supplier?.SupplierName}");
                            Console.WriteLine($"Update existing - Product data - ProductionDate: {product.ProductionDate}, ExpiryDate: {product.ExpiryDate}, SupplierId: {product.SupplierId}, Supplier: {product.SupplierNavigation?.SupplierName}");
                            }
                            else
                            {
                                Console.WriteLine("Update existing - No ProductBatch found - will use Product data");
                            }
                            
                            // Update product info - priority: detail -> product -> existing
                            existingCellProduct.Supplier = !string.IsNullOrEmpty(detail.Supplier) ? detail.Supplier : (product.SupplierNavigation?.SupplierName ?? existingCellProduct.Supplier);
                            existingCellProduct.ProductionDate = detail.ProductionDate ?? product.ProductionDate ?? existingCellProduct.ProductionDate;
                            existingCellProduct.ExpiryDate = detail.ExpiryDate ?? product.ExpiryDate ?? existingCellProduct.ExpiryDate;
                            
                            existingCellProduct.UpdatedAt = DateTime.UtcNow;
                            existingCellProduct.UpdatedBy = "System";
                            
                            Console.WriteLine($"Updated WarehouseCellProduct with:");
                            Console.WriteLine($"  - Detail ProductionDate: {detail.ProductionDate}");
                            Console.WriteLine($"  - Detail ExpiryDate: {detail.ExpiryDate}");
                            Console.WriteLine($"  - Detail Supplier: {detail.Supplier}");
                            Console.WriteLine($"  - Product ProductionDate: {product.ProductionDate}");
                            Console.WriteLine($"  - Product ExpiryDate: {product.ExpiryDate}");
                            Console.WriteLine($"  - Product Supplier: {product.SupplierNavigation?.SupplierName}");
                            Console.WriteLine($"  - Final ProductionDate: {existingCellProduct.ProductionDate}");
                            Console.WriteLine($"  - Final ExpiryDate: {existingCellProduct.ExpiryDate}");
                            Console.WriteLine($"  - Final Supplier: {existingCellProduct.Supplier}");
                        }
                        else
                        {
                            // Get ProductBatch info for production date, expiry date, and supplier
                            var productBatch = await _context.ProductBatches
                                .Include(pb => pb.Supplier)
                                .FirstOrDefaultAsync(pb => pb.BatchNumber == detail.BatchNumber && pb.ProductId == detail.ProductId);
                            
                            Console.WriteLine($"Looking for ProductBatch - BatchNumber: {detail.BatchNumber}, ProductId: {detail.ProductId}");
                            
                            // Debug: List all ProductBatches for this ProductId
                            var allBatches = await _context.ProductBatches
                                .Include(pb => pb.Supplier)
                                .Where(pb => pb.ProductId == detail.ProductId)
                                .ToListAsync();
                            Console.WriteLine($"All ProductBatches for ProductId {detail.ProductId}:");
                            foreach (var batch in allBatches)
                            {
                                Console.WriteLine($"  - BatchNumber: '{batch.BatchNumber}', ProductionDate: {batch.ProductionDate}, ExpiryDate: {batch.ExpiryDate}, Supplier: {batch.Supplier?.SupplierName}");
                            }
                            
                            if (productBatch != null)
                            {
                                Console.WriteLine($"Found ProductBatch - ProductionDate: {productBatch.ProductionDate}, ExpiryDate: {productBatch.ExpiryDate}, SupplierId: {productBatch.SupplierId}, Supplier: {productBatch.Supplier?.SupplierName}");
                            Console.WriteLine($"Product data - ProductionDate: {product.ProductionDate}, ExpiryDate: {product.ExpiryDate}, SupplierId: {product.SupplierId}, Supplier: {product.SupplierNavigation?.SupplierName}");
                            }
                            else
                            {
                                Console.WriteLine("No ProductBatch found - will use Product data");
                            }

                            // Create new product in cell
                            var cellProduct = new WarehouseCellProduct
                            {
                                WarehouseCellId = detail.WarehouseCellId,
                                ProductId = detail.ProductId,
                                ProductName = product.ProductName,
                                BatchNumber = detail.BatchNumber,
                                Quantity = detail.Quantity,
                                RemainingQuantity = detail.Quantity,
                                TotalPrice = detail.TotalPrice ?? (detail.UnitPrice ?? 0) * detail.Quantity,
                                UnitPrice = detail.UnitPrice ?? (detail.TotalPrice ?? 0) / (detail.Quantity > 0 ? detail.Quantity : 1),
                                // Priority: detail -> ProductBatch -> product
                                ProductionDate = detail.ProductionDate ?? product.ProductionDate,
                                ExpiryDate = detail.ExpiryDate ?? product.ExpiryDate,
                                Supplier = !string.IsNullOrEmpty(detail.Supplier) ? detail.Supplier : product.SupplierNavigation?.SupplierName,
                                Unit = detail.Unit ?? "kg",
                                ArrivalDate = detail.ArrivalDate,
                                ArrivalBatchNumber = detail.ArrivalBatchNumber?.ToString(),
                                ContainerVehicleNumber = detail.ContainerVehicleNumber?.ToString(),
                                Notes = detail.Notes,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = "System"
                            };
                            
                            Console.WriteLine($"Creating WarehouseCellProduct with:");
                            Console.WriteLine($"  - Detail ProductionDate: {detail.ProductionDate}");
                            Console.WriteLine($"  - Detail ExpiryDate: {detail.ExpiryDate}");
                            Console.WriteLine($"  - Detail Supplier: {detail.Supplier}");
                            Console.WriteLine($"  - Product ProductionDate: {product.ProductionDate}");
                            Console.WriteLine($"  - Product ExpiryDate: {product.ExpiryDate}");
                            Console.WriteLine($"  - Product Supplier: {product.SupplierNavigation?.SupplierName}");
                            Console.WriteLine($"  - Final ProductionDate: {cellProduct.ProductionDate}");
                            Console.WriteLine($"  - Final ExpiryDate: {cellProduct.ExpiryDate}");
                            Console.WriteLine($"  - Final Supplier: {cellProduct.Supplier}");

                            _context.WarehouseCellProducts.Add(cellProduct);
                        }
                    }
                    else if (order.OrderType == "Export")
                    {
                        // Debug: Log all WarehouseCellProducts for this cell and product
                        var allCellProducts = await _context.WarehouseCellProducts
                            .Where(wcp => wcp.WarehouseCellId == detail.WarehouseCellId && wcp.ProductId == detail.ProductId)
                            .ToListAsync();
                        
                        Console.WriteLine($"All WarehouseCellProducts for CellId: {detail.WarehouseCellId}, ProductId: {detail.ProductId}:");
                        foreach (var cp in allCellProducts)
                        {
                            Console.WriteLine($"  - ID: {cp.Id}, BatchNumber: '{cp.BatchNumber}', Quantity: {cp.Quantity}, IsActive: {cp.IsActive}");
                        }
                        Console.WriteLine($"Looking for BatchNumber: '{detail.BatchNumber}'");

                        // Find and update existing product in cell for export
                        // First try to find exact match with batch number
                        Console.WriteLine($"Looking for WarehouseCellProduct - CellId: {detail.WarehouseCellId}, ProductId: {detail.ProductId}, BatchNumber: '{detail.BatchNumber}'");
                        
                        var existingCellProduct = await _context.WarehouseCellProducts
                            .FirstOrDefaultAsync(wcp => wcp.WarehouseCellId == detail.WarehouseCellId 
                                && wcp.ProductId == detail.ProductId 
                                && wcp.BatchNumber == detail.BatchNumber
                                && wcp.IsActive == true);
                        
                        // If no exact match, try to find by product and cell only
                        if (existingCellProduct == null)
                        {
                            Console.WriteLine($"No exact batch match found, trying product and cell only...");
                            existingCellProduct = await _context.WarehouseCellProducts
                                .FirstOrDefaultAsync(wcp => wcp.WarehouseCellId == detail.WarehouseCellId 
                                    && wcp.ProductId == detail.ProductId 
                                    && wcp.IsActive == true);
                        }

                        if (existingCellProduct != null)
                        {
                            Console.WriteLine($"Found WarehouseCellProduct for export - CellId: {detail.WarehouseCellId}, ProductId: {detail.ProductId}, BatchNumber: {detail.BatchNumber}, Current Quantity: {existingCellProduct.Quantity}, Export Quantity: {detail.Quantity}");
                            
                            // Validate sufficient quantity
                            if (existingCellProduct.Quantity < detail.Quantity)
                            {
                                return BadRequest(new { message = $"Không đủ số lượng để xuất. Số lượng hiện có: {existingCellProduct.Quantity}, Số lượng yêu cầu: {detail.Quantity}" });
                            }
                            
                            // Update quantities for export
                            existingCellProduct.Quantity -= detail.Quantity;
                            existingCellProduct.RemainingQuantity -= detail.Quantity;
                            
                            // existingCellProduct.TotalPrice = existingCellProduct.TotalPrice;
                            existingCellProduct.UpdatedAt = DateTime.UtcNow;
                            existingCellProduct.UpdatedBy = "System";

                            // Deactivate if quantity is 0
                            if (existingCellProduct.Quantity == 0)
                            {
                                existingCellProduct.IsActive = false;
                            }
                            
                            Console.WriteLine($"Updated WarehouseCellProduct - New Quantity: {existingCellProduct.Quantity}, IsActive: {existingCellProduct.IsActive}");
                        }
                        else
                        {
                            Console.WriteLine($"No WarehouseCellProduct found for export - CellId: {detail.WarehouseCellId}, ProductId: {detail.ProductId}, BatchNumber: {detail.BatchNumber}");
                            
                            // For export, if no WarehouseCellProduct found, we need to check if there's enough quantity in the cell
                            // Get the product info
                            var productInfo = await _context.Products.FindAsync(detail.ProductId);
                            if (productInfo == null)
                            {
                                return BadRequest(new { message = $"Không tìm thấy sản phẩm với ID {detail.ProductId}" });
                            }
                            
                            // Check if there's enough quantity in the cell by looking at all WarehouseCellProducts for this cell and product
                            // Sum all quantities (including negative ones for exports) to get the actual available quantity
                            var allCellProductsForExport = await _context.WarehouseCellProducts
                                .Where(wcp => wcp.WarehouseCellId == detail.WarehouseCellId 
                                    && wcp.ProductId == detail.ProductId)
                                .ToListAsync();
                            
                            Console.WriteLine($"All WarehouseCellProducts for CellId: {detail.WarehouseCellId}, ProductId: {detail.ProductId}:");
                            foreach (var cp in allCellProductsForExport)
                            {
                                Console.WriteLine($"  - ID: {cp.Id}, Quantity: {cp.Quantity}, IsActive: {cp.IsActive}, BatchNumber: '{cp.BatchNumber}'");
                            }
                            
                            var totalQuantityInCell = allCellProductsForExport
                                .Where(wcp => wcp.IsActive == true)
                                .Sum(wcp => wcp.Quantity);
                            
                            Console.WriteLine($"Total quantity in cell for ProductId {detail.ProductId}: {totalQuantityInCell}, Export quantity: {detail.Quantity}");
                            
                            // For export, if no WarehouseCellProducts exist, we still allow the export
                            // but we need to check if there are any ImportOrderDetails for this product in this cell
                            if (totalQuantityInCell < detail.Quantity)
                            {
                                // Check if there are any ImportOrderDetails for this product in this cell
                                var importOrderDetails = await _context.ImportOrderDetails
                                    .Where(iod => iod.WarehouseCellId == detail.WarehouseCellId 
                                        && iod.ProductId == detail.ProductId 
                                        && iod.ImportOrder.IsActive == true
                                        && iod.ImportOrder.OrderType == "Import")
                                    .SumAsync(iod => iod.Quantity);
                                
                                Console.WriteLine($"ImportOrderDetails quantity for ProductId {detail.ProductId}: {importOrderDetails}");
                                
                                if (importOrderDetails < detail.Quantity)
                                {
                                    return BadRequest(new { message = $"Không đủ số lượng để xuất. Tổng số lượng trong ô: {Math.Max(totalQuantityInCell, importOrderDetails)}, Số lượng yêu cầu: {detail.Quantity}" });
                                }
                                
                                Console.WriteLine($"Allowing export based on ImportOrderDetails quantity: {importOrderDetails}");
                            }
                            
                            // Create a new WarehouseCellProduct with negative quantity to represent export
                            var exportCellProduct = new WarehouseCellProduct
                            {
                                WarehouseCellId = detail.WarehouseCellId,
                                ProductId = detail.ProductId,
                                ProductName = productInfo.ProductName,
                                Quantity = -detail.Quantity, // Negative quantity for export
                                RemainingQuantity = 0,
                                TotalPrice = detail.TotalPrice ?? (detail.UnitPrice ?? 0) * detail.Quantity,
                                UnitPrice = detail.UnitPrice ?? (detail.TotalPrice ?? 0) / (detail.Quantity > 0 ? detail.Quantity : 1),
                                BatchNumber = detail.BatchNumber,
                                ProductionDate = detail.ProductionDate,
                                ExpiryDate = detail.ExpiryDate,
                                Supplier = detail.Supplier,
                                Unit = detail.Unit,
                                ArrivalDate = detail.ArrivalDate,
                                ArrivalBatchNumber = detail.ArrivalBatchNumber?.ToString(),
                                ContainerVehicleNumber = detail.ContainerVehicleNumber?.ToString(),
                                Notes = detail.Notes,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = "System",
                                UpdatedAt = DateTime.UtcNow,
                                UpdatedBy = "System"
                            };
                            
                            _context.WarehouseCellProducts.Add(exportCellProduct);
                            Console.WriteLine($"Created export WarehouseCellProduct with negative quantity: {exportCellProduct.Quantity}");
                        }
                    }

                    // Update ProductBatch quantity if ProductBatchId is provided
                    if (detail.ProductBatchId.HasValue)
                    {
                        await RecalculateProductBatchQuantity(detail.ProductBatchId.Value);
                    }

                    // Create WarehouseActivity - Skip if there are issues
                    try
                    {
                        // Get the correct batch number from ProductBatch if available
                        string batchNumber = detail.BatchNumber;
                        if (detail.ProductBatchId.HasValue)
                        {
                            var productBatch = await _context.ProductBatches.FindAsync(detail.ProductBatchId.Value);
                            if (productBatch != null)
                            {
                                batchNumber = productBatch.BatchNumber;
                            }
                        }

                        // Debug log to check OrderType
                        Console.WriteLine($"Creating WarehouseActivity - OrderType: {order.OrderType}, ProductId: {detail.ProductId}, Quantity: {detail.Quantity}");

                        var activity = new WarehouseActivity
                        {
                            WarehouseId = order.WarehouseId,
                            CellId = detail.WarehouseCellId,
                            ActivityType = order.OrderType == "Export" ? "Export" : "Import",
                            Description = order.OrderType == "Export" ? 
                                $"Xuất hàng: {detail.Quantity} {detail.Unit ?? "kg"}" : 
                                $"Nhập hàng: {detail.Quantity} {detail.Unit ?? "kg"}",
                            ProductName = product.ProductName,
                            BatchNumber = batchNumber,
                            Quantity = detail.Quantity,
                            Unit = detail.Unit ?? "kg",
                            UserName = order.CreatedBy ?? "System",
                            Timestamp = DateTime.UtcNow,
                            Status = "Completed",
                            Notes = order.OrderType == "Export" ? 
                                $"Phiếu xuất: {order.OrderNumber}" : 
                                $"Phiếu nhập: {order.OrderNumber}",
                            IsActive = true
                        };

                        _context.WarehouseActivities.Add(activity);
                    }
                    catch (Exception ex)
                    {
                        // Continue without activity if it fails - don't throw exception
                    }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing detail: {ex.Message}");
                            Console.WriteLine($"Stack trace: {ex.StackTrace}");
                            // Continue with next detail
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing details: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    // Continue without details if it fails
                }

                // Calculate and update TotalValue from order details
                var calculatedTotalValue = request.Details?.Sum(detail => detail.TotalPrice ?? 0) ?? 0;
                Console.WriteLine($"Calculating TotalValue - OrderId: {order.Id}, Details count: {request.Details?.Count ?? 0}, CalculatedTotalValue: {calculatedTotalValue}");
                order.TotalValue = calculatedTotalValue;

                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetImportOrder), new { id = order.Id }, new { 
                id = order.Id,
                orderNumber = order.OrderNumber,
                orderName = order.OrderName,
                orderDate = order.OrderDate,
                warehouseId = order.WarehouseId,
                status = order.Status,
                orderType = order.OrderType,
                totalAmount = order.TotalAmount,
                totalValue = order.TotalValue,
                paymentMethod = order.PaymentMethod,
                notes = order.Notes,
                createdAt = order.CreatedAt
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating import order: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, new { 
                message = "An error occurred while creating import order", 
                error = ex.Message,
                stackTrace = ex.StackTrace
            });
        }
    }

    // PUT: api/ImportOrders/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateImportOrder(int id, UpdateImportOrderRequest request)
    {
        try
        {
            var order = await _context.ImportOrders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(request.OrderName))
                order.OrderName = request.OrderName;
            if (request.OrderDate.HasValue)
                order.OrderDate = request.OrderDate.Value;
            if (request.ExpectedDeliveryDate.HasValue)
                order.ExpectedDeliveryDate = request.ExpectedDeliveryDate;
            if (request.ActualDeliveryDate.HasValue)
                order.ActualDeliveryDate = request.ActualDeliveryDate;
            if (request.WarehouseId.HasValue)
                order.WarehouseId = request.WarehouseId.Value;
            if (request.SupplierId.HasValue)
                order.SupplierId = request.SupplierId.Value;
            if (request.CustomerId.HasValue)
                order.CustomerId = request.CustomerId.Value;
            if (!string.IsNullOrEmpty(request.CustomerName))
                order.CustomerName = request.CustomerName;
            if (!string.IsNullOrEmpty(request.CustomerPhone))
                order.CustomerPhone = request.CustomerPhone;
            if (!string.IsNullOrEmpty(request.CustomerAddress))
                order.CustomerAddress = request.CustomerAddress;
            if (!string.IsNullOrEmpty(request.ExportReason))
                order.ExportReason = request.ExportReason;
            if (!string.IsNullOrEmpty(request.Status))
                order.Status = request.Status;
            if (!string.IsNullOrEmpty(request.OrderType))
                order.OrderType = request.OrderType;
            if (request.TotalAmount.HasValue)
                order.TotalAmount = request.TotalAmount;
            if (request.TotalValue.HasValue)
                order.TotalValue = request.TotalValue;
            if (!string.IsNullOrEmpty(request.Notes))
                order.Notes = request.Notes;
            if (!string.IsNullOrEmpty(request.PaymentMethod))
                order.PaymentMethod = request.PaymentMethod;
            if (!string.IsNullOrEmpty(request.UpdatedBy))
                order.UpdatedBy = request.UpdatedBy;

            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating import order", error = ex.Message });
        }
    }

    // DELETE: api/ImportOrders/5 (Soft Delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteImportOrder(int id)
    {
        try
        {
            var order = await _context.ImportOrders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.IsActive = false;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting import order", error = ex.Message });
        }
    }

    // GET: api/ImportOrders/warehouses
    [HttpGet("warehouses")]
    public async Task<ActionResult<IEnumerable<object>>> GetWarehouses()
    {
        try
        {
            var warehouses = await _context.Warehouses
                .Where(w => w.IsActive)
                .Select(w => new
                {
                    w.Id,
                    WarehouseName = w.Name,
                    WarehouseCode = w.Name, // Using Name as code since there's no WarehouseCode
                    w.Address,
                    w.Status
                })
                .ToListAsync();

            return Ok(warehouses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving warehouses", error = ex.Message });
        }
    }

    // GET: api/ImportOrders/warehouse-cells/{warehouseId}
    [HttpGet("warehouse-cells/{warehouseId}")]
    public async Task<ActionResult<IEnumerable<object>>> GetWarehouseCells(int warehouseId)
    {
        try
        {
            var cells = await _context.WarehouseCells
                .Where(wc => wc.WarehouseId == warehouseId && wc.IsActive)
                .Select(wc => new
                {
                    wc.Id,
                    CellName = wc.CellCode,
                    wc.CellCode,
                    Capacity = wc.CurrentAmount > 0 ? (int)Math.Round((double)wc.CurrentAmount / wc.MaxCapacity * 100) : 0,
                    MaxCapacity = wc.MaxCapacity,
                    CurrentAmount = wc.CurrentAmount,
                    wc.Status,
                    // Add more detailed information
                    wc.ProductName,
                    wc.BatchNumber,
                    wc.ProductionDate,
                    wc.ExpiryDate,
                    wc.Supplier,
                    LastUpdated = wc.LastMoved,
                    // Environment information - using default values since these fields don't exist in the model
                    Temperature = 25,
                    Humidity = 60,
                    Ventilation = "Normal",
                    SensorStatus = "Active",
                    ElectronicScale = "Calibrated",
                    // Staff information
                    StaffResponsible = wc.AssignedStaff,
                    // Activities - get recent activities for this cell
                    Activities = _context.WarehouseActivities
                        .Where(wa => wa.CellId == wc.Id)
                        .OrderByDescending(wa => wa.Timestamp)
                        .Take(5)
                        .Select(wa => new
                        {
                            wa.Id,
                            wa.ActivityType,
                            wa.Description,
                            CreatedAt = wa.Timestamp,
                            CreatedBy = wa.UserName ?? "System"
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(cells);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving warehouse cells", error = ex.Message });
        }
    }

    // GET: api/importorders/cell-products/{cellId}
        [HttpGet("debug-productbatches/{productId}")]
        public async Task<IActionResult> DebugProductBatches(int productId)
        {
            try
            {
                var productBatches = await _context.ProductBatches
                    .Where(pb => pb.ProductId == productId)
                    .Include(pb => pb.Supplier)
                    .ToListAsync();
                
                var result = productBatches.Select(pb => new
                {
                    Id = pb.Id,
                    BatchNumber = pb.BatchNumber,
                    ProductId = pb.ProductId,
                    ProductionDate = pb.ProductionDate,
                    ExpiryDate = pb.ExpiryDate,
                    Supplier = pb.Supplier?.SupplierName,
                    CreatedAt = pb.CreatedAt
                }).ToList();
                
                return Ok(new { 
                    message = $"Found {result.Count} ProductBatches for ProductId {productId}",
                    data = result 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting ProductBatches", error = ex.Message });
            }
        }

        [HttpGet("debug-product/{productId}")]
        public async Task<IActionResult> DebugProduct(int productId)
        {
            try
            {
                var product = await _context.Products
                    .Where(p => p.Id == productId)
                    .Include(p => p.SupplierNavigation)
                    .FirstOrDefaultAsync();
                
                if (product == null)
                {
                    return NotFound(new { message = $"Product with ID {productId} not found" });
                }
                
                var result = new
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    ProductionDate = product.ProductionDate,
                    ExpiryDate = product.ExpiryDate,
                    SupplierId = product.SupplierId,
                    Supplier = product.SupplierNavigation?.SupplierName,
                    BatchNumber = product.BatchNumber
                };
                
                return Ok(new { 
                    message = $"Product data for ProductId {productId}",
                    data = result 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting Product", error = ex.Message });
            }
        }

        [HttpGet("cell-products/{cellId}")]
        public async Task<IActionResult> GetCellProducts(int cellId)
    {
        try
        {
            // First try to get from WarehouseCellProducts with ImportOrderDetails info
            var warehouseCellProducts = await _context.WarehouseCellProducts
                .Where(wcp => wcp.WarehouseCellId == cellId && wcp.IsActive && wcp.Quantity > 0)
                .Select(wcp => new
                {
                    Id = wcp.Id,
                    ProductId = wcp.ProductId,
                    ProductName = wcp.ProductName,
                    Quantity = wcp.Quantity,
                    RemainingQuantity = wcp.RemainingQuantity,
                    UnitPrice = wcp.UnitPrice,
                    TotalPrice = wcp.TotalPrice,
                    BatchNumber = wcp.BatchNumber,
                    ProductionDate = wcp.ProductionDate,
                    ExpiryDate = wcp.ExpiryDate,
                    Supplier = wcp.Supplier,
                    Unit = wcp.Unit,
                    ArrivalDate = wcp.ArrivalDate,
                    ArrivalBatchNumber = wcp.ArrivalBatchNumber,
                    ContainerVehicleNumber = wcp.ContainerVehicleNumber,
                    Notes = wcp.Notes,
                    // Get OrderNumber and OrderDate from related ImportOrderDetails
                    OrderNumber = _context.ImportOrderDetails
                        .Where(iod => iod.WarehouseCellId == cellId && iod.ProductId == wcp.ProductId && iod.ImportOrder.IsActive && iod.ImportOrder.OrderType == "Import")
                        .OrderByDescending(iod => iod.ImportOrder.OrderDate)
                        .Select(iod => iod.ImportOrder.OrderNumber)
                        .FirstOrDefault(),
                    OrderDate = _context.ImportOrderDetails
                        .Where(iod => iod.WarehouseCellId == cellId && iod.ProductId == wcp.ProductId && iod.ImportOrder.IsActive && iod.ImportOrder.OrderType == "Import")
                        .OrderByDescending(iod => iod.ImportOrder.OrderDate)
                        .Select(iod => iod.ImportOrder.OrderDate)
                        .FirstOrDefault(),
                    CreatedAt = wcp.CreatedAt,
                    UpdatedAt = wcp.UpdatedAt,
                    CreatedBy = wcp.CreatedBy,
                    UpdatedBy = wcp.UpdatedBy
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            if (warehouseCellProducts.Any())
            {
                return Ok(warehouseCellProducts);
            }

            // Fallback to ImportOrderDetails if no WarehouseCellProducts - only Import orders
            var cellProducts = await _context.ImportOrderDetails
                .Where(iod => iod.WarehouseCellId == cellId && iod.ImportOrder.IsActive && iod.ImportOrder.OrderType == "Import")
                .Include(iod => iod.Product)
                    .ThenInclude(p => p.SupplierNavigation)
                .Include(iod => iod.ImportOrder)
                .Select(iod => new
                {
                    Id = iod.Id,
                    ProductId = iod.ProductId,
                    ProductName = iod.Product != null ? iod.Product.ProductName : "Unknown",
                    Quantity = iod.Quantity,
                    RemainingQuantity = iod.RemainingQuantity,
                    TotalPrice = iod.TotalPrice,
                    BatchNumber = iod.BatchNumber,
                    ProductionDate = iod.Product != null ? iod.Product.ProductionDate : null,
                    ExpiryDate = iod.Product != null ? iod.Product.ExpiryDate : null,
                    Supplier = iod.Product != null && iod.Product.SupplierNavigation != null ? iod.Product.SupplierNavigation.SupplierName : null,
                    Unit = iod.Unit,
                    ArrivalDate = iod.ArrivalDate,
                    ArrivalBatchNumber = iod.ArrivalBatchNumber,
                    ContainerVehicleNumber = iod.ContainerVehicleNumber,
                    Notes = iod.Notes,
                    OrderNumber = iod.ImportOrder.OrderNumber,
                    OrderDate = iod.ImportOrder.OrderDate,
                    CreatedAt = iod.ImportOrder.CreatedAt,
                    UpdatedAt = iod.ImportOrder.UpdatedAt,
                    CreatedBy = iod.ImportOrder.CreatedBy,
                    UpdatedBy = iod.ImportOrder.UpdatedBy
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(cellProducts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving cell products", error = ex.Message });
        }
    }
}

// Request DTOs
public class CreateImportOrderRequest
{
    [Required]
    [MaxLength(255)]
    public string OrderName { get; set; } = string.Empty;
    
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    
    [Required]
    public int WarehouseId { get; set; }
    public int? SupplierId { get; set; }
    
    // Thông tin khách hàng (cho phiếu xuất)
    public int? CustomerId { get; set; }
    [MaxLength(255)]
    public string? CustomerName { get; set; }
    [MaxLength(20)]
    public string? CustomerPhone { get; set; }
    [MaxLength(500)]
    public string? CustomerAddress { get; set; }
    [MaxLength(255)]
    public string? ExportReason { get; set; }
    
    public int? CompanyId { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
    
    [MaxLength(50)]
    public string? OrderType { get; set; }
    
    public decimal? TotalAmount { get; set; }
    public decimal? TotalValue { get; set; }
    
    // Thông tin vận chuyển
    [MaxLength(50)]
    public string? ContainerNumber { get; set; }
    
    [MaxLength(50)]
    public string? VehiclePlateNumber { get; set; }
    
    [MaxLength(50)]
    public string? DriverName { get; set; }
    
    public decimal? TotalWeight { get; set; }
    
    [MaxLength(50)]
    public string? BatchNumber { get; set; }
    
    [MaxLength(50)]
    public string? BillOfLadingNumber { get; set; }
    
    public decimal? ExportTax { get; set; }
    
    public decimal? Discount { get; set; }
    
    [MaxLength(255)]
    public string? PaymentMethod { get; set; }
    
    [MaxLength(255)]
    public string? ReceiptCode { get; set; }
    
    public DateTime? ImportDate { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    
    // Thêm các trường mới từ ảnh
    [MaxLength(255)]
    public string? VoucherCode { get; set; }
    
    public decimal? TaxRate { get; set; }
    
    public decimal? DiscountPercent { get; set; }
    
    public List<CreateImportOrderDetailRequest>? Details { get; set; }
}

public class CreateImportOrderDetailRequest
{
    [Required]
    public int ProductId { get; set; }
    public int? ProductBatchId { get; set; }
    
    [Required]
    public int WarehouseCellId { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    public int? ReceivedQuantity { get; set; }
    public int? RemainingQuantity { get; set; }
    
    public decimal? TotalPrice { get; set; }
    public decimal? UnitPrice { get; set; }
    
    [MaxLength(100)]
    public string? BatchNumber { get; set; }
    
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    [MaxLength(100)]
    public string? Supplier { get; set; }
    
    [MaxLength(50)]
    public string? Unit { get; set; }
    
    // Thông tin từ bảng Lo_hang
    public DateTime? ArrivalDate { get; set; }
    public int? ArrivalBatchNumber { get; set; }
    public int? ContainerVehicleNumber { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class UpdateImportOrderRequest
{
    [MaxLength(255)]
    public string? OrderName { get; set; }
    
    public DateTime? OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    
    public int? WarehouseId { get; set; }
    public int? SupplierId { get; set; }
    
    // Thông tin khách hàng (cho phiếu xuất)
    public int? CustomerId { get; set; }
    [MaxLength(255)]
    public string? CustomerName { get; set; }
    [MaxLength(20)]
    public string? CustomerPhone { get; set; }
    [MaxLength(500)]
    public string? CustomerAddress { get; set; }
    [MaxLength(255)]
    public string? ExportReason { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
    
    [MaxLength(50)]
    public string? OrderType { get; set; }
    
    public decimal? TotalAmount { get; set; }
    public decimal? TotalValue { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [MaxLength(255)]
    public string? PaymentMethod { get; set; }
    
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
}
