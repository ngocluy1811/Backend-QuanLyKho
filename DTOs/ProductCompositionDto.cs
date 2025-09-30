using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class ProductCompositionDto
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        
        public int ComponentProductId { get; set; }
        public string? ComponentProductName { get; set; }
        public string? ComponentProductCode { get; set; }
        
        [Range(0.001, double.MaxValue)]
        public decimal Percentage { get; set; }
        
        [Range(0.001, double.MaxValue)]
        public decimal Quantity { get; set; }
        
        [StringLength(50)]
        public string? Unit { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateProductCompositionDto
    {
        public int ProductId { get; set; }
        public int ComponentProductId { get; set; }
        
        [Range(0.001, 100)]
        public decimal Percentage { get; set; }
        
        [Range(0.001, double.MaxValue)]
        public decimal Quantity { get; set; }
        
        [StringLength(50)]
        public string? Unit { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdateProductCompositionDto
    {
        [Range(0.001, 100)]
        public decimal Percentage { get; set; }
        
        [Range(0.001, double.MaxValue)]
        public decimal Quantity { get; set; }
        
        [StringLength(50)]
        public string? Unit { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; }
    }
}
