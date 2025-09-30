using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Models.Enums;
using UserRole = FertilizerWarehouseAPI.Models.Enums.UserRole;
using System.Text.Json;

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
        /// Get all employees
        /// </summary>
        [HttpGet]
        // [Authorize(Policy = "Management")] // Temporarily disabled for testing
        public async Task<ActionResult<IEnumerable<object>>> GetEmployees()
        {
            try
            {
                var employees = await _context.Users
                    .Include(u => u.Company)
                    .Include(u => u.Department)
                    // .Where(u => u.IsActive) // Removed to show all users including inactive
                    .Select(u => new
                    {
                        u.Id,
                        u.Username,
                        u.Email,
                        u.FullName,
                        u.Phone,
                        u.Role,
                        RoleName = u.Role.ToString(),
                        u.CompanyId,
                        CompanyName = u.Company.CompanyName,
                        u.DepartmentId,
                        DepartmentName = u.Department != null ? u.Department.Name : null,
                        u.IsActive,
                        u.CreatedAt,
                        u.LastLoginAt,
                        IsLocked = u.LockedUntil > DateTime.UtcNow
                    })
                    .ToListAsync();

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving employees", error = ex.Message });
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
                    .Include(u => u.Company)
                    .Include(u => u.Department)
                    .Where(u => u.Id == id && u.IsActive)
                    .Select(u => new
                    {
                        u.Id,
                        u.Username,
                        u.Email,
                        u.FullName,
                        u.Phone,
                        u.Role,
                        RoleName = u.Role.ToString(),
                        u.CompanyId,
                        CompanyName = u.Company.CompanyName,
                        u.DepartmentId,
                        DepartmentName = u.Department != null ? u.Department.Name : null,
                        u.Level,
                        u.IsActive,
                        u.CreatedAt,
                        u.LastLoginAt,
                        u.FailedLoginAttempts,
                        IsLocked = u.LockedUntil > DateTime.UtcNow
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

                var employee = new User
                {
                    Username = createDto.Username,
                    Email = createDto.Email,
                    FullName = createDto.FullName,
                    Phone = createDto.Phone,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password),
                    Role = createDto.Role,
                    CompanyId = createDto.CompanyId,
                    DepartmentId = createDto.DepartmentId,
                    Level = createDto.Level,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    MustChangePassword = true
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
                    employee.CreatedAt
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
                var employee = await _context.Users.FindAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                // Update fields
                if (!string.IsNullOrEmpty(updateDto.Username))
                    employee.Username = updateDto.Username;
                if (!string.IsNullOrEmpty(updateDto.Email))
                    employee.Email = updateDto.Email;
                if (!string.IsNullOrEmpty(updateDto.FullName))
                    employee.FullName = updateDto.FullName;
                if (updateDto.Phone != null)
                    employee.Phone = updateDto.Phone;
                if (updateDto.Role.HasValue)
                    employee.Role = updateDto.Role.Value;
                if (updateDto.CompanyId.HasValue)
                    employee.CompanyId = updateDto.CompanyId.Value;
                if (updateDto.DepartmentId.HasValue)
                    employee.DepartmentId = updateDto.DepartmentId;
                if (updateDto.IsActive.HasValue)
                    employee.IsActive = updateDto.IsActive.Value;

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
                    employee.CreatedAt,
                    employee.LastLoginAt
                });
            }
            catch (Exception ex)
            {
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
        public UserRole Role { get; set; }
        public int CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        public int? Level { get; set; }
    }

    public class UpdateEmployeeDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public UserRole? Role { get; set; }
        public int? CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateStatusDto
    {
        public bool IsActive { get; set; }
    }

    public class UpdateRoleDto
    {
        public UserRole Role { get; set; }
    }
}
