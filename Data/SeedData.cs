using FertilizerWarehouseAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FertilizerWarehouseAPI.Data
{
    public static class SeedData
    {
        public static async System.Threading.Tasks.Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            // Check if we already have users
            if (await context.Users.AnyAsync())
            {
                return;
            }

            // Get the default company (should exist from ApplicationDbContext seed data)
            var company = await context.Companies.FirstOrDefaultAsync();
            if (company == null)
            {
                // Try to create company with conflict handling
                try
                {
                    company = new Company
                    {
                        Id = 1, // Use same ID as in ApplicationDbContext
                        Code = "FWC",
                        CompanyName = "Fertilizer Warehouse Company",
                        Address = "123 Main Street",
                        Phone = "0123456789",
                        Email = "admin@fwc.com",
                        TaxCode = "123456789",
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    context.Companies.Add(company);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    // If company already exists, get it from database
                    company = await context.Companies.FirstOrDefaultAsync();
                    if (company == null)
                    {
                        throw new InvalidOperationException("Failed to create or find default company");
                    }
                }
            }

            // Create sample users with CompanyId
            var users = new List<User>
            {
                new User
                {
                    CompanyId = company.Id,
                    Username = "admin",
                    FullName = "Quản trị viên",
                    Email = "admin@company.com",
                    Phone = "0123456789",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = Models.Enums.UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    CompanyId = company.Id,
                    Username = "nv001",
                    FullName = "Nguyễn Văn A",
                    Email = "nva@company.com",
                    Phone = "0123456780",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Role = Models.Enums.UserRole.Warehouse,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    CompanyId = company.Id,
                    Username = "nv002",
                    FullName = "Trần Thị B",
                    Email = "ttb@company.com",
                    Phone = "0123456781",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Role = Models.Enums.UserRole.Sales,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        public static async System.Threading.Tasks.Task SeedPermissionsAsync(ApplicationDbContext context)
        {
            // Check if permissions already exist
            if (await context.RolePermissions.AnyAsync())
            {
                return;
            }

            var permissions = new List<RolePermission>
            {
                // Admin permissions - all permissions
                new RolePermission { Role = "admin", PermissionKey = "all", PermissionName = "Tất cả quyền", Module = "General", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Warehouse permissions
                new RolePermission { Role = "admin", PermissionKey = "view_warehouse", PermissionName = "Xem thông tin kho", Module = "Warehouse", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "edit_warehouse", PermissionName = "Chỉnh sửa kho", Module = "Warehouse", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_locations", PermissionName = "Quản lý vị trí", Module = "Warehouse", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "view_cell_details", PermissionName = "Xem chi tiết ô", Module = "Warehouse", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "customize_warehouse", PermissionName = "Tùy chỉnh kho", Module = "Warehouse", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "warehouse_config", PermissionName = "Cấu hình kho", Module = "Warehouse", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "use_warehouse_filters", PermissionName = "Sử dụng bộ lọc kho", Module = "Warehouse", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "drag_drop_position", PermissionName = "Kéo thả vị trí", Module = "Warehouse", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "transfer_goods", PermissionName = "Chuyển hàng giữa các ô", Module = "Warehouse", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "empty_cell", PermissionName = "Làm trống ô", Module = "Warehouse", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_clusters", PermissionName = "Quản lý cụm", Module = "Warehouse", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "create_cluster", PermissionName = "Tạo cụm mới", Module = "Warehouse", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Production permissions
                new RolePermission { Role = "admin", PermissionKey = "view_production", PermissionName = "Xem thông tin sản xuất", Module = "Production", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_production", PermissionName = "Quản lý sản xuất", Module = "Production", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "create_production_order", PermissionName = "Tạo lệnh sản xuất", Module = "Production", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "edit_production_order", PermissionName = "Sửa lệnh sản xuất", Module = "Production", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "delete_production_order", PermissionName = "Xóa lệnh sản xuất", Module = "Production", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_machines", PermissionName = "Quản lý máy móc", Module = "Production", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_production_schedule", PermissionName = "Quản lý lịch sản xuất", Module = "Production", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Employee permissions
                new RolePermission { Role = "admin", PermissionKey = "view_employees", PermissionName = "Xem nhân viên", Module = "Employees", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "view_employee_performance", PermissionName = "Xem hiệu suất", Module = "Employees", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "assign_staff", PermissionName = "Phân công nhân viên", Module = "Employees", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_permissions", PermissionName = "Quản lý quyền", Module = "Admin", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_users", PermissionName = "Quản lý người dùng", Module = "Admin", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "view_staff_info", PermissionName = "Xem thông tin nhân viên", Module = "Employees", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Reports permissions
                new RolePermission { Role = "admin", PermissionKey = "view_reports", PermissionName = "Xem báo cáo", Module = "Reports", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "export_reports", PermissionName = "Xuất báo cáo", Module = "Reports", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "view_activities", PermissionName = "Xem hoạt động", Module = "Reports", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "print_reports", PermissionName = "In báo cáo", Module = "Reports", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "schedule_reports", PermissionName = "Lên lịch báo cáo", Module = "Reports", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Inventory Check permissions
                new RolePermission { Role = "admin", PermissionKey = "inventory_check", PermissionName = "Truy cập chức năng kiểm kê", Module = "Inventory", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "initiate_inventory_check", PermissionName = "Khởi tạo kiểm kê", Module = "Inventory", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "perform_inventory_check", PermissionName = "Thực hiện kiểm kê", Module = "Inventory", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "verify_inventory_check", PermissionName = "Xác minh kiểm kê", Module = "Inventory", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "approve_inventory_adjustments", PermissionName = "Duyệt điều chỉnh kiểm kê", Module = "Inventory", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "view_inventory_check_history", PermissionName = "Xem lịch sử kiểm kê", Module = "Inventory", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "inventory_detail", PermissionName = "Xem chi tiết kiểm kê", Module = "Inventory", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Alerts permissions
                new RolePermission { Role = "admin", PermissionKey = "view_alerts", PermissionName = "Xem cảnh báo", Module = "Alerts", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_alerts", PermissionName = "Quản lý cảnh báo", Module = "Alerts", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "create_alert", PermissionName = "Tạo cảnh báo", Module = "Alerts", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "delete_alert", PermissionName = "Xóa cảnh báo", Module = "Alerts", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Settings permissions
                new RolePermission { Role = "admin", PermissionKey = "manage_settings", PermissionName = "Quản lý cài đặt", Module = "Settings", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Products permissions
                new RolePermission { Role = "admin", PermissionKey = "view_products", PermissionName = "Xem sản phẩm", Module = "Products", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "edit_products", PermissionName = "Chỉnh sửa sản phẩm", Module = "Products", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "delete_products", PermissionName = "Xóa sản phẩm", Module = "Products", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "import_products", PermissionName = "Nhập sản phẩm", Module = "Products", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "export_products", PermissionName = "Xuất sản phẩm", Module = "Products", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "track_products", PermissionName = "Theo dõi sản phẩm", Module = "Products", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_products", PermissionName = "Quản lý sản phẩm", Module = "Products", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_inventory", PermissionName = "Quản lý tồn kho", Module = "Products", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Maintenance permissions
                new RolePermission { Role = "admin", PermissionKey = "view_maintenance_schedule", PermissionName = "Xem lịch bảo trì", Module = "Maintenance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "create_maintenance_task", PermissionName = "Tạo nhiệm vụ bảo trì", Module = "Maintenance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "assign_maintenance_task", PermissionName = "Giao nhiệm vụ bảo trì", Module = "Maintenance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "mark_task_complete", PermissionName = "Đánh dấu hoàn thành", Module = "Maintenance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "maintenance", PermissionName = "Truy cập chức năng bảo trì", Module = "Maintenance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Leave Request permissions
                new RolePermission { Role = "admin", PermissionKey = "view_leave_requests", PermissionName = "Xem đơn xin phép", Module = "LeaveRequests", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_leave_requests", PermissionName = "Quản lý đơn xin phép", Module = "LeaveRequests", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "create_leave_requests", PermissionName = "Tạo đơn xin phép", Module = "LeaveRequests", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "edit_leave_requests", PermissionName = "Sửa đơn xin phép", Module = "LeaveRequests", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "approve_leave_requests", PermissionName = "Duyệt đơn xin phép", Module = "LeaveRequests", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "reject_leave_requests", PermissionName = "Từ chối đơn xin phép", Module = "LeaveRequests", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "delete_leave_requests", PermissionName = "Xóa đơn xin phép", Module = "LeaveRequests", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "cancel_leave_requests", PermissionName = "Hủy đơn xin phép", Module = "LeaveRequests", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Notifications permissions
                new RolePermission { Role = "admin", PermissionKey = "view_notifications", PermissionName = "Xem thông báo", Module = "Notifications", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_notifications", PermissionName = "Quản lý thông báo", Module = "Notifications", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "create_notifications", PermissionName = "Tạo thông báo", Module = "Notifications", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "edit_notifications", PermissionName = "Sửa thông báo", Module = "Notifications", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "delete_notifications", PermissionName = "Xóa thông báo", Module = "Notifications", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "send_notifications", PermissionName = "Gửi thông báo", Module = "Notifications", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Attendance permissions
                new RolePermission { Role = "admin", PermissionKey = "view_attendance", PermissionName = "Xem chấm công", Module = "Attendance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_attendance", PermissionName = "Quản lý chấm công", Module = "Attendance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "create_attendance", PermissionName = "Tạo chấm công", Module = "Attendance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "edit_attendance", PermissionName = "Sửa chấm công", Module = "Attendance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "delete_attendance", PermissionName = "Xóa chấm công", Module = "Attendance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "export_attendance", PermissionName = "Xuất chấm công", Module = "Attendance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "approve_attendance", PermissionName = "Duyệt chấm công", Module = "Attendance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "reject_attendance", PermissionName = "Từ chối chấm công", Module = "Attendance", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Import/Export Management permissions
                new RolePermission { Role = "admin", PermissionKey = "view_import_export_orders", PermissionName = "Xem đơn nhập xuất", Module = "ImportExport", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "create_import_orders", PermissionName = "Tạo đơn nhập", Module = "ImportExport", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "create_export_orders", PermissionName = "Tạo đơn xuất", Module = "ImportExport", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "edit_import_export_orders", PermissionName = "Sửa đơn nhập xuất", Module = "ImportExport", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "delete_import_export_orders", PermissionName = "Xóa đơn nhập xuất", Module = "ImportExport", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "print_import_export_orders", PermissionName = "In đơn nhập xuất", Module = "ImportExport", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Supplier Management permissions
                new RolePermission { Role = "admin", PermissionKey = "view_suppliers", PermissionName = "Xem nhà cung cấp", Module = "Suppliers", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_suppliers", PermissionName = "Quản lý nhà cung cấp", Module = "Suppliers", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "edit_suppliers", PermissionName = "Sửa nhà cung cấp", Module = "Suppliers", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "delete_suppliers", PermissionName = "Xóa nhà cung cấp", Module = "Suppliers", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Category Management permissions
                new RolePermission { Role = "admin", PermissionKey = "view_categories", PermissionName = "Xem danh mục", Module = "Categories", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "manage_categories", PermissionName = "Quản lý danh mục", Module = "Categories", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "edit_categories", PermissionName = "Sửa danh mục", Module = "Categories", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "delete_categories", PermissionName = "Xóa danh mục", Module = "Categories", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // User Management permissions
                new RolePermission { Role = "admin", PermissionKey = "view_users", PermissionName = "Xem người dùng", Module = "Admin", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "edit_users", PermissionName = "Sửa người dùng", Module = "Admin", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "delete_users", PermissionName = "Xóa người dùng", Module = "Admin", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                
                // Dashboard permissions
                new RolePermission { Role = "admin", PermissionKey = "view_dashboard", PermissionName = "Xem tổng quan", Module = "Dashboard", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "customize_dashboard", PermissionName = "Tùy chỉnh dashboard", Module = "Dashboard", IsEnabled = true, CreatedAt = DateTime.UtcNow },
                new RolePermission { Role = "admin", PermissionKey = "export_dashboard", PermissionName = "Xuất dashboard", Module = "Dashboard", IsEnabled = true, CreatedAt = DateTime.UtcNow }
            };

            context.RolePermissions.AddRange(permissions);
            await context.SaveChangesAsync();
        }
    }
}
