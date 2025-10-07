using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Models.DTOs
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Message { get; set; }
    }

    public class ContactAdminRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung yêu cầu là bắt buộc")]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;
    }

    public class PasswordResetRequestResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ProcessedBy { get; set; }
        public string? AdminNotes { get; set; }
        public DateTime? PasswordResetAt { get; set; }
    }

    public class ProcessPasswordResetRequest
    {
        [Required]
        public int RequestId { get; set; }

        [Required]
        public string Action { get; set; } = string.Empty; // Approve, Reject

        [MaxLength(1000)]
        public string? AdminNotes { get; set; }
    }

    public class ResetUserPasswordRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string NewPassword { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
