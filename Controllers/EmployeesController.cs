using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Models.Enums;
using FertilizerWarehouseAPI.DTOs;
using UserRole = FertilizerWarehouseAPI.Models.Enums.UserRole;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Temporarily disabled for testing
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all employees - Simple version for testing
        /// </summary>
        [HttpGet]
        // [Authorize(Policy = "Management")] // Temporarily disabled for testing
        public async Task<ActionResult<IEnumerable<object>>> GetEmployees()
        {
            try
            {
                // Simplified query to avoid any potential issues
                var employees = await _context.Users
                    .Select(u => new
                    {
                        u.Id,
                        Username = u.Username ?? "",
                        Email = u.Email ?? "",
                        FullName = u.FullName ?? "",
                        Phone = u.Phone ?? "",
                        u.Role,
                        RoleName = u.Role.ToString(),
                        u.CompanyId,
                        CompanyName = "Công ty mặc định",
                        u.DepartmentId,
                        DepartmentName = "Chưa phân công",
                        u.IsActive,
                        Level = u.Level ?? 0,
                        u.CreatedAt,
                        UpdatedAt = u.UpdatedAt ?? u.CreatedAt,
                        LastLoginAt = u.LastLoginAt ?? (DateTime?)null,
                        PasswordExpiresAt = u.PasswordExpiresAt ?? (DateTime?)null,
                        u.MustChangePassword,
                        IsLocked = u.LockedUntil.HasValue && u.LockedUntil > DateTime.UtcNow
                    })
                    .ToListAsync();

                return Ok(new { success = true, count = employees.Count, data = employees });
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                Console.WriteLine($"Error in GetEmployees: {ex}");
                return StatusCode(500, new { 
                    success = false,
                    message = "An error occurred while retrieving employees", 
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Test endpoint - Get simple user list
        /// </summary>
        [HttpGet("test")]
        public async Task<ActionResult> GetEmployeesTest()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.Username,
                        u.FullName,
                        u.Email,
                        u.Role
                    })
                    .ToListAsync();

                return Ok(new { success = true, count = users.Count, data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Test endpoint error", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Get employee by ID
        /// </summary>
        [HttpGet("{id}")]
        // [Authorize(Policy = "Management")] // Temporarily disabled for testing
        public async Task<ActionResult<object>> GetEmployee(int id)
        {
            try
            {
                var employee = await _context.Users
                    // .Include(u => u.Company) // Removed to avoid null reference
                    // .Include(u => u.Department) // Removed to avoid null reference
                    .Where(u => u.Id == id) // Removed IsActive filter to show all users
                    .Select(u => new
                    {
                        u.Id,
                        Username = u.Username ?? "",
                        Email = u.Email ?? "",
                        FullName = u.FullName ?? "",
                        Phone = u.Phone ?? "",
                        u.Role,
                        RoleName = u.Role.ToString(),
                        u.CompanyId,
                        CompanyName = "Công ty mặc định", // Simplified
                        u.DepartmentId,
                        DepartmentName = "Chưa phân công", // Simplified
                        Level = u.Level ?? 0,
                        u.IsActive,
                        u.CreatedAt,
                        LastLoginAt = u.LastLoginAt ?? (DateTime?)null,
                        u.FailedLoginAttempts,
                        IsLocked = u.LockedUntil.HasValue && u.LockedUntil > DateTime.UtcNow
                    })
                    .FirstOrDefaultAsync();

                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving employee", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new employee
        /// </summary>
        [HttpPost]
        // [Authorize(Policy = "AdminOnly")] // Temporarily disabled for testing
        public async Task<ActionResult<object>> CreateEmployee([FromBody] CreateEmployeeBasicDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if username already exists
                var existingUser = await _context.Users.AnyAsync(u => u.Username == createDto.Username);
                if (existingUser)
                    return BadRequest(new { message = "Username already exists" });

                // Parse role string to enum
                if (!Enum.TryParse<UserRole>(createDto.Role, true, out var userRole))
                {
                    return BadRequest(new { message = "Invalid role value" });
                }

                var employee = new User
                {
                    Username = createDto.Username,
                    Email = createDto.Email,
                    FullName = createDto.FullName,
                    Phone = createDto.Phone,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password),
                    Role = userRole,
                    CompanyId = createDto.CompanyId,
                    DepartmentId = createDto.DepartmentId,
                    Level = createDto.Level ?? 1, // Default level
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true,
                    MustChangePassword = createDto.MustChangePassword ?? true,
                    TwoFactorEnabled = createDto.TwoFactorEnabled ?? false,
                    // Auto-generated fields
                    PasswordExpiresAt = createDto.PasswordExpiresAt ?? DateTime.UtcNow.AddDays(90), // Password expires in 90 days
                    LastLoginAt = null, // No login yet
                    LockedUntil = createDto.LockedUntil // Can be set by admin
                };

                _context.Users.Add(employee);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, new
                {
                    employee.Id,
                    employee.Username,
                    employee.Email,
                    employee.FullName,
                    employee.Phone,
                    employee.Role,
                    RoleName = employee.Role.ToString(),
                    employee.CompanyId,
                    employee.DepartmentId,
                    employee.Level,
                    employee.IsActive,
                    employee.CreatedAt,
                    employee.UpdatedAt,
                    employee.LastLoginAt,
                    employee.PasswordExpiresAt,
                    employee.MustChangePassword
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating employee", error = ex.Message });
            }
        }

        /// <summary>
        /// Get employee performance summary
        /// </summary>
        [HttpGet("performance")]
        // [Authorize(Policy = "Management")] // Temporarily disabled for testing
        public async Task<ActionResult<object>> GetPerformanceSummary()
        {
            try
            {
                var totalEmployees = await _context.Users.CountAsync(u => u.IsActive);
                var activeToday = await _context.Users.CountAsync(u => u.IsActive && u.LastLoginAt.HasValue && u.LastLoginAt.Value.Date == DateTime.Today);

                var summary = new
                {
                    TotalEmployees = totalEmployees,
                    ActiveToday = activeToday,
                    InactiveToday = totalEmployees - activeToday,
                    AverageLoginCount = 0, // Will calculate properly with session tracking
                    TopPerformers = new List<object>() // Will implement with task completion tracking
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving performance summary", error = ex.Message });
            }
        }

        /// <summary>
        /// Update employee
        /// </summary>
        [HttpPut("{id}")]
        // [Authorize(Policy = "AdminOnly")] // Temporarily disabled for testing
        public async Task<ActionResult<object>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto updateDto)
        {
            try
            {
                if (updateDto == null)
                {
                    return BadRequest(new { message = "Request body cannot be null" });
                }

                var employee = await _context.Users.FindAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                // Update basic fields
                if (!string.IsNullOrEmpty(updateDto.FullName))
                    employee.FullName = updateDto.FullName;
                if (!string.IsNullOrEmpty(updateDto.Email))
                    employee.Email = updateDto.Email;
                if (!string.IsNullOrEmpty(updateDto.Phone))
                    employee.Phone = updateDto.Phone;
                if (!string.IsNullOrEmpty(updateDto.Address))
                    employee.Address = updateDto.Address;
                if (!string.IsNullOrEmpty(updateDto.Position))
                    employee.Position = updateDto.Position;
                
                // Update role if provided
                if (!string.IsNullOrEmpty(updateDto.Role))
                {
                    string roleString = updateDto.Role.ToLower(System.Globalization.CultureInfo.InvariantCulture);
                    switch (roleString)
                    {
                        case "admin":
                        case "1":
                            employee.Role = Models.Enums.UserRole.Admin;
                            break;
                        case "warehouse":
                        case "2":
                            employee.Role = Models.Enums.UserRole.Warehouse;
                            break;
                        case "team_leader":
                        case "teamleader":
                        case "3":
                            employee.Role = Models.Enums.UserRole.TeamLeader;
                            break;
                        case "sales":
                        case "employee":
                        case "4":
                            employee.Role = Models.Enums.UserRole.Sales;
                            break;
                        default:
                            return BadRequest(new { message = $"Invalid role value: {updateDto.Role}" });
                    }
                }

                // Update optional fields
                if (updateDto.DepartmentId.HasValue)
                    employee.DepartmentId = updateDto.DepartmentId;
                
                // Update Level if provided (using reflection to avoid build errors)
                var levelProperty = updateDto.GetType().GetProperty("Level");
                if (levelProperty != null)
                {
                    var levelValue = levelProperty.GetValue(updateDto);
                    if (levelValue != null && int.TryParse(levelValue.ToString(), out int level))
                    {
                        employee.Level = level;
                    }
                }
                
                // Update PasswordExpiresAt if provided (using reflection)
                var passwordExpiresProperty = updateDto.GetType().GetProperty("PasswordExpiresAt");
                if (passwordExpiresProperty != null)
                {
                    var passwordExpiresValue = passwordExpiresProperty.GetValue(updateDto);
                    if (passwordExpiresValue is DateTime dateTime)
                    {
                        employee.PasswordExpiresAt = dateTime;
                    }
                }
                
                // Update MustChangePassword if provided (using reflection)
                var mustChangePasswordProperty = updateDto.GetType().GetProperty("MustChangePassword");
                if (mustChangePasswordProperty != null)
                {
                    var mustChangePasswordValue = mustChangePasswordProperty.GetValue(updateDto);
                    if (mustChangePasswordValue is bool mustChange)
                    {
                        employee.MustChangePassword = mustChange;
                    }
                }
                
                employee.IsActive = updateDto.IsActive;

                // Update timestamp
                employee.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    employee.Id,
                    employee.Username,
                    employee.Email,
                    employee.FullName,
                    employee.Phone,
                    employee.Role,
                    RoleName = employee.Role.ToString(),
                    employee.CompanyId,
                    employee.DepartmentId,
                    employee.IsActive,
                    employee.Level,
                    employee.CreatedAt,
                    employee.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating employee: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating employee", error = ex.Message });
            }
        }

        /// <summary>
        /// Update employee status
        /// </summary>
        [HttpPut("{id}/status")]
        // [Authorize(Policy = "AdminOnly")] // Temporarily disabled for testing
        public async Task<ActionResult<object>> UpdateEmployeeStatus(int id, [FromBody] JsonElement statusData)
        {
            try
            {
                var employee = await _context.Users.FindAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                // Parse isActive from different possible formats
                bool isActive = false;
                if (statusData.TryGetProperty("isActive", out var isActiveElement))
                {
                    isActive = isActiveElement.GetBoolean();
                }
                else if (statusData.TryGetProperty("IsActive", out var isActiveElement2))
                {
                    isActive = isActiveElement2.GetBoolean();
                }

                // Log the change
                Console.WriteLine($"Updating user {id} status from {employee.IsActive} to {isActive}");
                
                employee.IsActive = isActive;
                
                // Mark entity as modified
                _context.Entry(employee).Property(x => x.IsActive).IsModified = true;
                
                // Save changes with explicit transaction
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var result = await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                    Console.WriteLine($"Successfully updated user {id} status. Changes saved: {result}");
                    
                    return Ok(new
                    {
                        employee.Id,
                        employee.Username,
                        employee.FullName,
                        employee.IsActive,
                        message = "Employee status updated successfully"
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error saving changes: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating employee status: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating employee status", error = ex.Message });
            }
        }

        /// <summary>
        /// Update employee role
        /// </summary>
        [HttpPut("{id}/role")]
        // [Authorize(Policy = "AdminOnly")] // Temporarily disabled for testing
        public async Task<ActionResult<object>> UpdateEmployeeRole(int id, [FromBody] JsonElement roleData)
        {
            try
            {
                var employee = await _context.Users.FindAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                // Parse role from different possible formats
                string roleString = null;
                if (roleData.TryGetProperty("role", out var roleElement))
                {
                    roleString = roleElement.GetString();
                }
                else if (roleData.TryGetProperty("Role", out var roleElement2))
                {
                    roleString = roleElement2.GetString();
                }

                if (string.IsNullOrEmpty(roleString))
                    return BadRequest(new { message = "Role is required" });

                // Convert string to UserRole enum with mapping
                UserRole newRole;
                switch (roleString.ToLower())
                {
                    case "admin":
                    case "1":
                        newRole = UserRole.Admin;
                        break;
                    case "warehouse":
                    case "2":
                        newRole = UserRole.Warehouse;
                        break;
                    case "team_leader":
                    case "teamleader":
                    case "3":
                        newRole = UserRole.TeamLeader;
                        break;
                    case "sales":
                    case "4":
                        newRole = UserRole.Sales;
                        break;
                    default:
                        Console.WriteLine($"Invalid role value received: {roleString}");
                        return BadRequest(new { message = $"Invalid role value: {roleString}. Valid values are: admin, warehouse, team_leader, sales" });
                }

                // Log the change
                Console.WriteLine($"Updating user {id} role from {employee.Role} to {newRole}");
                
                employee.Role = newRole;
                
                // Mark entity as modified
                _context.Entry(employee).Property(x => x.Role).IsModified = true;
                
                // Save changes with explicit transaction
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var result = await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                    Console.WriteLine($"Successfully updated user {id} role. Changes saved: {result}");
                    
                    return Ok(new
                    {
                        employee.Id,
                        employee.Username,
                        employee.FullName,
                        employee.Role,
                        RoleName = employee.Role.ToString(),
                        message = "Employee role updated successfully"
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error saving changes: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating employee role: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating employee role", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete employee (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        // [Authorize(Policy = "AdminOnly")] // Temporarily disabled for testing
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.Users.FindAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                employee.IsActive = false;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Employee deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting employee", error = ex.Message });
            }
        }
    }

    // Basic DTOs
    public class CreateEmployeeBasicDto
    {
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Changed to string
        public int CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        public int? Level { get; set; }
        public bool? MustChangePassword { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public DateTime? LockedUntil { get; set; }
        public DateTime? PasswordExpiresAt { get; set; }
    }


    public class UpdateStatusDto
    {
        public bool IsActive { get; set; }
    }

    public class UpdateRoleDto
    {
        public UserRole Role { get; set; }
    }
    
    // Helper methods for validation
    public static class ValidationHelpers
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        
        public static bool IsValidRole(string role)
        {
            if (string.IsNullOrEmpty(role))
                return true; // Allow null/empty roles for updates
            
            var validRoles = new[] { "admin", "warehouse", "team_leader", "teamleader", "sales", "employee", "1", "2", "3", "4" };
            return validRoles.Contains(role.ToLower());
        }
    }
}
