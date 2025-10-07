using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FertilizerWarehouseAPI.Services;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionsController> _logger;
    private readonly ApplicationDbContext _context;

    public PermissionsController(IPermissionService permissionService, ILogger<PermissionsController> logger, ApplicationDbContext context)
    {
        _permissionService = permissionService;
        _logger = logger;
        _context = context;
    }

    [HttpGet("user-permissions")]
    public async Task<IActionResult> GetUserPermissions()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // If no user context, return admin permissions for development
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        userId = 1,
                        role = "admin",
                        permissions = new[] { "all" }
                    }
                });
            }
            
            // Admin always has all permissions
            if (userRole?.ToLower() == "admin")
            {
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        userId = int.Parse(userId),
                        role = userRole,
                        permissions = new[] { "all" }
                    }
                });
            }

            var permissions = await _permissionService.GetUserPermissionsAsync(userId, userRole);
            
            return Ok(new
            {
                success = true,
                data = new
                {
                    userId = int.Parse(userId),
                    role = userRole,
                    permissions = permissions
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user permissions");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [AllowAnonymous]
    [HttpGet("check-permission/{permission}")]
    public async Task<IActionResult> CheckPermission(string permission)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { success = false, message = "Invalid user token" });
            }

            var hasPermission = await _permissionService.HasPermissionAsync(userId, userRole, permission);
            
            return Ok(new
            {
                success = true,
                data = new
                {
                    hasPermission = hasPermission,
                    permission = permission
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission {Permission}", permission);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpGet("role-permissions/{role}")]
    public async Task<IActionResult> GetRolePermissions(string role)
    {
        try
        {
            var permissions = await _permissionService.GetRolePermissionsAsync(role);
            
            return Ok(new
            {
                success = true,
                data = new
                {
                    role = role,
                    permissions = permissions
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role permissions for {Role}", role);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpGet("all-roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            var roles = await _permissionService.GetAllRolesAsync();
            
            return Ok(new
            {
                success = true,
                data = roles
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

        [HttpGet("users-by-role/{role}")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            try
            {
                var users = await _context.Users
                    .ToListAsync();

                var filteredUsers = users
                    .Where(u => u.Role.ToString().ToLower() == role.ToLower())
                    .Select(u => new
                    {
                        id = u.Id.ToString(),
                        name = u.Username,
                        role = u.Role.ToString(),
                        email = u.Email
                    })
                    .ToList();

                return Ok(new
                {
                    success = true,
                    data = filteredUsers
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users for role {Role}", role);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("toggle-permission")]
        public async Task<IActionResult> TogglePermission([FromBody] TogglePermissionRequest request)
    {
        try
        {
                // First, remove all duplicate permissions for this role and permission key
                var duplicatePermissions = await _context.RolePermissions
                    .Where(rp => rp.Role == request.Role && rp.PermissionKey == request.PermissionKey)
                    .ToListAsync();

                if (duplicatePermissions.Count > 1)
                {
                    _logger.LogWarning("Found {Count} duplicate permissions for {Role}:{PermissionKey}, removing duplicates", 
                        duplicatePermissions.Count, request.Role, request.PermissionKey);
                    
                    // Keep the first one, remove the rest
                    for (int i = 1; i < duplicatePermissions.Count; i++)
                    {
                        _context.RolePermissions.Remove(duplicatePermissions[i]);
                    }
            await _context.SaveChangesAsync();
                }

                var existingPermission = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.Role == request.Role && rp.PermissionKey == request.PermissionKey);

                if (existingPermission != null)
                {
                    // Update existing permission
                    existingPermission.IsEnabled = request.IsEnabled;
                    existingPermission.PermissionName = GetPermissionDisplayName(request.PermissionKey);
                    existingPermission.Module = GetPermissionModule(request.PermissionKey);
                    _context.RolePermissions.Update(existingPermission);
                }
                else
                {
                    // Create new permission
                    _context.RolePermissions.Add(new RolePermission
                    {
                        Role = request.Role,
                        PermissionKey = request.PermissionKey,
                        PermissionName = GetPermissionDisplayName(request.PermissionKey),
                        Module = GetPermissionModule(request.PermissionKey),
                        IsEnabled = request.IsEnabled,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Permission {Permission} for role {Role} updated to {Status} in database", 
                    request.PermissionKey, request.Role, request.IsEnabled ? "enabled" : "disabled");

                _logger.LogInformation("Toggled permission {Permission} for role {Role} to {Status}", 
                    request.PermissionKey, request.Role, request.IsEnabled ? "enabled" : "disabled");

                return Ok(new
                {
                    success = true,
                    message = $"Permission {request.PermissionKey} {(request.IsEnabled ? "enabled" : "disabled")} successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling permission {Permission} for role {Role}", request.PermissionKey, request.Role);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("save-permissions")]
        public async Task<IActionResult> SavePermissions([FromBody] SavePermissionsRequest request)
    {
        try
        {
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                // Get existing permissions for this role
                var existingPermissions = await _context.RolePermissions
                    .Where(rp => rp.Role == request.Role)
                    .ToListAsync();
                
                // Create a set of requested permissions for quick lookup
                var requestedPermissions = request.Permissions.ToHashSet();
                
                // Update existing permissions or add new ones
                foreach (var permission in requestedPermissions)
                {
                    var existing = existingPermissions.FirstOrDefault(ep => ep.PermissionKey == permission);
                    if (existing != null)
                    {
                        // Update existing permission
                        existing.IsEnabled = true;
                        existing.PermissionName = GetPermissionDisplayName(permission);
                        existing.Module = GetPermissionModule(permission);
                    }
                    else
                    {
                        // Add new permission
                        _context.RolePermissions.Add(new RolePermission
                        {
                            Role = request.Role,
                            PermissionKey = permission,
                            PermissionName = GetPermissionDisplayName(permission),
                            Module = GetPermissionModule(permission),
                            IsEnabled = true,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
                
                // Only disable permissions that are explicitly marked as disabled in the request
                // Don't automatically disable all permissions not in the request
                // This allows for more granular control
                
            await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                // Debug: Log enabled permissions for this role
                var enabledPermissions = await _context.RolePermissions
                    .Where(rp => rp.Role == request.Role && rp.IsEnabled == true)
                    .Select(rp => rp.PermissionKey)
                    .ToListAsync();
                
                _logger.LogInformation("Updated permissions for role {Role}. Enabled permissions: {Count}", 
                    request.Role, enabledPermissions.Count);
                _logger.LogInformation("Enabled permissions: {Permissions}", string.Join(", ", enabledPermissions));
                
                return Ok(new
                {
                    success = true,
                    message = "Permissions saved successfully"
                });
        }
        catch (Exception ex)
        {
                _logger.LogError(ex, "Error saving permissions for role {Role}", request.Role);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

    private string GetPermissionDisplayName(string permissionKey)
    {
        return permissionKey switch
        {
            // Warehouse permissions
            "view_warehouse" => "Xem thông tin kho",
            "edit_warehouse" => "Chỉnh sửa kho",
            "delete_warehouse" => "Xóa kho",
            "manage_locations" => "Quản lý vị trí",
            "view_cell_details" => "Xem chi tiết ô",
            "customize_warehouse" => "Tùy chỉnh kho",
            "warehouse_config" => "Cấu hình kho",
            "use_warehouse_filters" => "Sử dụng bộ lọc kho",
            "drag_drop_position" => "Kéo thả vị trí",
            "transfer_goods" => "Chuyển hàng giữa các ô",
            "empty_cell" => "Làm trống ô",
            "manage_clusters" => "Quản lý cụm",
            "create_cluster" => "Tạo cụm mới",
            
            // Production permissions
            "view_production" => "Xem thông tin sản xuất",
            "manage_production" => "Quản lý sản xuất",
            "create_production_order" => "Tạo lệnh sản xuất",
            "edit_production_order" => "Sửa lệnh sản xuất",
            "delete_production_order" => "Xóa lệnh sản xuất",
            "manage_machines" => "Quản lý máy móc",
            "manage_production_schedule" => "Quản lý lịch sản xuất",
            
            // Employee permissions
            "view_employees" => "Xem nhân viên",
            "view_employee_performance" => "Xem hiệu suất",
            "assign_staff" => "Phân công nhân viên",
            "manage_permissions" => "Quản lý quyền",
            "manage_users" => "Quản lý người dùng",
            "view_staff_info" => "Xem thông tin nhân viên",
            
            // Reports permissions
            "view_reports" => "Xem báo cáo",
            "export_reports" => "Xuất báo cáo",
            "view_activities" => "Xem hoạt động",
            "print_reports" => "In báo cáo",
            "schedule_reports" => "Lên lịch báo cáo",
            
            // Inventory Check permissions
            "inventory_check" => "Truy cập chức năng kiểm kê",
            "initiate_inventory_check" => "Khởi tạo kiểm kê",
            "perform_inventory_check" => "Thực hiện kiểm kê",
            "verify_inventory_check" => "Xác minh kiểm kê",
            "approve_inventory_adjustments" => "Duyệt điều chỉnh kiểm kê",
            "view_inventory_check_history" => "Xem lịch sử kiểm kê",
            "inventory_detail" => "Xem chi tiết kiểm kê",
            
            // Alerts permissions
            "view_alerts" => "Xem cảnh báo",
            "manage_alerts" => "Quản lý cảnh báo",
            "create_alert" => "Tạo cảnh báo",
            "delete_alert" => "Xóa cảnh báo",
            
            // Settings permissions
            "manage_settings" => "Quản lý cài đặt",
            "all" => "Tất cả quyền",
            
            // Products permissions
            "view_products" => "Xem sản phẩm",
            "edit_products" => "Chỉnh sửa sản phẩm",
            "delete_products" => "Xóa sản phẩm",
            "import_products" => "Nhập sản phẩm",
            "export_products" => "Xuất sản phẩm",
            "track_products" => "Theo dõi sản phẩm",
            "manage_products" => "Quản lý sản phẩm",
            "manage_inventory" => "Quản lý tồn kho",
            
            // Maintenance permissions
            "view_maintenance_schedule" => "Xem lịch bảo trì",
            "create_maintenance_task" => "Tạo nhiệm vụ bảo trì",
            "assign_maintenance_task" => "Giao nhiệm vụ bảo trì",
            "mark_task_complete" => "Đánh dấu hoàn thành",
            "maintenance" => "Truy cập chức năng bảo trì",
            
            // Leave Request permissions
            "view_leave_requests" => "Xem đơn xin phép",
            "manage_leave_requests" => "Quản lý đơn xin phép",
            "create_leave_requests" => "Tạo đơn xin phép",
            "edit_leave_requests" => "Sửa đơn xin phép",
            "approve_leave_requests" => "Duyệt đơn xin phép",
            "reject_leave_requests" => "Từ chối đơn xin phép",
            "delete_leave_requests" => "Xóa đơn xin phép",
            "cancel_leave_requests" => "Hủy đơn xin phép",
            
            // Notifications permissions
            "view_notifications" => "Xem thông báo",
            "manage_notifications" => "Quản lý thông báo",
            "create_notifications" => "Tạo thông báo",
            "edit_notifications" => "Sửa thông báo",
            "delete_notifications" => "Xóa thông báo",
            "send_notifications" => "Gửi thông báo",
            
            // Attendance permissions
            "view_attendance" => "Xem chấm công",
            "manage_attendance" => "Quản lý chấm công",
            "create_attendance" => "Tạo chấm công",
            "edit_attendance" => "Sửa chấm công",
            "delete_attendance" => "Xóa chấm công",
            "export_attendance" => "Xuất chấm công",
            "approve_attendance" => "Duyệt chấm công",
            "reject_attendance" => "Từ chối chấm công",
            
            // Import/Export Management permissions
            "view_import_export_orders" => "Xem đơn nhập xuất",
            "create_import_orders" => "Tạo đơn nhập",
            "create_export_orders" => "Tạo đơn xuất",
            "edit_import_export_orders" => "Sửa đơn nhập xuất",
            "delete_import_export_orders" => "Xóa đơn nhập xuất",
            "print_import_export_orders" => "In đơn nhập xuất",
            
            // Supplier Management permissions
            "view_suppliers" => "Xem nhà cung cấp",
            "manage_suppliers" => "Quản lý nhà cung cấp",
            "edit_suppliers" => "Sửa nhà cung cấp",
            "delete_suppliers" => "Xóa nhà cung cấp",
            
            // Category Management permissions
            "view_categories" => "Xem danh mục",
            "manage_categories" => "Quản lý danh mục",
            "edit_categories" => "Sửa danh mục",
            "delete_categories" => "Xóa danh mục",
            
            // User Management permissions
            "view_users" => "Xem người dùng",
            "edit_users" => "Sửa người dùng",
            "delete_users" => "Xóa người dùng",
            
            // Dashboard permissions
            "view_dashboard" => "Xem tổng quan",
            "customize_dashboard" => "Tùy chỉnh dashboard",
            "export_dashboard" => "Xuất dashboard",
            
            _ => permissionKey
        };
    }

    private string GetPermissionModule(string permissionKey)
    {
        return permissionKey switch
        {
            var key when key.StartsWith("view_warehouse") || key.StartsWith("edit_warehouse") || 
                         key.StartsWith("delete_warehouse") || key.StartsWith("manage_locations") ||
                         key.StartsWith("view_cell_details") || key.StartsWith("customize_warehouse") ||
                         key.StartsWith("warehouse_config") || key.StartsWith("use_warehouse_filters") ||
                         key.StartsWith("drag_drop_position") || key.StartsWith("transfer_goods") ||
                         key.StartsWith("empty_cell") || key.StartsWith("manage_clusters") ||
                         key.StartsWith("create_cluster") => "Warehouse",
            
            var key when key.StartsWith("view_production") || key.StartsWith("manage_production") ||
                         key.StartsWith("create_production_order") || key.StartsWith("edit_production_order") ||
                         key.StartsWith("delete_production_order") || key.StartsWith("manage_machines") ||
                         key.StartsWith("manage_production_schedule") => "Production",
            
            var key when key.StartsWith("view_employees") || key.StartsWith("view_employee_performance") ||
                         key.StartsWith("assign_staff") || key.StartsWith("view_staff_info") => "Employees",
            
            var key when key.StartsWith("view_reports") || key.StartsWith("export_reports") ||
                         key.StartsWith("view_activities") || key.StartsWith("print_reports") ||
                         key.StartsWith("schedule_reports") => "Reports",
            
            var key when key.StartsWith("inventory_check") || key.StartsWith("initiate_inventory_check") ||
                         key.StartsWith("perform_inventory_check") || key.StartsWith("verify_inventory_check") ||
                         key.StartsWith("approve_inventory_adjustments") || key.StartsWith("view_inventory_check_history") ||
                         key.StartsWith("inventory_detail") => "Inventory",
            
            var key when key.StartsWith("view_alerts") || key.StartsWith("manage_alerts") ||
                         key.StartsWith("create_alert") || key.StartsWith("delete_alert") => "Alerts",
            
            var key when key.StartsWith("manage_settings") => "Settings",
            
            var key when key.StartsWith("view_products") || key.StartsWith("edit_products") ||
                         key.StartsWith("delete_products") || key.StartsWith("import_products") ||
                         key.StartsWith("export_products") || key.StartsWith("track_products") ||
                         key.StartsWith("manage_products") || key.StartsWith("manage_inventory") => "Products",
            
            var key when key.StartsWith("view_maintenance_schedule") || key.StartsWith("create_maintenance_task") ||
                         key.StartsWith("assign_maintenance_task") || key.StartsWith("mark_task_complete") ||
                         key.StartsWith("maintenance") => "Maintenance",
            
            var key when key.StartsWith("view_leave_requests") || key.StartsWith("manage_leave_requests") ||
                         key.StartsWith("create_leave_requests") || key.StartsWith("edit_leave_requests") ||
                         key.StartsWith("approve_leave_requests") || key.StartsWith("reject_leave_requests") ||
                         key.StartsWith("delete_leave_requests") || key.StartsWith("cancel_leave_requests") => "LeaveRequests",
            
            var key when key.StartsWith("view_notifications") || key.StartsWith("manage_notifications") ||
                         key.StartsWith("create_notifications") || key.StartsWith("edit_notifications") ||
                         key.StartsWith("delete_notifications") || key.StartsWith("send_notifications") => "Notifications",
            
            var key when key.StartsWith("view_attendance") || key.StartsWith("manage_attendance") ||
                         key.StartsWith("create_attendance") || key.StartsWith("edit_attendance") ||
                         key.StartsWith("delete_attendance") || key.StartsWith("export_attendance") ||
                         key.StartsWith("approve_attendance") || key.StartsWith("reject_attendance") => "Attendance",
            
            var key when key.StartsWith("view_import_export_orders") || key.StartsWith("create_import_orders") ||
                         key.StartsWith("create_export_orders") || key.StartsWith("edit_import_export_orders") ||
                         key.StartsWith("delete_import_export_orders") || key.StartsWith("print_import_export_orders") => "ImportExport",
            
            var key when key.StartsWith("view_suppliers") || key.StartsWith("manage_suppliers") ||
                         key.StartsWith("edit_suppliers") || key.StartsWith("delete_suppliers") => "Suppliers",
            
            var key when key.StartsWith("view_categories") || key.StartsWith("manage_categories") ||
                         key.StartsWith("edit_categories") || key.StartsWith("delete_categories") => "Categories",
            
            var key when key.StartsWith("view_users") || key.StartsWith("edit_users") ||
                         key.StartsWith("delete_users") || key.StartsWith("manage_users") ||
                         key.StartsWith("manage_permissions") => "Admin",
            
            var key when key.StartsWith("view_dashboard") || key.StartsWith("customize_dashboard") ||
                         key.StartsWith("export_dashboard") => "Dashboard",
            
            _ => "General"
        };
    }
}

public class SavePermissionsRequest
{
    public string Role { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}

public class TogglePermissionRequest
{
    public string Role { get; set; } = string.Empty;
    public string PermissionKey { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}