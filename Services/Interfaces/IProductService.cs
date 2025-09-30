using FertilizerWarehouseAPI.DTOs;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Services.Interfaces
{
    public interface IProductService
    {
        // Product CRUD operations
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto?> GetProductByCodeAsync(string code);
        Task<ProductDto> CreateProductAsync(CreateProductDto createDto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto);
        Task<bool> DeleteProductAsync(int id);

        // Product search and filtering
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchQuery);
        Task<IEnumerable<ProductDto>> FilterProductsAsync(string? category = null, string? status = null, string? supplier = null);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductDto>> GetLowStockProductsAsync();

        // Product categories
        Task<IEnumerable<ProductCategoryDto>> GetAllCategoriesAsync();
        Task<ProductCategoryDto?> GetCategoryByIdAsync(int id);
        Task<ProductCategoryDto> CreateCategoryAsync(CreateProductCategoryDto createDto);
        Task<ProductCategoryDto?> UpdateCategoryAsync(int id, UpdateProductCategoryDto updateDto);
        Task<bool> DeleteCategoryAsync(int id);

        // Product inventory tracking
        Task<ProductInventoryDto?> GetProductInventoryAsync(int productId);
        Task<IEnumerable<ProductStockLocationDto>> GetProductStockLocationsAsync(int productId);
        Task<bool> UpdateProductStockAsync(int productId, int warehouseId, decimal quantity, string movementType);
        Task<bool> ReserveProductStockAsync(int productId, decimal quantity, string reservedBy);
        Task<bool> ReleaseProductReservationAsync(int productId, decimal quantity);

        // Product batches
        Task<IEnumerable<ProductBatchDto>> GetProductBatchesAsync(int productId);
        Task<ProductBatchDto?> GetProductBatchByIdAsync(int batchId);
        Task<ProductBatchDto> CreateProductBatchAsync(CreateProductBatchDto createDto);
        Task<ProductBatchDto?> UpdateProductBatchAsync(int batchId, UpdateProductBatchDto updateDto);
        Task<bool> DeleteProductBatchAsync(int batchId);

        // Product pricing
        Task<ProductPriceHistoryDto> GetProductPriceHistoryAsync(int productId);
        Task<bool> UpdateProductPriceAsync(int productId, decimal newPrice, string updatedBy);

        // Product suppliers
        Task<IEnumerable<ProductSupplierDto>> GetProductSuppliersAsync(int productId);
        Task<bool> AddProductSupplierAsync(int productId, int supplierId, decimal supplierPrice);
        Task<bool> RemoveProductSupplierAsync(int productId, int supplierId);

        // Product import/export
        Task<bool> ImportProductsAsync(Stream fileStream);
        Task<Stream> ExportProductsAsync(string format = "excel");
        Task<Stream> ExportProductInventoryAsync(string format = "excel");

        // Product analytics
        Task<ProductStatsDto> GetProductStatsAsync();
        Task<IEnumerable<ProductMovementHistoryDto>> GetProductMovementHistoryAsync(int productId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<ProductTrendDto>> GetProductTrendsAsync(DateTime fromDate, DateTime toDate);

        // Product quality control
        Task<IEnumerable<ProductQualityCheckDto>> GetProductQualityChecksAsync(int productId);
        Task<ProductQualityCheckDto> CreateProductQualityCheckAsync(CreateProductQualityCheckDto createDto);
        Task<bool> UpdateProductQualityStatusAsync(int productId, string qualityStatus, string notes);
    }
}
