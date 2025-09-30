using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs;

public class AttendanceStatsDto
{
    public int TotalEmployees { get; set; }
    public int PresentToday { get; set; }
    public int AbsentToday { get; set; }
    public int LateToday { get; set; }
    public decimal AttendanceRate { get; set; }
    public int PresentThisWeek { get; set; }
    public int PresentThisMonth { get; set; }
    public decimal WeeklyAttendanceRate { get; set; }
    public decimal MonthlyAttendanceRate { get; set; }
    public DateTime Date { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public int LeaveCount { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class LeaveBalanceDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int TotalAnnualLeave { get; set; }
    public int AnnualLeaveBalance { get; set; }
    public int RemainingAnnualLeave { get; set; }
    public int SickLeaveBalance { get; set; }
    public int SickLeaveUsed { get; set; }
    public int PersonalLeaveBalance { get; set; }
    public int PersonalLeaveUsed { get; set; }
    public int MaternityLeaveBalance { get; set; }
    public int PaternityLeaveBalance { get; set; }
    public int TotalLeaveBalance { get; set; }
    public int UsedAnnualLeave { get; set; }
    public int UsedSickLeave { get; set; }
    public int UsedPersonalLeave { get; set; }
    public int UsedMaternityLeave { get; set; }
    public int UsedPaternityLeave { get; set; }
    public int TotalUsedLeave { get; set; }
    public int Year { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class LeaveStatsDto
{
    public int TotalRequests { get; set; }
    public int TotalLeaveRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int CancelledRequests { get; set; }
    public int RequestsThisMonth { get; set; }
    public int RequestsThisWeek { get; set; }
    public int TotalDaysRequested { get; set; }
    public int TotalDaysApproved { get; set; }
    public decimal ApprovalRate { get; set; }
    public int AverageProcessingDays { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class StockTakeStatsDto
{
    public int TotalStockTakes { get; set; }
    public int PlannedStockTakes { get; set; }
    public int CompletedStockTakes { get; set; }
    public int InProgressStockTakes { get; set; }
    public int PendingStockTakes { get; set; }
    public int CancelledStockTakes { get; set; }
    public int OverdueStockTakes { get; set; }
    public int StockTakesThisMonth { get; set; }
    public int StockTakesThisWeek { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal AverageAccuracy { get; set; }
    public int TotalItemsChecked { get; set; }
    public int TotalDiscrepancies { get; set; }
    public int DiscrepanciesFound { get; set; }
    public decimal TotalVarianceValue { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public DateTime LastUpdated { get; set; }
}
