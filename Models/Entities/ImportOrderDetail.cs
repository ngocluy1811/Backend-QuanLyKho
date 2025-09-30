using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities;

public class ImportOrderDetail
{
    [Key]
    public int Id { get; set; }

    public int ImportOrderId { get; set; }
    [ForeignKey("ImportOrderId")]
    public ImportOrder ImportOrder { get; set; } = null!;

    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    public int? ProductBatchId { get; set; }
    [ForeignKey("ProductBatchId")]
    public ProductBatch? ProductBatch { get; set; }

    public int WarehouseCellId { get; set; }
    [ForeignKey("WarehouseCellId")]
    public WarehouseCell WarehouseCell { get; set; } = null!;

    public int Quantity { get; set; }
    public int? ReceivedQuantity { get; set; }
    public int? RemainingQuantity { get; set; }

    public decimal? UnitPrice { get; set; }
    public decimal? TotalPrice { get; set; }

    [MaxLength(100)]
    public string? BatchNumber { get; set; }

    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    [MaxLength(100)]
    public string? Supplier { get; set; }

    [MaxLength(50)]
    public string Unit { get; set; } = "kg";

    // Thông tin từ bảng Lo_hang
    public DateTime? ArrivalDate { get; set; } // ngay_ve
    public int? ArrivalBatchNumber { get; set; } // so_dot_ve
    public int? ContainerVehicleNumber { get; set; } // so_xecontainer_tungdot

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
