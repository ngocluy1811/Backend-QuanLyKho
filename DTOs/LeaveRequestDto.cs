using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs;

public class LeaveRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string? Comments { get; set; }
    public int? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovalNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateLeaveRequestDto
{
    [Required(ErrorMessage = "ID nhân viên là bắt buộc.")]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Loại nghỉ phép là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Loại nghỉ phép không được vượt quá 50 ký tự.")]
    public string LeaveType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loại nghỉ phép là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Loại nghỉ phép không được vượt quá 50 ký tự.")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc là bắt buộc.")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "Tổng số ngày là bắt buộc.")]
    public int TotalDays { get; set; }

    [Required(ErrorMessage = "Lý do là bắt buộc.")]
    [StringLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự.")]
    public string Reason { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? Comments { get; set; }
}

public class UpdateLeaveRequestDto
{
    [StringLength(50, ErrorMessage = "Loại nghỉ phép không được vượt quá 50 ký tự.")]
    public string? LeaveType { get; set; }

    [StringLength(50, ErrorMessage = "Loại nghỉ phép không được vượt quá 50 ký tự.")]
    public string? Type { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? TotalDays { get; set; }

    [StringLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự.")]
    public string? Reason { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? Comments { get; set; }

    public string? Status { get; set; }
    public int? ApprovedBy { get; set; }
    public string? ApprovalNotes { get; set; }
}

public class ApproveLeaveRequestDto
{
    [Required(ErrorMessage = "Trạng thái phê duyệt là bắt buộc.")]
    public string Status { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Ghi chú phê duyệt không được vượt quá 500 ký tự.")]
    public string? ApprovalNotes { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? Comments { get; set; }
}
