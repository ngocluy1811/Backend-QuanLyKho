using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PermissionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all available permissions
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPermissions()
    {
        try
        {
            var permissions = await _context.Permissions
                .Where(p => p.IsActive)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Category,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return Ok(permissions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving permissions", error = ex.Message });
        }
    }

    /// <summary>
    /// Get user permissions
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPermissions(int userId)
    {
        try
        {
            var userPermissions = await _context.UserPermissions
                .Where(up => up.UserId == userId && up.IsActive)
                .Select(up => new
                {
                    up.PermissionId,
                    up.IsGranted,
                    up.GrantedAt,
                    up.GrantedBy
                })
                .ToListAsync();

            return Ok(userPermissions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving user permissions", error = ex.Message });
        }
    }

    /// <summary>
    /// Update user permissions
    /// </summary>
    [HttpPut("user/{userId}")]
    public async Task<IActionResult> UpdateUserPermissions(int userId, [FromBody] UpdateUserPermissionsRequest request)
    {
        try
        {
            // Remove existing permissions for this user
            var existingPermissions = await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .ToListAsync();

            _context.UserPermissions.RemoveRange(existingPermissions);

            // Add new permissions
            var newPermissions = request.PermissionIds.Select(permissionId => new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                IsGranted = true,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = 1, // TODO: Get from current user
                IsActive = true
            }).ToList();

            _context.UserPermissions.AddRange(newPermissions);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User permissions updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating user permissions", error = ex.Message });
        }
    }

    /// <summary>
    /// Clear all permissions
    /// </summary>
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearPermissions()
    {
        try
        {
            var permissions = await _context.Permissions.ToListAsync();
            var userPermissions = await _context.UserPermissions.ToListAsync();
            
            _context.UserPermissions.RemoveRange(userPermissions);
            _context.Permissions.RemoveRange(permissions);
            await _context.SaveChangesAsync();

            return Ok(new { message = "All permissions cleared successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while clearing permissions", error = ex.Message });
        }
    }

    /// <summary>
    /// Test endpoint
    /// </summary>
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { message = "Permissions controller is working" });
    }

    /// <summary>
    /// Seed default permissions
    /// </summary>
    [HttpPost("seed")]
    public async Task<IActionResult> SeedPermissions()
    {
        try
        {
            // Clear existing permissions first
            var existingPermissions = await _context.Permissions.ToListAsync();
            var existingUserPermissions = await _context.UserPermissions.ToListAsync();
            
            _context.UserPermissions.RemoveRange(existingUserPermissions);
            _context.Permissions.RemoveRange(existingPermissions);
            await _context.SaveChangesAsync();

            var permissions = new List<Permission>
            {
                // Tổng quan
                new Permission { Name = "Xem tổng quan", Description = "Xem dashboard tổng quan", Category = "Tổng quan" },
                new Permission { Name = "Tùy chỉnh tổng quan", Description = "Tùy chỉnh giao diện dashboard", Category = "Tổng quan" },
                new Permission { Name = "Xem phân tích", Description = "Xem các biểu đồ phân tích", Category = "Tổng quan" },
                
                // Warehouse Management
                new Permission { Name = "Xem thông tin kho", Description = "Xem thông tin kho hàng", Category = "Quản lý kho" },
                new Permission { Name = "Chỉnh sửa kho", Description = "Chỉnh sửa thông tin kho", Category = "Quản lý kho" },
                new Permission { Name = "Xem chi tiết ô", Description = "Xem chi tiết từng ô trong kho", Category = "Quản lý kho" },
                new Permission { Name = "Quản lý vị trí", Description = "Quản lý vị trí các ô trong kho", Category = "Quản lý kho" },
                new Permission { Name = "Tùy chỉnh kho", Description = "Tùy chỉnh kích thước và layout kho", Category = "Quản lý kho" },
                new Permission { Name = "Xem sơ đồ 3D", Description = "Xem sơ đồ kho 3D", Category = "Quản lý kho" },
                new Permission { Name = "Quản lý cluster", Description = "Quản lý các cluster trong kho", Category = "Quản lý kho" },
                new Permission { Name = "Quản lý zone", Description = "Quản lý các zone trong kho", Category = "Quản lý kho" },
                new Permission { Name = "Quản lý cell", Description = "Quản lý các cell trong kho", Category = "Quản lý kho" },
                new Permission { Name = "Theo dõi hàng hóa", Description = "Theo dõi vị trí hàng hóa", Category = "Quản lý kho" },
                new Permission { Name = "Kiểm kê kho", Description = "Thực hiện kiểm kê kho", Category = "Quản lý kho" },
                new Permission { Name = "Báo cáo kho", Description = "Xem báo cáo tình trạng kho", Category = "Quản lý kho" },
                
                // Product Management
                new Permission { Name = "Xem sản phẩm", Description = "Xem danh sách sản phẩm", Category = "Sản phẩm" },
                new Permission { Name = "Tạo sản phẩm", Description = "Thêm sản phẩm mới", Category = "Sản phẩm" },
                new Permission { Name = "Chỉnh sửa sản phẩm", Description = "Cập nhật thông tin sản phẩm", Category = "Sản phẩm" },
                new Permission { Name = "Xóa sản phẩm", Description = "Xóa sản phẩm", Category = "Sản phẩm" },
                new Permission { Name = "Quản lý danh mục", Description = "Quản lý danh mục sản phẩm", Category = "Sản phẩm" },
                new Permission { Name = "Quản lý nhà cung cấp", Description = "Quản lý thông tin nhà cung cấp", Category = "Sản phẩm" },
                new Permission { Name = "Quản lý khách hàng", Description = "Quản lý thông tin khách hàng", Category = "Sản phẩm" },
                
                // User Management
                new Permission { Name = "Xem người dùng", Description = "Xem danh sách người dùng", Category = "Người dùng" },
                new Permission { Name = "Tạo người dùng", Description = "Thêm người dùng mới", Category = "Người dùng" },
                new Permission { Name = "Chỉnh sửa người dùng", Description = "Cập nhật thông tin người dùng", Category = "Người dùng" },
                new Permission { Name = "Xóa người dùng", Description = "Xóa người dùng", Category = "Người dùng" },
                new Permission { Name = "Quản lý vai trò", Description = "Phân quyền vai trò người dùng", Category = "Người dùng" },
                new Permission { Name = "Quản lý trạng thái", Description = "Quản lý trạng thái người dùng", Category = "Người dùng" },
                
                // Department Management
                new Permission { Name = "Xem phòng ban", Description = "Xem danh sách phòng ban", Category = "Phòng ban" },
                new Permission { Name = "Tạo phòng ban", Description = "Thêm phòng ban mới", Category = "Phòng ban" },
                new Permission { Name = "Chỉnh sửa phòng ban", Description = "Cập nhật thông tin phòng ban", Category = "Phòng ban" },
                new Permission { Name = "Xóa phòng ban", Description = "Xóa phòng ban", Category = "Phòng ban" },
                new Permission { Name = "Quản lý trưởng phòng", Description = "Phân công trưởng phòng", Category = "Phòng ban" },
                
                // Company Management
                new Permission { Name = "Xem công ty", Description = "Xem danh sách công ty", Category = "Công ty" },
                new Permission { Name = "Tạo công ty", Description = "Thêm công ty mới", Category = "Công ty" },
                new Permission { Name = "Chỉnh sửa công ty", Description = "Cập nhật thông tin công ty", Category = "Công ty" },
                new Permission { Name = "Xóa công ty", Description = "Xóa công ty", Category = "Công ty" },
                new Permission { Name = "Quản lý trạng thái công ty", Description = "Quản lý trạng thái hoạt động công ty", Category = "Công ty" },
                
                // Reports
                new Permission { Name = "Xem báo cáo", Description = "Xem các báo cáo thống kê", Category = "Báo cáo" },
                new Permission { Name = "Xuất báo cáo", Description = "Xuất báo cáo ra file", Category = "Báo cáo" },
                new Permission { Name = "Báo cáo kho", Description = "Xem báo cáo tình trạng kho", Category = "Báo cáo" },
                new Permission { Name = "Báo cáo sản phẩm", Description = "Xem báo cáo sản phẩm", Category = "Báo cáo" },
                new Permission { Name = "Báo cáo người dùng", Description = "Xem báo cáo người dùng", Category = "Báo cáo" },
                new Permission { Name = "Báo cáo tài chính", Description = "Xem báo cáo tài chính", Category = "Báo cáo" },
                
                // Admin
                new Permission { Name = "Truy cập Admin Panel", Description = "Truy cập trang quản trị", Category = "Quản trị" },
                new Permission { Name = "Cài đặt hệ thống", Description = "Cấu hình hệ thống", Category = "Quản trị" },
                new Permission { Name = "Quản lý backup", Description = "Quản lý sao lưu dữ liệu", Category = "Quản trị" },
                new Permission { Name = "Quản lý log", Description = "Xem và quản lý log hệ thống", Category = "Quản trị" },
                new Permission { Name = "Quản lý cấu hình", Description = "Cấu hình các tham số hệ thống", Category = "Quản trị" }
            };

            _context.Permissions.AddRange(permissions);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Permissions seeded successfully", count = permissions.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while seeding permissions", error = ex.Message });
        }
    }

    public class UpdateUserPermissionsRequest
    {
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}
