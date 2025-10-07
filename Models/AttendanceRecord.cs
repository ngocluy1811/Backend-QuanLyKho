using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public decimal? OvertimeHours { get; set; }

        public DateTime? OvertimeStartTime { get; set; }

        public DateTime? OvertimeEndTime { get; set; }

        public string? Notes { get; set; }

        public string? Status { get; set; }

        public bool IsOvertimeRequired { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
