using System.ComponentModel.DataAnnotations;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string TaskType { get; set; } = string.Empty;
        [StringLength(50)]
        public string Priority { get; set; } = "Normal"; // Low, Normal, High, Critical
        public int? AssignedTo { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public FertilizerWarehouseAPI.Models.Enums.TaskStatus Status { get; set; } = FertilizerWarehouseAPI.Models.Enums.TaskStatus.Pending;
        [Range(0, 100)]
        public int ProgressPercentage { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int EstimatedHours { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int ActualHours { get; set; } = 0;
        public int? WarehouseId { get; set; }
        [StringLength(100)]
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CompanyDto? Company { get; set; }
        public DepartmentDto? Department { get; set; }
        public UserDto? AssignedToUser { get; set; }
        public UserDto? CreatedByUser { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public ICollection<TaskCommentDto>? Comments { get; set; }
    }

    public class CreateTaskDto
    {
        [Required]
        public int CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string TaskType { get; set; } = string.Empty;
        [StringLength(50)]
        public string Priority { get; set; } = "Normal";
        public int? AssignedTo { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        [Range(0, int.MaxValue)]
        public int EstimatedHours { get; set; } = 0;
        public int? WarehouseId { get; set; }
        [StringLength(100)]
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
    }

    public class UpdateTaskDto
    {
        [StringLength(255)]
        public string? Title { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string? TaskType { get; set; }
        [StringLength(50)]
        public string? Priority { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public FertilizerWarehouseAPI.Models.Enums.TaskStatus? Status { get; set; }
        [Range(0, 100)]
        public int? ProgressPercentage { get; set; }
        [Range(0, int.MaxValue)]
        public int? EstimatedHours { get; set; }
        [Range(0, int.MaxValue)]
        public int? ActualHours { get; set; }
        public int? WarehouseId { get; set; }
        [StringLength(100)]
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class TaskCommentDto
    {
        public int Id { get; set; }
        [Required]
        public int TaskId { get; set; }
        [Required]
        public int CommentBy { get; set; }
        [Required]
        [StringLength(1000)]
        public string CommentText { get; set; } = string.Empty;
        public DateTime CommentAt { get; set; }
        [StringLength(500)]
        public string? AttachmentPath { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDto? CommentByUser { get; set; }
    }

    public class CreateTaskCommentDto
    {
        [Required]
        public int TaskId { get; set; }
        [Required]
        public int CommentBy { get; set; }
        [Required]
        [StringLength(1000)]
        public string CommentText { get; set; } = string.Empty;
        [StringLength(500)]
        public string? AttachmentPath { get; set; }
    }

    public class TaskSummaryDto
    {
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal AverageCompletionTime { get; set; } // in hours
    }
}