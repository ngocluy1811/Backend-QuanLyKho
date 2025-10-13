using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models;
using FertilizerWarehouseAPI.Models.DTOs;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Models.Enums;
using System.Security.Claims;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/employee
        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] string? department = null, [FromQuery] string? status = null, [FromQuery] string? search = null)
        {
            try
            {
                var employees = await _context.Users
                    .Include(u => u.Department)
                    .Select(u => new
                    {
                        u.Id,
                        u.Username,
                        Name = u.FullName,
                        Department = u.Department != null ? u.Department.Name : "N/A",
                        Position = u.Role.ToString(),
                        Status = u.IsActive == true ? "active" : "inactive",
                        Email = u.Email ?? "",
                        Phone = u.Phone ?? "",
                        Address = u.Address ?? "",
                        JoinDate = u.CreatedAt.ToString("yyyy-MM-dd"),
                        Salary = "N/A",
                        Avatar = "/avatars/default.jpg"
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = employees });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy danh sách nhân viên", error = ex.Message });
            }
        }

        // GET: api/employee/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            try
            {
                var employee = await _context.Users
                    .Include(u => u.Department)
                    .Where(u => u.Id == id)
                    .Select(u => new
                    {
                        u.Id,
                        u.Username,
                        Name = u.FullName,
                        Department = u.Department != null ? u.Department.Name : "N/A",
                        Position = u.Role.ToString(),
                        Status = u.IsActive == true ? "active" : "inactive",
                        Email = u.Email ?? "",
                        Phone = u.Phone ?? "",
                        Address = u.Address ?? "",
                        JoinDate = u.CreatedAt.ToString("yyyy-MM-dd"),
                        Salary = "N/A",
                        Avatar = "/avatars/default.jpg"
                    })
                    .FirstOrDefaultAsync();

                if (employee == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy nhân viên" });
                }

                return Ok(new { success = true, data = employee });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy thông tin nhân viên", error = ex.Message });
            }
        }

        // POST: api/employee
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto request)
        {
            try
            {
                // Check if username already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
                if (existingUser != null)
                {
                    return BadRequest(new { success = false, message = "Tên đăng nhập đã tồn tại" });
                }

                var user = new User
                {
                    Username = request.Username,
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Address = request.Address,
                    Position = request.Position,
                    DepartmentId = request.DepartmentId,
                    Role = request.Role,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Tạo nhân viên thành công", data = new { id = user.Id } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tạo nhân viên", error = ex.Message });
            }
        }

        // PUT: api/employee/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto request)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy nhân viên" });
                }

                user.FullName = request.FullName;
                user.Email = request.Email;
                user.Phone = request.Phone;
                user.Address = request.Address;
                user.Position = request.Position;
                user.DepartmentId = request.DepartmentId;
                
                // Handle password update if provided
                if (!string.IsNullOrEmpty(request.Password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    user.MustChangePassword = true;
                }
                
                // Convert string role to enum
                if (!string.IsNullOrEmpty(request.Role))
                {
                    string roleString = request.Role.ToLower(System.Globalization.CultureInfo.InvariantCulture);
                    switch (roleString)
                    {
                        case "admin":
                        case "1":
                            user.Role = Models.Enums.UserRole.Admin;
                            break;
                        case "warehouse":
                        case "2":
                            user.Role = Models.Enums.UserRole.Warehouse;
                            break;
                        case "team_leader":
                        case "teamleader":
                        case "3":
                            user.Role = Models.Enums.UserRole.TeamLeader;
                            break;
                        case "sales":
                        case "employee":
                        case "4":
                            user.Role = Models.Enums.UserRole.Sales;
                            break;
                    }
                }
                
                user.IsActive = request.IsActive;
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Cập nhật nhân viên thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật nhân viên", error = ex.Message });
            }
        }

        // DELETE: api/employee/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy nhân viên" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Xóa nhân viên thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa nhân viên", error = ex.Message });
            }
        }

        // GET: api/employee/departments
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var departments = await _context.Departments
                    .Select(d => new
                    {
                        d.Id,
                        d.Name,
                        d.Description
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = departments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy danh sách phòng ban", error = ex.Message });
            }
        }
    }

    public class CreateEmployeeDto
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public Models.Enums.UserRole Role { get; set; }
    }

    public class UpdateEmployeeDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? Password { get; set; } // Optional password field for updates
        public int? DepartmentId { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool? MustChangePassword { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public DateTime? LockedUntil { get; set; }
        public DateTime? PasswordExpiresAt { get; set; }
    }

}
