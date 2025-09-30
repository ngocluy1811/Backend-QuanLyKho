using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs;

public class AttendanceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public TimeSpan? WorkHours { get; set; }
    public TimeSpan? OvertimeHours { get; set; }
    public DateTime? OvertimeStart { get; set; }
    public DateTime? OvertimeEnd { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateAttendanceDto
{
    [Required(ErrorMessage = "ID nhân viên là bắt buộc.")]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Ngày là bắt buộc.")]
    public DateTime Date { get; set; }

    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public TimeSpan? OvertimeHours { get; set; }
    public DateTime? OvertimeStart { get; set; }
    public DateTime? OvertimeEnd { get; set; }
    public string Status { get; set; } = "Present";
    public string? Notes { get; set; }
}

public class UpdateAttendanceDto
{
    public DateTime? Date { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public TimeSpan? OvertimeHours { get; set; }
    public DateTime? OvertimeStart { get; set; }
    public DateTime? OvertimeEnd { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}