using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using System.Text.Json;

namespace FertilizerWarehouseAPI.Controllers
{
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
        /// Get all permissions
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<object>> GetPermissions()
        {
            try
            {
                var permissions = new[]
                {
                    new { id = "view_dashboard", name = "Xem dashboard", description = "Xem trang tổng quan hệ thống", category = "dashboard" },
                    new { id = "customize_dashboard", name = "Tùy chỉnh dashboard", description = "Tùy chỉnh giao diện dashboard", category = "dashboard" },
                    new { id = "export_dashboard", name = "Xuất báo cáo dashboard", description = "Xuất dữ liệu từ dashboard", category = "dashboard" },
                    
                    new { id = "view_warehouse", name = "Xem kho", description = "Xem thông tin kho hàng", category = "warehouse" },
                    new { id = "edit_warehouse", name = "Chỉnh sửa kho", description = "Chỉnh sửa thông tin kho", category = "warehouse" },
                    new { id = "delete_warehouse", name = "Xóa kho", description = "Xóa kho hàng", category = "warehouse" },
                    new { id = "warehouse_config", name = "Cấu hình kho", description = "Cấu hình thiết lập kho", category = "warehouse" },
                    new { id = "view_cell_details", name = "Xem chi tiết ô kho", description = "Xem thông tin chi tiết các ô kho", category = "warehouse" },
                    new { id = "use_warehouse_filters", name = "Sử dụng bộ lọc kho", description = "Sử dụng các bộ lọc tìm kiếm kho", category = "warehouse" },
                    new { id = "drag_drop_position", name = "Kéo thả vị trí", description = "Thay đổi vị trí sản phẩm bằng kéo thả", category = "warehouse" },
                    new { id = "transfer_goods", name = "Chuyển hàng", description = "Chuyển hàng giữa các vị trí", category = "warehouse" },
                    new { id = "manage_clusters", name = "Quản lý nhóm", description = "Quản lý các nhóm sản phẩm", category = "warehouse" },
                    new { id = "create_cluster", name = "Tạo nhóm mới", description = "Tạo nhóm sản phẩm mới", category = "warehouse" },
                    new { id = "empty_cell", name = "Làm trống ô", description = "Làm trống các ô kho", category = "warehouse" },
                    
                    new { id = "view_products", name = "Xem sản phẩm", description = "Xem danh sách sản phẩm", category = "products" },
                    new { id = "manage_products", name = "Quản lý sản phẩm", description = "Thêm, sửa, xóa sản phẩm", category = "products" },
                    new { id = "edit_products", name = "Chỉnh sửa sản phẩm", description = "Chỉnh sửa thông tin sản phẩm", category = "products" },
                    new { id = "delete_products", name = "Xóa sản phẩm", description = "Xóa sản phẩm khỏi hệ thống", category = "products" },
                    new { id = "import_products", name = "Nhập sản phẩm", description = "Nhập sản phẩm từ file", category = "products" },
                    new { id = "export_products", name = "Xuất sản phẩm", description = "Xuất danh sách sản phẩm", category = "products" },
                    new { id = "view_categories", name = "Xem danh mục", description = "Xem danh sách danh mục", category = "products" },
                    new { id = "manage_categories", name = "Quản lý danh mục", description = "Thêm, sửa, xóa danh mục", category = "products" },
                    new { id = "edit_categories", name = "Chỉnh sửa danh mục", description = "Chỉnh sửa thông tin danh mục", category = "products" },
                    new { id = "delete_categories", name = "Xóa danh mục", description = "Xóa danh mục khỏi hệ thống", category = "products" },
                    
                    new { id = "view_employees", name = "Xem nhân viên", description = "Xem danh sách nhân viên", category = "users" },
                    new { id = "manage_users", name = "Quản lý người dùng", description = "Thêm, sửa, xóa người dùng", category = "users" },
                    new { id = "edit_users", name = "Chỉnh sửa người dùng", description = "Chỉnh sửa thông tin người dùng", category = "users" },
                    new { id = "delete_users", name = "Xóa người dùng", description = "Xóa người dùng khỏi hệ thống", category = "users" },
                    new { id = "manage_permissions", name = "Quản lý phân quyền", description = "Phân quyền cho người dùng", category = "users" },
                    
                    new { id = "view_reports", name = "Xem báo cáo", description = "Xem các báo cáo hệ thống", category = "reports" },
                    new { id = "export_reports", name = "Xuất báo cáo", description = "Xuất báo cáo ra file", category = "reports" },
                    new { id = "print_reports", name = "In báo cáo", description = "In báo cáo ra giấy", category = "reports" },
                    new { id = "schedule_reports", name = "Lên lịch báo cáo", description = "Tự động tạo báo cáo theo lịch", category = "reports" },
                    
                    new { id = "view_import_export_orders", name = "Xem đơn hàng", description = "Xem danh sách đơn nhập/xuất", category = "import_export" },
                    new { id = "create_import_orders", name = "Tạo đơn nhập", description = "Tạo đơn hàng nhập kho", category = "import_export" },
                    new { id = "create_export_orders", name = "Tạo đơn xuất", description = "Tạo đơn hàng xuất kho", category = "import_export" },
                    new { id = "edit_import_export_orders", name = "Chỉnh sửa đơn hàng", description = "Chỉnh sửa thông tin đơn hàng", category = "import_export" },
                    new { id = "delete_import_export_orders", name = "Xóa đơn hàng", description = "Xóa đơn hàng khỏi hệ thống", category = "import_export" },
                    new { id = "print_import_export_orders", name = "In đơn hàng", description = "In đơn hàng ra giấy", category = "import_export" },
                    
                    new { id = "inventory_check", name = "Kiểm kê kho", description = "Thực hiện kiểm kê kho hàng", category = "inventory" },
                    new { id = "initiate_inventory_check", name = "Bắt đầu kiểm kê", description = "Khởi tạo quá trình kiểm kê", category = "inventory" },
                    new { id = "perform_inventory_check", name = "Thực hiện kiểm kê", description = "Thực hiện kiểm kê chi tiết", category = "inventory" },
                    new { id = "approve_inventory_adjustments", name = "Duyệt điều chỉnh", description = "Duyệt các điều chỉnh kiểm kê", category = "inventory" },
                    new { id = "reject_inventory_adjustments", name = "Từ chối điều chỉnh", description = "Từ chối các điều chỉnh kiểm kê", category = "inventory" },
                    
                    new { id = "view_attendance", name = "Xem chấm công", description = "Xem lịch sử chấm công", category = "attendance" },
                    new { id = "create_attendance", name = "Chấm công", description = "Thực hiện chấm công vào/ra", category = "attendance" },
                    new { id = "edit_attendance", name = "Chỉnh sửa chấm công", description = "Chỉnh sửa thông tin chấm công", category = "attendance" },
                    new { id = "approve_attendance", name = "Duyệt chấm công", description = "Duyệt chấm công của nhân viên", category = "attendance" },
                    new { id = "reject_attendance", name = "Từ chối chấm công", description = "Từ chối chấm công của nhân viên", category = "attendance" },
                    
                    new { id = "view_leave_requests", name = "Xem đơn nghỉ phép", description = "Xem danh sách đơn nghỉ phép", category = "leave_requests" },
                    new { id = "create_leave_requests", name = "Tạo đơn nghỉ phép", description = "Tạo đơn xin nghỉ phép", category = "leave_requests" },
                    new { id = "approve_leave_requests", name = "Duyệt đơn nghỉ phép", description = "Duyệt đơn xin nghỉ phép", category = "leave_requests" },
                    new { id = "reject_leave_requests", name = "Từ chối đơn nghỉ phép", description = "Từ chối đơn xin nghỉ phép", category = "leave_requests" },
                    new { id = "delete_leave_requests", name = "Xóa đơn nghỉ phép", description = "Xóa đơn xin nghỉ phép", category = "leave_requests" },
                    
                    new { id = "view_notifications", name = "Xem thông báo", description = "Xem danh sách thông báo", category = "notifications" },
                    new { id = "create_notifications", name = "Tạo thông báo", description = "Tạo thông báo mới", category = "notifications" },
                    new { id = "edit_notifications", name = "Chỉnh sửa thông báo", description = "Chỉnh sửa thông báo", category = "notifications" },
                    new { id = "delete_notifications", name = "Xóa thông báo", description = "Xóa thông báo khỏi hệ thống", category = "notifications" },
                    new { id = "send_notifications", name = "Gửi thông báo", description = "Gửi thông báo cho người dùng", category = "notifications" },
                    
                    new { id = "manage_settings", name = "Quản lý cài đặt", description = "Cấu hình các thiết lập hệ thống", category = "settings" },
                    new { id = "manage_suppliers", name = "Quản lý nhà cung cấp", description = "Quản lý thông tin nhà cung cấp", category = "settings" },
                    new { id = "edit_suppliers", name = "Chỉnh sửa nhà cung cấp", description = "Chỉnh sửa thông tin nhà cung cấp", category = "settings" },
                    new { id = "delete_suppliers", name = "Xóa nhà cung cấp", description = "Xóa nhà cung cấp khỏi hệ thống", category = "settings" },
                    new { id = "view_suppliers", name = "Xem nhà cung cấp", description = "Xem danh sách nhà cung cấp", category = "settings" }
                };

                return Ok(new { success = true, data = permissions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving permissions", error = ex.Message });
            }
        }

        /// <summary>
        /// Get permissions for a specific role
        /// </summary>
        [HttpGet("role-permissions/{role}")]
        public async Task<ActionResult<object>> GetRolePermissions(string role)
        {
            try
            {
                // Define default permissions for each role
                var rolePermissions = role.ToLower() switch
                {
                    "admin" => new[]
                    {
                        "view_dashboard", "customize_dashboard", "export_dashboard",
                        "view_warehouse", "edit_warehouse", "delete_warehouse", "warehouse_config", "view_cell_details", "use_warehouse_filters",
                        "drag_drop_position", "transfer_goods", "manage_clusters", "create_cluster", "empty_cell",
                        "view_products", "manage_products", "edit_products", "delete_products", "import_products", "export_products",
                        "view_categories", "manage_categories", "edit_categories", "delete_categories",
                        "view_employees", "manage_users", "edit_users", "delete_users", "manage_permissions",
                        "view_reports", "export_reports", "print_reports", "schedule_reports",
                        "view_import_export_orders", "create_import_orders", "create_export_orders", "edit_import_export_orders", "delete_import_export_orders", "print_import_export_orders",
                        "inventory_check", "initiate_inventory_check", "perform_inventory_check", "approve_inventory_adjustments", "reject_inventory_adjustments",
                        "view_attendance", "create_attendance", "edit_attendance", "approve_attendance", "reject_attendance",
                        "view_leave_requests", "create_leave_requests", "approve_leave_requests", "reject_leave_requests", "delete_leave_requests",
                        "view_notifications", "create_notifications", "edit_notifications", "delete_notifications", "send_notifications",
                        "manage_settings", "manage_suppliers", "edit_suppliers", "delete_suppliers", "view_suppliers"
                    },
                    "warehouse" => new[]
                    {
                        "view_dashboard", "view_warehouse", "edit_warehouse", "view_cell_details", "use_warehouse_filters",
                        "drag_drop_position", "transfer_goods", "manage_clusters", "create_cluster", "empty_cell",
                        "view_products", "manage_products", "edit_products", "import_products", "export_products",
                        "view_categories", "view_reports", "export_reports", "print_reports",
                        "view_import_export_orders", "create_import_orders", "create_export_orders", "edit_import_export_orders", "print_import_export_orders",
                        "inventory_check", "initiate_inventory_check", "perform_inventory_check",
                        "create_attendance", "view_leave_requests", "create_leave_requests", "view_notifications", "view_suppliers"
                    },
                    "team_leader" => new[]
                    {
                        "view_dashboard", "view_warehouse", "view_products", "view_categories", "view_reports",
                        "view_import_export_orders", "view_attendance", "create_attendance", "edit_attendance", "approve_attendance", "reject_attendance",
                        "view_leave_requests", "create_leave_requests", "approve_leave_requests", "reject_leave_requests", "view_notifications", "view_employees"
                    },
                    "sales" => new[]
                    {
                        "view_dashboard", "view_warehouse", "view_products", "view_categories", "view_reports", "export_reports", "print_reports",
                        "view_import_export_orders", "create_attendance", "view_leave_requests", "create_leave_requests", "view_notifications"
                    },
                    _ => new string[0]
                };

                return Ok(new { success = true, data = new { role = role, permissions = rolePermissions } });
        }
        catch (Exception ex)
        {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving role permissions", error = ex.Message });
            }
        }

        /// <summary>
        /// Save permissions for a role
        /// </summary>
        [HttpPost("save-permissions")]
        public async Task<ActionResult<object>> SavePermissions([FromBody] SavePermissionsRequest request)
        {
            try
            {
                // In a real application, you would save these permissions to a database
                // For now, we'll just log the request and return success
                Console.WriteLine($"Saving permissions for role {request.Role}: {string.Join(", ", request.Permissions)}");
                
                // Here you would typically:
                // 1. Validate the role exists
                // 2. Validate the permissions exist
                // 3. Save to database
                // 4. Update user permissions cache if needed
                
                return Ok(new { success = true, message = $"Permissions saved successfully for role {request.Role}" });
        }
        catch (Exception ex)
        {
                return StatusCode(500, new { success = false, message = "An error occurred while saving permissions", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user permissions
        /// </summary>
        [HttpGet("user-permissions")]
        public async Task<ActionResult<object>> GetUserPermissions()
        {
            try
            {
                // In a real application, you would get the current user's permissions from the database
                // For now, we'll return admin permissions as default
                var userPermissions = new[]
                {
                    "view_dashboard", "customize_dashboard", "export_dashboard",
                    "view_warehouse", "edit_warehouse", "delete_warehouse", "warehouse_config", "view_cell_details", "use_warehouse_filters",
                    "drag_drop_position", "transfer_goods", "manage_clusters", "create_cluster", "empty_cell",
                    "view_products", "manage_products", "edit_products", "delete_products", "import_products", "export_products",
                    "view_categories", "manage_categories", "edit_categories", "delete_categories",
                    "view_employees", "manage_users", "edit_users", "delete_users", "manage_permissions",
                    "view_reports", "export_reports", "print_reports", "schedule_reports",
                    "view_import_export_orders", "create_import_orders", "create_export_orders", "edit_import_export_orders", "delete_import_export_orders", "print_import_export_orders",
                    "inventory_check", "initiate_inventory_check", "perform_inventory_check", "approve_inventory_adjustments", "reject_inventory_adjustments",
                    "view_attendance", "create_attendance", "edit_attendance", "approve_attendance", "reject_attendance",
                    "view_leave_requests", "create_leave_requests", "approve_leave_requests", "reject_leave_requests", "delete_leave_requests",
                    "view_notifications", "create_notifications", "edit_notifications", "delete_notifications", "send_notifications",
                    "manage_settings", "manage_suppliers", "edit_suppliers", "delete_suppliers", "view_suppliers"
                };

                return Ok(new { success = true, data = new { role = "admin", userId = 1, permissions = userPermissions } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving user permissions", error = ex.Message });
            }
        }

        /// <summary>
        /// Toggle a specific permission for a role
        /// </summary>
        [HttpPost("toggle-permission")]
        public async Task<ActionResult<object>> TogglePermission([FromBody] TogglePermissionRequest request)
    {
        try
        {
                Console.WriteLine($"Toggling permission {request.PermissionKey} for role {request.Role} to {request.IsEnabled}");
                
                // In a real application, you would:
                // 1. Validate the role and permission exist
                // 2. Update the database
                // 3. Update user permissions cache if needed
                
                return Ok(new { success = true, message = $"Permission {request.PermissionKey} toggled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while toggling permission", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        [HttpGet("all-roles")]
        public async Task<ActionResult<object>> GetAllRoles()
    {
        try
        {
                var roles = new[]
                {
                    new { id = "admin", name = "Quản trị viên", description = "Có tất cả quyền trong hệ thống", color = "red" },
                    new { id = "warehouse", name = "Nhân viên kho", description = "Quản lý kho hàng và sản phẩm", color = "green" },
                    new { id = "team_leader", name = "Tổ trưởng", description = "Quản lý nhóm và duyệt các yêu cầu", color = "blue" },
                    new { id = "sales", name = "Nhân viên kinh doanh", description = "Xem sản phẩm và tạo báo cáo", color = "purple" }
                };

                return Ok(new { success = true, data = roles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving roles", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if user has specific permission
        /// </summary>
        [HttpGet("check-permission/{permission}")]
        public async Task<ActionResult<object>> CheckPermission(string permission)
        {
            try
            {
                // In a real application, you would check the current user's permissions
                // For now, we'll return true for all permissions (admin access)
                return Ok(new { success = true, data = new { hasPermission = true } });
        }
        catch (Exception ex)
        {
                return StatusCode(500, new { success = false, message = "An error occurred while checking permission", error = ex.Message });
            }
        }
    }

    // DTOs for permission requests
public class SavePermissionsRequest
{
    public string Role { get; set; } = string.Empty;
        public string[] Permissions { get; set; } = Array.Empty<string>();
}

public class TogglePermissionRequest
{
    public string Role { get; set; } = string.Empty;
    public string PermissionKey { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    }
}