using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs;

public class WarehousePositionDto
{
    public int Id { get; set; }
    public int ZoneId { get; set; }
    public string PositionCode { get; set; } = string.Empty; // e.g., A01, B02
    public string PositionType { get; set; } = string.Empty; // e.g., Shelf, Floor, Rack
    public int MaxCapacity { get; set; } // Max items or weight
    public int CurrentQuantity { get; set; }
    public decimal OccupancyPercentage => MaxCapacity > 0 ? (decimal)CurrentQuantity / MaxCapacity * 100 : 0;
    public string Status { get; set; } = string.Empty; // Empty, Occupied, Full, Reserved
    public string? ProductId { get; set; } // Current product stored
    public string? ProductName { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? LastMoved { get; set; }
}

public class CreateWarehousePositionDto
{
    [Required(ErrorMessage = "Mã vị trí là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Mã vị trí không được vượt quá 50 ký tự.")]
    public string PositionCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loại vị trí là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Loại vị trí không được vượt quá 100 ký tự.")]
    public string PositionType { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Sức chứa tối đa phải là số dương.")]
    public int MaxCapacity { get; set; }
}

public class UpdateWarehousePositionDto
{
    [StringLength(50, ErrorMessage = "Mã vị trí không được vượt quá 50 ký tự.")]
    public string? PositionCode { get; set; }

    [StringLength(100, ErrorMessage = "Loại vị trí không được vượt quá 100 ký tự.")]
    public string? PositionType { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Sức chứa tối đa phải là số dương.")]
    public int? MaxCapacity { get; set; }

    [StringLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự.")]
    public string? Status { get; set; }
}

public class AssignProductToPositionDto
{
    [Required(ErrorMessage = "ID sản phẩm là bắt buộc.")]
    public string ProductId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số lượng là bắt buộc.")]
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public int Quantity { get; set; }

    public string? BatchNumber { get; set; }
}

public class TransferProductDto
{
    [Required(ErrorMessage = "Số lượng là bắt buộc.")]
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public int Quantity { get; set; }
}
