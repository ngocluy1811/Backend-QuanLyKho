using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyInfo()
        {
            try
            {
                // Lấy thông tin công ty đầu tiên hoặc tạo mặc định
                var company = await _context.Companies.FirstOrDefaultAsync();
                
                if (company == null)
                {
                    // Tạo thông tin công ty mặc định nếu chưa có
                    company = new Company
                    {
                        CompanyName = "CÔNG TY TNHH AQUACHEM",
                        TaxCode = "AQC",
                        Phone = "0326302451",
                        Email = "vongocluy12345@gmail.com",
                        Address = "Thôn Phú Hà, Xã Mỹ Đức, Huyện Phù Mỹ, Tỉnh Bình Định",
                        Department = "Quản lý kho",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    companyName = company.CompanyName,
                    taxCode = company.TaxCode,
                    phone = company.Phone,
                    email = company.Email,
                    address = company.Address,
                    department = company.Department
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin công ty", error = ex.Message });
            }
        }
    }
}
