namespace FertilizerWarehouseAPI.Models.Entities;

public class Supplier : BaseEntity
{
    public int CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxCode { get; set; }
    public string? Website { get; set; }
    public string? BankAccount { get; set; }
    public string? BankName { get; set; }
    public string? PaymentTerms { get; set; }
    public decimal? CreditLimit { get; set; }
    public int? Rating { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Active";

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    public virtual ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();
}
