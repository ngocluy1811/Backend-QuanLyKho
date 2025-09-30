namespace FertilizerWarehouseAPI.Models.Entities;

public class Report : BaseEntity
{
    public int CompanyId { get; set; }
    public int? DepartmentId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public int SubmittedBy { get; set; }
    public DateTime SubmittedDate { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string? FilePath { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual Department? Department { get; set; }
    public virtual User Submitter { get; set; } = null!;
    public virtual User? Approver { get; set; }
}
