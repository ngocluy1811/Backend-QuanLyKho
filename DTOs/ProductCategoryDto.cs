using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class ProductCategoryDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public int? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Statistics
        public int ProductCount { get; set; }
        public decimal TotalStockValue { get; set; }
        
        public List<ProductCategoryDto> SubCategories { get; set; } = new();
        public List<ProductDto> Products { get; set; } = new();
    }

    public class CreateProductCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public int? ParentCategoryId { get; set; }
    }

    public class UpdateProductCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public int? ParentCategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}
