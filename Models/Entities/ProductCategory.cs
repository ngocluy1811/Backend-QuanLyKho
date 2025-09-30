namespace FertilizerWarehouseAPI.Models.Entities;

public class ProductCategory : BaseEntity
{
    public int CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual ProductCategory? ParentCategory { get; set; }
    public virtual ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
