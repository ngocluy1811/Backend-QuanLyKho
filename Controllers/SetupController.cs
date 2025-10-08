using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models;
using BCrypt.Net;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetupController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SetupController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin()
        {
            try
            {
                // Kiểm tra xem đã có admin chưa
                var existingAdmin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
                if (existingAdmin != null)
                {
                    return Ok(new { message = "Admin user already exists", username = "admin" });
                }

                // Tạo company mặc định
                var company = new Models.Entities.Company
                {
                    Id = 1,
                    CompanyName = "Công ty TNHH Quản lý Kho",
                    Address = "Hà Nội, Việt Nam",
                    Phone = "0123456789",
                    Email = "info@company.com",
                    TaxCode = "0123456789",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Companies.Add(company);

                // Tạo department mặc định
                var department = new Models.Entities.Department
                {
                    Id = 1,
                    Name = "Phòng Quản lý",
                    Description = "Phòng quản lý chung",
                    CompanyId = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Departments.Add(department);

                // Tạo admin user
                var adminUser = new Models.Entities.User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FullName = "Administrator",
                    Email = "admin@company.com",
                    Phone = "0123456789",
                    Role = Models.Enums.UserRole.Admin,
                    DepartmentId = 1,
                    CompanyId = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(adminUser);

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Admin user created successfully", 
                    username = "admin",
                    password = "admin123"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating admin user", error = ex.Message });
            }
        }

        [HttpGet("check-admin")]
        public async Task<IActionResult> CheckAdmin()
        {
            try
            {
                var admin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
                if (admin != null)
                {
                    return Ok(new { 
                        exists = true, 
                        username = admin.Username,
                        fullName = admin.FullName,
                        role = admin.Role
                    });
                }
                return Ok(new { exists = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error checking admin user", error = ex.Message });
            }
        }
    }
}
