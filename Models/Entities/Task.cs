using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.Models.Entities;

public class Task : BaseEntity
{
    public int CompanyId { get; set; }
    public int? DepartmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TaskType { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal";
    public int? AssignedTo { get; set; }
    public new int CreatedBy { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Models.Enums.TaskStatus Status { get; set; }
    public int ProgressPercentage { get; set; } = 0;
    public int EstimatedHours { get; set; } = 0;
    public int ActualHours { get; set; } = 0;
    public int? WarehouseId { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual Department? Department { get; set; }
    public virtual User? AssignedToUser { get; set; }
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual Warehouse? Warehouse { get; set; }
    public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
}
