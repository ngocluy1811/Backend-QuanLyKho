using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FertilizerWarehouseAPI.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateEmployeeDto
{
    [Required(ErrorMessage = "Mã nhân viên là bắt buộc.")]
    [StringLength(20, ErrorMessage = "Mã nhân viên không được vượt quá 20 ký tự.")]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
    public string? Phone { get; set; }

    [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Giới tính là bắt buộc.")]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Chức vụ là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Chức vụ không được vượt quá 100 ký tự.")]
    public string Position { get; set; } = string.Empty;

    public int? DepartmentId { get; set; }
    public int? CompanyId { get; set; }

    [Required(ErrorMessage = "Ngày tuyển dụng là bắt buộc.")]
    public DateTime HireDate { get; set; }

    [Required(ErrorMessage = "Trạng thái làm việc là bắt buộc.")]
    public string EmploymentStatus { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lương là bắt buộc.")]
    [Range(0, double.MaxValue, ErrorMessage = "Lương phải lớn hơn hoặc bằng 0.")]
    public decimal Salary { get; set; }

    [StringLength(100, ErrorMessage = "Liên hệ khẩn cấp không được vượt quá 100 ký tự.")]
    public string? EmergencyContact { get; set; }

    [StringLength(20, ErrorMessage = "Số điện thoại khẩn cấp không được vượt quá 20 ký tự.")]
    public string? EmergencyPhone { get; set; }

    public string? Notes { get; set; }
}

    public class UpdateEmployeeDto
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        
        public string? Password { get; set; } // Optional password field for updates
        
        public int? DepartmentId { get; set; }
        public int? CompanyId { get; set; }
        public int? Level { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool? MustChangePassword { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public DateTime? LockedUntil { get; set; }
        public DateTime? PasswordExpiresAt { get; set; }
    }
