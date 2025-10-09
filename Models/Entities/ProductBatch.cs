using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities;

public class ProductBatch
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string BatchNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string BatchName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    public int Quantity { get; set; }
    public int RemainingQuantity { get; set; }
    public int InitialQuantity { get; set; }
    public int CurrentQuantity { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Active"; // Active, Expired, Consumed, Inactive
    
    public int QualityStatus { get; set; } = 1; // 1=Good, 2=Damaged, 3=Expired, 4=Recalled

    public int? SupplierId { get; set; }
    [ForeignKey("SupplierId")]
    public Supplier? Supplier { get; set; }

    public int? WarehousePositionId { get; set; }

    public decimal? UnitPrice { get; set; }
    public decimal? TotalValue { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Thêm 3 trường mới từ database schema dbo.Lo_hang
    public DateTime? NgayVe { get; set; }           // ngay_ve - Ngày hàng về (chung)
    public int? SoDotVe { get; set; }              // so_dot_ve - Số đợt về
    public int? SoXeContainerTungDot { get; set; }  // so_xecontainer_tungdot - Số xe container từng đợt
    
    // Thêm trường để lưu mảng ngày về cho từng đợt (JSON string)
    [MaxLength(2000)]
    public string? NgayVeDetails { get; set; }      // JSON string chứa mảng ngày về cho từng đợt

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<WarehouseCell> WarehouseCells { get; set; } = new List<WarehouseCell>();
}