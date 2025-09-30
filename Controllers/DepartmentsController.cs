using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/departments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetDepartments()
    {
        var departments = await _context.Departments
            .Where(d => d.IsActive)
            .Select(d => new
            {
                Id = d.Id,
                Name = d.Name ?? "",
                Code = d.Code ?? "",
                Description = d.Description ?? "",
                ManagerId = d.ManagerId,
                CompanyId = d.CompanyId ?? 0,
                Status = d.Status ?? "Active",
                IsActive = d.IsActive,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt ?? DateTime.UtcNow,
                ManagerName = d.Manager != null ? d.Manager.FullName : null,
                CompanyName = d.Company != null ? d.Company.CompanyName : null,
                UserCount = d.Users.Count(u => u.IsActive)
            })
            .ToListAsync();

        return Ok(departments);
    }

    // GET: api/departments/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetDepartment(int id)
    {
        var department = await _context.Departments
            .Where(d => d.Id == id && d.IsActive)
            .Select(d => new
            {
                Id = d.Id,
                Name = d.Name ?? "",
                Code = d.Code ?? "",
                Description = d.Description ?? "",
                ManagerId = d.ManagerId,
                CompanyId = d.CompanyId ?? 0,
                Status = d.Status ?? "Active",
                IsActive = d.IsActive,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt ?? DateTime.UtcNow,
                ManagerName = d.Manager != null ? d.Manager.FullName : null,
                CompanyName = d.Company != null ? d.Company.CompanyName : null,
                UserCount = d.Users.Count(u => u.IsActive)
            })
            .FirstOrDefaultAsync();

        if (department == null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    // POST: api/departments
    [HttpPost]
    public async Task<ActionResult<Department>> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        try
        {
            Console.WriteLine($"Creating department with data: Name={request.Name}, Description={request.Description}, CompanyId={request.CompanyId}");
            
            var department = new Department
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                ManagerId = null, // TODO: Add manager selection if needed
                CompanyId = request.CompanyId,
                Status = request.Status,
                IsActive = request.IsActive,
                CreatedBy = 0, // TODO: Get from current user
                CreatedAt = DateTime.UtcNow
            };

            _context.Departments.Add(department);
            Console.WriteLine("Saving new department...");
            
            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"Department created with ID: {department.Id}");
            }
            catch (Exception saveEx)
            {
                Console.WriteLine($"Error saving department: {saveEx.Message}");
                Console.WriteLine($"Inner exception: {saveEx.InnerException?.Message}");
                throw;
            }

            // Return department with related data
            var createdDepartment = await _context.Departments
                .Include(d => d.Company)
                .Include(d => d.Manager)
                .Where(d => d.Id == department.Id)
                .Select(d => new
                {
                    Id = d.Id,
                    Name = d.Name ?? "",
                    Code = d.Code ?? "",
                    Description = d.Description ?? "",
                    CompanyId = d.CompanyId ?? 0,
                    ManagerId = d.ManagerId,
                    Status = d.Status ?? "Active",
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    ManagerName = d.Manager != null ? d.Manager.FullName : null,
                    CompanyName = d.Company != null ? d.Company.CompanyName : null,
                    UserCount = d.Users.Count(u => u.IsActive)
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, createdDepartment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Lỗi khi tạo phòng ban", error = ex.Message });
        }
    }

    // PUT: api/departments/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentRequest request)
    {
        try
        {
            Console.WriteLine($"Updating department {id} with data: Name={request.Name}, Code={request.Code}, Description={request.Description}, CompanyId={request.CompanyId}, Status={request.Status}");
            
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                Console.WriteLine($"Department with ID {id} not found");
                return NotFound();
            }
            
            Console.WriteLine($"Found department: Name={department.Name}, Code={department.Code}, Status={department.Status}");

            department.Name = request.Name;
            department.Code = request.Code;
            department.Description = request.Description;
            department.ManagerId = null; // TODO: Add manager selection if needed
            department.CompanyId = request.CompanyId;
            department.Status = request.Status;
            department.IsActive = request.IsActive;
            department.UpdatedAt = DateTime.UtcNow;
            department.UpdatedBy = null; // TODO: Get from current user
            
            // Fix NULL values for old departments
            if (department.CreatedBy == null)
            {
                department.CreatedBy = 0;
            }

            Console.WriteLine($"Saving changes for department {id}...");
            Console.WriteLine($"Department before save: Name={department.Name}, Code={department.Code}, Status={department.Status}, CompanyId={department.CompanyId}");
            
            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"Department {id} updated successfully");
            }
            catch (Exception saveEx)
            {
                Console.WriteLine($"Error updating department: {saveEx.Message}");
                Console.WriteLine($"Inner exception: {saveEx.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {saveEx.StackTrace}");
                throw;
            }

            // Return updated department with related data
            Console.WriteLine($"Building response for department {id}...");
            var updatedDepartment = await _context.Departments
                .Include(d => d.Company)
                .Include(d => d.Manager)
                .Where(d => d.Id == department.Id)
                .Select(d => new
                {
                    Id = d.Id,
                    Name = d.Name ?? "",
                    Code = d.Code ?? "",
                    Description = d.Description ?? "",
                    CompanyId = d.CompanyId ?? 0,
                    ManagerId = d.ManagerId,
                    Status = d.Status ?? "Active",
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt ?? DateTime.UtcNow,
                    ManagerName = d.Manager != null ? d.Manager.FullName : null,
                    CompanyName = d.Company != null ? d.Company.CompanyName : null,
                    UserCount = d.Users.Count(u => u.IsActive)
                })
                .FirstOrDefaultAsync();

            Console.WriteLine($"Response built successfully for department {id}");
            return Ok(updatedDepartment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Lỗi khi cập nhật phòng ban", error = ex.Message });
        }
    }

    // DELETE: api/departments/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        try
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            // Soft delete
            department.IsActive = false;
            department.UpdatedAt = DateTime.UtcNow;
            department.UpdatedBy = null; // TODO: Get from current user

            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa phòng ban thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Lỗi khi xóa phòng ban", error = ex.Message });
        }
    }

    // GET: api/departments/test-db
    [HttpGet("test-db")]
    public async Task<IActionResult> TestDatabaseConnection()
    {
        try
        {
            Console.WriteLine("Testing database connection...");
            var departmentCount = await _context.Departments.CountAsync();
            Console.WriteLine($"Database connection successful. Department count: {departmentCount}");
            return Ok(new { 
                message = "Database connection successful", 
                departmentCount = departmentCount,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection failed: {ex.Message}");
            return StatusCode(500, new { 
                message = "Database connection failed", 
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    // POST: api/departments/clear-and-seed-real-departments
    [HttpPost("clear-and-seed-real-departments")]
    public async Task<IActionResult> ClearAndSeedRealDepartments()
    {
        try
        {
            // Clear all existing departments
            var existingDepartments = await _context.Departments.ToListAsync();
            _context.Departments.RemoveRange(existingDepartments);
            await _context.SaveChangesAsync();

            // Get companies first
            var companies = await _context.Companies.ToListAsync();
            if (!companies.Any())
            {
                return BadRequest(new { message = "No companies found. Please seed companies first." });
            }

            var departments = new List<Department>
            {
                new Department
                {
                    Name = "Phòng Kỹ thuật",
                    Description = "Phòng ban phụ trách kỹ thuật và công nghệ",
                    CompanyId = companies.First().Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1
                },
                new Department
                {
                    Name = "Phòng Kinh doanh",
                    Description = "Phòng ban phụ trách kinh doanh và bán hàng",
                    CompanyId = companies.First().Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1
                },
                new Department
                {
                    Name = "Phòng Nhân sự",
                    Description = "Phòng ban phụ trách nhân sự và quản lý nhân viên",
                    CompanyId = companies.First().Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1
                },
                new Department
                {
                    Name = "Phòng Kế toán",
                    Description = "Phòng ban phụ trách kế toán và tài chính",
                    CompanyId = companies.First().Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1
                },
                new Department
                {
                    Name = "Phòng Sản xuất",
                    Description = "Phòng ban phụ trách sản xuất và vận hành",
                    CompanyId = companies.First().Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1
                }
            };

            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Real departments seeded successfully", 
                count = departments.Count,
                departments = departments.Select(d => new { d.Id, d.Name, d.CompanyId })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while seeding departments", error = ex.Message });
        }
    }
}

public class CreateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CompanyId { get; set; }
    public string Status { get; set; } = "Active";
    public bool IsActive { get; set; } = true;
}

public class UpdateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CompanyId { get; set; }
    public string Status { get; set; } = "Active";
    public bool IsActive { get; set; } = true;
}