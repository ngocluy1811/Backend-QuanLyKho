using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs;

public class BulkAttendanceDto
{
    public List<int> EmployeeIds { get; set; } = new();
    public List<AttendanceRecordDto> Records { get; set; } = new();
    public DateTime Date { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Status { get; set; } = "Present";
    public string? Notes { get; set; }
}

public class AttendanceRecordDto
{
    public int EmployeeId { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Status { get; set; } = "Present";
    public string? Notes { get; set; }
}

public class DepartmentAttendanceDto
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int TotalEmployees { get; set; }
    public int PresentEmployees { get; set; }
    public int AbsentEmployees { get; set; }
    public int LateEmployees { get; set; }
    public decimal AttendanceRate { get; set; }
    public List<EmployeeAttendanceDto> EmployeeAttendances { get; set; } = new();
}

public class EmployeeAttendanceDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public TimeSpan? WorkHours { get; set; }
    public TimeSpan? OvertimeHours { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class AttendanceReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalEmployees { get; set; }
    public int TotalWorkingDays { get; set; }
    public int TotalPresentDays { get; set; }
    public int TotalAbsentDays { get; set; }
    public int TotalLateDays { get; set; }
    public decimal OverallAttendanceRate { get; set; }
    public List<DepartmentAttendanceDto> DepartmentStats { get; set; } = new();
    public List<EmployeeAttendanceDto> EmployeeStats { get; set; } = new();
}