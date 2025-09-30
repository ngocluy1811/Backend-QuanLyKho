using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs;

// Maintenance DTOs
public class MaintenanceTaskDto
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty; // Low, Medium, High, Critical
    public string Status { get; set; } = string.Empty; // Pending, InProgress, Completed, Cancelled
    public string? AssignedTo { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int? EstimatedDuration { get; set; } // minutes
    public int? ActualDuration { get; set; } // minutes
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateMaintenanceTaskDto
{
    [Required(ErrorMessage = "ID kho là bắt buộc.")]
    public int WarehouseId { get; set; }

    [Required(ErrorMessage = "Loại tác vụ là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Loại tác vụ không được vượt quá 50 ký tự.")]
    public string TaskType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
    [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Độ ưu tiên là bắt buộc.")]
    public string Priority { get; set; } = string.Empty;

    public string? AssignedTo { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public int? EstimatedDuration { get; set; }
    public string? Notes { get; set; }
}

public class UpdateMaintenanceTaskDto
{
    [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự.")]
    public string? Title { get; set; }

    [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
    public string? Description { get; set; }

    public string? Priority { get; set; }
    public string? Status { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int? EstimatedDuration { get; set; }
    public int? ActualDuration { get; set; }
    public string? Notes { get; set; }
}

public class MaintenanceLogDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public string? Notes { get; set; }
}

public class CreateMaintenanceLogDto
{
    [Required(ErrorMessage = "ID tác vụ là bắt buộc.")]
    public int TaskId { get; set; }

    [Required(ErrorMessage = "Hành động là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Hành động không được vượt quá 100 ký tự.")]
    public string Action { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? Description { get; set; }

    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Notes { get; set; }
}

public class MaintenanceScheduleDto
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Monthly, Quarterly, Yearly
    public int Interval { get; set; } // Every X days/weeks/months
    public DateTime NextDueDate { get; set; }
    public DateTime? LastPerformedDate { get; set; }
    public bool IsActive { get; set; }
    public string? AssignedTo { get; set; }
    public int? EstimatedDuration { get; set; }
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class MaintenanceStatsDto
{
    public int TotalTasks { get; set; }
    public int PendingTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int CriticalTasks { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal AverageCompletionTime { get; set; } // hours
    public int TasksThisMonth { get; set; }
    public int TasksThisWeek { get; set; }
    public DateTime LastUpdated { get; set; }
}
