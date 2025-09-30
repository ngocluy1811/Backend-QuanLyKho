using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProductCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string ProductName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int? CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public ProductCategory? CategoryNavigation { get; set; }

    [MaxLength(255)]
    public string Category { get; set; } = string.Empty; // String field for category name

    public decimal UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty; // e.g., "kg", "bao", "lít"
    public decimal Price { get; set; } // Alternative price field

    public int MinStockLevel { get; set; }
    public int MaxStockLevel { get; set; }

    public int? CompanyId { get; set; }
    [ForeignKey("CompanyId")]
    public Company? Company { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Active"; // e.g., Active, Inactive, Discontinued

    public int? SupplierId { get; set; }
    [ForeignKey("SupplierId")]
    public Supplier? SupplierNavigation { get; set; }

    // Product tracking fields
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    [MaxLength(100)]
    public string? BatchNumber { get; set; }

    // Additional properties that might be needed
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();
    public ICollection<ProductComposition> ProductCompositions { get; set; } = new List<ProductComposition>();
    public ICollection<Production> Productions { get; set; } = new List<Production>();
    public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    public ICollection<SalesOrderDetail> SalesOrderDetails { get; set; } = new List<SalesOrderDetail>();
    public ICollection<StockAdjustmentDetail> StockAdjustmentDetails { get; set; } = new List<StockAdjustmentDetail>();
    public ICollection<StockTransferDetail> StockTransferDetails { get; set; } = new List<StockTransferDetail>();
    public ICollection<StockTakeDetail> StockTakeDetails { get; set; } = new List<StockTakeDetail>();
    public ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<WarehouseCell> WarehouseCells { get; set; } = new List<WarehouseCell>();
}
