namespace FertilizerWarehouseAPI.Models.Entities;

public class TaskComment : BaseEntity
{
    public int TaskId { get; set; }
    public int CommentBy { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CommentAt { get; set; }
    public string? AttachmentPath { get; set; }

    // Navigation properties
    public virtual Task Task { get; set; } = null!;
    public virtual User CommentByUser { get; set; } = null!;
}
