using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.DTOs;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Services.Interfaces;
using FertilizerWarehouseAPI.Helpers;
using AutoMapper;

namespace FertilizerWarehouseAPI.Services.Implementations;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ApplicationDbContext context, IMapper mapper, ILogger<ProductService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _context.Products
            .Include(p => p.CategoryNavigation)
            .Include(p => p.SupplierNavigation)
            .Where(p => p.IsActive)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.CategoryNavigation)
            .Include(p => p.SupplierNavigation)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        return product != null ? _mapper.Map<ProductDto>(product) : null;
    }

    public async Task<ProductDto?> GetProductByCodeAsync(string code)
    {
        var product = await _context.Products
            .Include(p => p.CategoryNavigation)
            .Include(p => p.SupplierNavigation)
            .FirstOrDefaultAsync(p => p.ProductCode == code && p.IsActive);

        return product != null ? _mapper.Map<ProductDto>(product) : null;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
    {
        var product = _mapper.Map<Product>(createDto);
        product.CreatedAt = DateTime.UtcNow;
        product.IsActive = true;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product created: {ProductCode} by user", product.ProductCode);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null || !product.IsActive) return null;

        _mapper.Map(updateDto, product);
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Product updated: {ProductCode}", product.ProductCode);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product deleted: {ProductCode}", product.ProductCode);
        return true;
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchQuery)
    {
        var products = await _context.Products
            .Include(p => p.CategoryNavigation)
            .Include(p => p.SupplierNavigation)
            .Where(p => p.IsActive && 
                       (p.ProductName.Contains(searchQuery) || 
                        p.ProductCode.Contains(searchQuery) ||
                        p.Description!.Contains(searchQuery)))
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> FilterProductsAsync(string? category = null, string? status = null, string? supplier = null)
    {
        var query = _context.Products
            .Include(p => p.CategoryNavigation)
            .Include(p => p.SupplierNavigation)
            .Where(p => p.IsActive);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.CategoryNavigation != null && p.CategoryNavigation.CategoryName == category);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status == status);

        if (!string.IsNullOrEmpty(supplier))
            query = query.Where(p => p.SupplierNavigation != null && p.SupplierNavigation.SupplierName == supplier);

        var products = await query.ToListAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _context.Products
            .Include(p => p.CategoryNavigation)
            .Include(p => p.SupplierNavigation)
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync()
    {
        var products = await _context.Products
            .Include(p => p.CategoryNavigation)
            .Include(p => p.SupplierNavigation)
            .Where(p => p.IsActive)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    // Product Categories
    public async Task<IEnumerable<ProductCategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _context.ProductCategories
            .Where(c => c.IsActive)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductCategoryDto>>(categories);
    }

    public async Task<ProductCategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _context.ProductCategories
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        return category != null ? _mapper.Map<ProductCategoryDto>(category) : null;
    }

    public async Task<ProductCategoryDto> CreateCategoryAsync(CreateProductCategoryDto createDto)
    {
        var category = _mapper.Map<ProductCategory>(createDto);
        category.CreatedAt = DateTime.UtcNow;
        category.IsActive = true;

        _context.ProductCategories.Add(category);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductCategoryDto>(category);
    }

    public async Task<ProductCategoryDto?> UpdateCategoryAsync(int id, UpdateProductCategoryDto updateDto)
    {
        var category = await _context.ProductCategories.FindAsync(id);
        if (category == null || !category.IsActive) return null;

        _mapper.Map(updateDto, category);
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return _mapper.Map<ProductCategoryDto>(category);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.ProductCategories.FindAsync(id);
        if (category == null) return false;

        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    // Placeholder implementations for complex methods
    public async Task<ProductInventoryDto?> GetProductInventoryAsync(int productId)
    {
        // Implementation for product inventory
        return new ProductInventoryDto();
    }

    public async Task<IEnumerable<ProductStockLocationDto>> GetProductStockLocationsAsync(int productId)
    {
        // Implementation for stock locations
        return new List<ProductStockLocationDto>();
    }

    public async Task<bool> UpdateProductStockAsync(int productId, int warehouseId, decimal quantity, string movementType)
    {
        // Implementation for stock updates
        return true;
    }

    public async Task<bool> ReserveProductStockAsync(int productId, decimal quantity, string reservedBy)
    {
        // Implementation for stock reservation
        return true;
    }

    public async Task<bool> ReleaseProductReservationAsync(int productId, decimal quantity)
    {
        // Implementation for stock release
        return true;
    }

    public async Task<IEnumerable<ProductBatchDto>> GetProductBatchesAsync(int productId)
    {
        // Implementation for product batches
        return new List<ProductBatchDto>();
    }

    public async Task<ProductBatchDto?> GetProductBatchByIdAsync(int batchId)
    {
        // Implementation for batch by ID
        return new ProductBatchDto();
    }

    public async Task<ProductBatchDto> CreateProductBatchAsync(CreateProductBatchDto createDto)
    {
        // Implementation for batch creation
        return new ProductBatchDto();
    }

    public async Task<ProductBatchDto?> UpdateProductBatchAsync(int batchId, UpdateProductBatchDto updateDto)
    {
        // Implementation for batch update
        return new ProductBatchDto();
    }

    public async Task<bool> DeleteProductBatchAsync(int batchId)
    {
        // Implementation for batch deletion
        return true;
    }

    public async Task<ProductPriceHistoryDto> GetProductPriceHistoryAsync(int productId)
    {
        // Implementation for price history
        return new ProductPriceHistoryDto();
    }

    public async Task<bool> UpdateProductPriceAsync(int productId, decimal newPrice, string updatedBy)
    {
        // Implementation for price update
        return true;
    }

    public async Task<IEnumerable<ProductSupplierDto>> GetProductSuppliersAsync(int productId)
    {
        // Implementation for product suppliers
        return new List<ProductSupplierDto>();
    }

    public async Task<bool> AddProductSupplierAsync(int productId, int supplierId, decimal supplierPrice)
    {
        // Implementation for adding supplier
        return true;
    }

    public async Task<bool> RemoveProductSupplierAsync(int productId, int supplierId)
    {
        // Implementation for removing supplier
        return true;
    }

    public async Task<bool> ImportProductsAsync(Stream fileStream)
    {
        // Implementation for product import
        return true;
    }

    public async Task<Stream> ExportProductsAsync(string format = "excel")
    {
        // Implementation for product export
        return new MemoryStream();
    }

    public async Task<Stream> ExportProductInventoryAsync(string format = "excel")
    {
        // Implementation for inventory export
        return new MemoryStream();
    }

    public async Task<ProductStatsDto> GetProductStatsAsync()
    {
        // Implementation for product statistics
        return new ProductStatsDto();
    }

    public async Task<IEnumerable<ProductMovementHistoryDto>> GetProductMovementHistoryAsync(int productId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        // Implementation for movement history
        return new List<ProductMovementHistoryDto>();
    }

    public async Task<IEnumerable<ProductTrendDto>> GetProductTrendsAsync(DateTime fromDate, DateTime toDate)
    {
        // Implementation for product trends
        return new List<ProductTrendDto>();
    }

    public async Task<IEnumerable<ProductQualityCheckDto>> GetProductQualityChecksAsync(int productId)
    {
        // Implementation for quality checks
        return new List<ProductQualityCheckDto>();
    }

    public async Task<ProductQualityCheckDto> CreateProductQualityCheckAsync(CreateProductQualityCheckDto createDto)
    {
        // Implementation for quality check creation
        return new ProductQualityCheckDto();
    }

    public async Task<bool> UpdateProductQualityStatusAsync(int productId, string qualityStatus, string notes)
    {
        // Implementation for quality status update
        return true;
    }
}
