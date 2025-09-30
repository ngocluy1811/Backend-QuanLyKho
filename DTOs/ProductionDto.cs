using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs;

public class ProductionDto
{
    public int Id { get; set; }
    public string ProductionOrderNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal PlannedQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime PlannedStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateProductionDto
{
    [Required(ErrorMessage = "Số lệnh sản xuất là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Số lệnh sản xuất không được vượt quá 50 ký tự.")]
    public string ProductionOrderNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "ID sản phẩm là bắt buộc.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
    [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự.")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số lượng kế hoạch là bắt buộc.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng kế hoạch phải lớn hơn 0.")]
    public decimal PlannedQuantity { get; set; }

    [Required(ErrorMessage = "Đơn vị là bắt buộc.")]
    [StringLength(20, ErrorMessage = "Đơn vị không được vượt quá 20 ký tự.")]
    public string Unit { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày bắt đầu kế hoạch là bắt buộc.")]
    public DateTime PlannedStartDate { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc kế hoạch là bắt buộc.")]
    public DateTime PlannedEndDate { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    public string Status { get; set; } = string.Empty;

    [Required(ErrorMessage = "Độ ưu tiên là bắt buộc.")]
    public string Priority { get; set; } = string.Empty;

    public int? AssignedTo { get; set; }
    public string? Notes { get; set; }
}

public class UpdateProductionDto
{
    [StringLength(50, ErrorMessage = "Số lệnh sản xuất không được vượt quá 50 ký tự.")]
    public string? ProductionOrderNumber { get; set; }

    public int? ProductId { get; set; }

    [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự.")]
    public string? ProductName { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng kế hoạch phải lớn hơn 0.")]
    public decimal? PlannedQuantity { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Số lượng thực tế phải lớn hơn hoặc bằng 0.")]
    public decimal? ActualQuantity { get; set; }

    [StringLength(20, ErrorMessage = "Đơn vị không được vượt quá 20 ký tự.")]
    public string? Unit { get; set; }

    public DateTime? PlannedStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public int? AssignedTo { get; set; }
    public string? Notes { get; set; }
}


public class CreateProductionMachineDto
{
    [Required(ErrorMessage = "ID sản xuất là bắt buộc.")]
    public int ProductionId { get; set; }

    [Required(ErrorMessage = "Tên máy là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên máy không được vượt quá 100 ký tự.")]
    public string MachineName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loại máy là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Loại máy không được vượt quá 50 ký tự.")]
    public string MachineType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mã máy là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Mã máy không được vượt quá 50 ký tự.")]
    public string MachineCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    public string Status { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "Hiệu suất phải từ 0-100%.")]
    public decimal Efficiency { get; set; } = 100;

    public int? AssignedOperator { get; set; }
    public DateTime? LastMaintenance { get; set; }
    public DateTime? NextMaintenance { get; set; }
    public string? Notes { get; set; }
}

public class UpdateProductionMachineDto
{
    [StringLength(100, ErrorMessage = "Tên máy không được vượt quá 100 ký tự.")]
    public string? MachineName { get; set; }

    [StringLength(50, ErrorMessage = "Loại máy không được vượt quá 50 ký tự.")]
    public string? MachineType { get; set; }

    [StringLength(50, ErrorMessage = "Mã máy không được vượt quá 50 ký tự.")]
    public string? MachineCode { get; set; }

    public string? Status { get; set; }

    [Range(0, 100, ErrorMessage = "Hiệu suất phải từ 0-100%.")]
    public decimal? Efficiency { get; set; }

    public int? AssignedOperator { get; set; }
    public DateTime? LastMaintenance { get; set; }
    public DateTime? NextMaintenance { get; set; }
    public string? Notes { get; set; }
}