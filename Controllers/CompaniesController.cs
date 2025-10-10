using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/companies
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCompanies()
        {
            try
            {
                var companies = await _context.Companies
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    data = companies,
                    message = "Lấy danh sách công ty thành công"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting companies: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy danh sách công ty", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/companies/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCompany(int id)
        {
            try
            {
                var company = await _context.Companies.FindAsync(id);
                
                if (company == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Không tìm thấy công ty" 
                    });
                }

                return Ok(new { 
                    success = true, 
                    data = company,
                    message = "Lấy thông tin công ty thành công"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting company: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy thông tin công ty", 
                    error = ex.Message 
                });
            }
        }

        // POST: api/companies
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                if (string.IsNullOrWhiteSpace(request.CompanyName))
                {
                    return BadRequest(new { success = false, message = "Tên công ty không được để trống" });
                }

                // Check if company name already exists
                var existingCompany = await _context.Companies
                    .FirstOrDefaultAsync(c => c.CompanyName.ToLower() == request.CompanyName.ToLower());
                
                if (existingCompany != null)
                {
                    return BadRequest(new { success = false, message = "Tên công ty đã tồn tại" });
                }

                var company = new Company
                {
                    CompanyName = request.CompanyName,
                    Address = request.Address,
                    Phone = request.Phone,
                    Email = request.Email,
                    TaxCode = request.TaxCode,
                    Description = request.Description,
                    IsActive = request.IsActive ?? true,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                    UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    data = company,
                    message = "Tạo công ty thành công"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating company: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi tạo công ty", 
                    error = ex.Message 
                });
            }
        }

        // PUT: api/companies/{id}
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] UpdateCompanyRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var company = await _context.Companies.FindAsync(id);
                if (company == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy công ty" });
                }

                if (string.IsNullOrWhiteSpace(request.CompanyName))
                {
                    return BadRequest(new { success = false, message = "Tên công ty không được để trống" });
                }

                // Check if company name already exists (excluding current company)
                var existingCompany = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Id != id && c.CompanyName.ToLower() == request.CompanyName.ToLower());
                
                if (existingCompany != null)
                {
                    return BadRequest(new { success = false, message = "Tên công ty đã tồn tại" });
                }

                // Update company properties
                company.CompanyName = request.CompanyName;
                company.Address = request.Address;
                company.Phone = request.Phone;
                company.Email = request.Email;
                company.TaxCode = request.TaxCode;
                company.Description = request.Description;
                company.IsActive = request.IsActive ?? company.IsActive;
                company.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    data = company,
                    message = "Cập nhật công ty thành công"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating company: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi cập nhật công ty", 
                    error = ex.Message 
                });
            }
        }

        // DELETE: api/companies/{id}
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                var company = await _context.Companies.FindAsync(id);
                if (company == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy công ty" });
                }

                // Check if company has users
                var hasUsers = await _context.Users.AnyAsync(u => u.CompanyId == id);
                if (hasUsers)
                {
                    return BadRequest(new { success = false, message = "Không thể xóa công ty có người dùng" });
                }

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "Xóa công ty thành công"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting company: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi xóa công ty", 
                    error = ex.Message 
                });
            }
        }
    }

    // DTOs for Company operations
    public class CreateCompanyRequest
    {
        public string CompanyName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? TaxCode { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateCompanyRequest
    {
        public string CompanyName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? TaxCode { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}