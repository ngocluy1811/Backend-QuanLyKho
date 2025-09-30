using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs;

public class ProductionOrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string OrderCode { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public decimal CompletionRate { get; set; }
    public string Machine { get; set; } = string.Empty;
    public string Formula { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateProductionOrderDto
{
    [Required(ErrorMessage = "Số lệnh sản xuất là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Số lệnh sản xuất không được vượt quá 50 ký tự.")]
    public string OrderNumber { get; set; } = string.Empty;

    public string OrderCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "ID sản phẩm là bắt buộc.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Số lượng là bắt buộc.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public decimal Quantity { get; set; }

    [Required(ErrorMessage = "Đơn vị là bắt buộc.")]
    [StringLength(20, ErrorMessage = "Đơn vị không được vượt quá 20 ký tự.")]
    public string Unit { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày bắt đầu kế hoạch là bắt buộc.")]
    public DateTime PlannedStartDate { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc kế hoạch là bắt buộc.")]
    public DateTime PlannedEndDate { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Formula { get; set; } = string.Empty;

    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    public string Status { get; set; } = string.Empty;

    [Required(ErrorMessage = "Độ ưu tiên là bắt buộc.")]
    public string Priority { get; set; } = string.Empty;

    public int? AssignedTo { get; set; }
    public string? Notes { get; set; }
}

public class UpdateProductionOrderDto
{
    [StringLength(50, ErrorMessage = "Số lệnh sản xuất không được vượt quá 50 ký tự.")]
    public string? OrderNumber { get; set; }

    public string? OrderCode { get; set; }

    public int? ProductId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public decimal? Quantity { get; set; }

    [StringLength(20, ErrorMessage = "Đơn vị không được vượt quá 20 ký tự.")]
    public string? Unit { get; set; }

    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public decimal? CompletionRate { get; set; }
    public string? Machine { get; set; }
    public string? Formula { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public int? AssignedTo { get; set; }
    public string? Notes { get; set; }
}
