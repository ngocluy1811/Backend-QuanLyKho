namespace FertilizerWarehouseAPI.Models.Entities;

public class ProductComposition : BaseEntity
{
    public int ProductId { get; set; }
    public string Component { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public string Unit { get; set; } = string.Empty;

    // Navigation properties
    public virtual Product Product { get; set; } = null!;
}
