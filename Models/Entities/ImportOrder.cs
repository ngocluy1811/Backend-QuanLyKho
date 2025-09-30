using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities;

public class ImportOrder
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string OrderName { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }

    public int WarehouseId { get; set; }
    [ForeignKey("WarehouseId")]
    public Warehouse Warehouse { get; set; } = null!;

    public int? SupplierId { get; set; }
    [ForeignKey("SupplierId")]
    public Supplier? Supplier { get; set; }

    // Thông tin khách hàng (cho phiếu xuất)
    public int? CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    public Customer? Customer { get; set; }

    [MaxLength(255)]
    public string? CustomerName { get; set; }

    [MaxLength(20)]
    public string? CustomerPhone { get; set; }

    [MaxLength(500)]
    public string? CustomerAddress { get; set; }

    [MaxLength(255)]
    public string? ExportReason { get; set; } // Lý do xuất kho

    public int? CompanyId { get; set; }
    [ForeignKey("CompanyId")]
    public Company? Company { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled

    [MaxLength(50)]
    public string OrderType { get; set; } = "Import"; // Import, Export

    public decimal? TotalAmount { get; set; }
    public decimal? TotalValue { get; set; }

    // Thông tin vận chuyển
    [MaxLength(50)]
    public string? ContainerNumber { get; set; } // so_container

    [MaxLength(50)]
    public string? VehiclePlateNumber { get; set; } // bien_so_xe

    [MaxLength(50)]
    public string? DriverName { get; set; } // ten_tai_xe

    public decimal? TotalWeight { get; set; } // trong_luong

    [MaxLength(50)]
    public string? BatchNumber { get; set; } // so_dot_ve

    [MaxLength(50)]
    public string? BillOfLadingNumber { get; set; } // so_van_don

    public decimal? ExportTax { get; set; } // thue_xuat

    public decimal? Discount { get; set; } // chiet_khau

    [MaxLength(255)]
    public string? PaymentMethod { get; set; } // hinh_thuctt

    [MaxLength(255)]
    public string? ReceiptCode { get; set; } // ma_phieu

    public DateTime? ImportDate { get; set; } // ngay_nhap
    
    // Thêm các trường mới từ ảnh
    [MaxLength(255)]
    public string? VoucherCode { get; set; } // ma_phieu
    
    public decimal? TaxRate { get; set; } // thue_suat
    
    public decimal? DiscountPercent { get; set; } // chiet_khau

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<ImportOrderDetail> ImportOrderDetails { get; set; } = new List<ImportOrderDetail>();
}
