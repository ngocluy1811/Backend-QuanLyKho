using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs;

public class MachineControlDto
{
    public int MachineId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Start, Stop, Pause, Resume
    public string Status { get; set; } = string.Empty;
    public decimal Efficiency { get; set; }
    public string? Notes { get; set; }
}

public class ProductionMachineDto
{
    public int Id { get; set; }
    public int ProductionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public string MachineType { get; set; } = string.Empty;
    public string MachineCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Efficiency { get; set; }
    public int? AssignedOperator { get; set; }
    public string? AssignedOperatorName { get; set; }
    public string? Product { get; set; }
    public decimal Progress { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Batch { get; set; }
    public string? Operator { get; set; }
    public string? Details { get; set; }
    public DateTime? LastUpdated { get; set; }
    public DateTime? LastMaintenance { get; set; }
    public DateTime? NextMaintenance { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ProductionFormulaDto
{
    public int Id { get; set; }
    public int FormulaId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FormulaName { get; set; } = string.Empty;
    public string FormulaCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TotalWeight { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal ProductionRate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastUsed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<FormulaComponentDto> Components { get; set; } = new();
}

public class CreateProductionFormulaDto
{
    [Required(ErrorMessage = "ID sản phẩm là bắt buộc.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Tên công thức là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên công thức không được vượt quá 100 ký tự.")]
    public string FormulaName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mã công thức là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Mã công thức không được vượt quá 50 ký tự.")]
    public string FormulaCode { get; set; } = string.Empty;

    public string FormulaId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Tổng trọng lượng là bắt buộc.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Tổng trọng lượng phải lớn hơn 0.")]
    public decimal TotalWeight { get; set; }

    [Required(ErrorMessage = "Đơn vị là bắt buộc.")]
    [StringLength(20, ErrorMessage = "Đơn vị không được vượt quá 20 ký tự.")]
    public string Unit { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Tốc độ sản xuất phải lớn hơn hoặc bằng 0.")]
    public decimal ProductionRate { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    public string Status { get; set; } = string.Empty;

    public List<CreateFormulaComponentDto> Components { get; set; } = new();
}

public class FormulaComponentDto
{
    public int Id { get; set; }
    public int FormulaId { get; set; }
    public int MaterialId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public string? Notes { get; set; }
}

public class CreateFormulaComponentDto
{
    [Required(ErrorMessage = "ID nguyên liệu là bắt buộc.")]
    public int MaterialId { get; set; }

    [Required(ErrorMessage = "Số lượng là bắt buộc.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public decimal Quantity { get; set; }

    [Required(ErrorMessage = "Số lượng là bắt buộc.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Đơn vị là bắt buộc.")]
    [StringLength(20, ErrorMessage = "Đơn vị không được vượt quá 20 ký tự.")]
    public string Unit { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "Phần trăm phải từ 0-100%.")]
    public decimal Percentage { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Notes { get; set; }
}

public class MaterialInventoryDto
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string MaterialCode { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal Stock { get; set; }
    public decimal MinStockLevel { get; set; }
    public decimal MaxStockLevel { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal Purity { get; set; }
    public decimal NitrogenContent { get; set; }
    public decimal P2O5Content { get; set; }
    public decimal K2OContent { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public DateTime LastChecked { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public List<MaterialComponentDto> Components { get; set; } = new();
}

public class MaterialComponentDto
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ComponentName { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public decimal Amount { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class ProductionHistoryDto
{
    public int Id { get; set; }
    public int ProductionId { get; set; }
    public string BatchId { get; set; } = string.Empty;
    public string ProductionOrderNumber { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Timeframe { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public string Quality { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string Supervisor { get; set; } = string.Empty;
    public DateTime ProductionDate { get; set; }
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ProductionStatsDto
{
    public int TotalProductions { get; set; }
    public int CompletedProductions { get; set; }
    public int InProgressProductions { get; set; }
    public int CancelledProductions { get; set; }
    public int RunningMachines { get; set; }
    public int TotalMachines { get; set; }
    public int ActiveOrders { get; set; }
    public int MaterialAlerts { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TodayProduction { get; set; }
    public decimal AverageEfficiency { get; set; }
    public decimal OverallEfficiency { get; set; }
    public int ProductionsThisMonth { get; set; }
    public int ProductionsThisWeek { get; set; }
    public DateTime LastUpdated { get; set; }
}
