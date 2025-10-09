using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Models;
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

        public static async System.Threading.Tasks.Task SeedAttendanceDataAsync(ApplicationDbContext context)
        {
            if (await context.AttendanceRecords.AnyAsync())
            {
                return;
            }

            // Check if Users table has data
            var userCount = await context.Users.CountAsync();
            Console.WriteLine($"Total users in database: {userCount}");
            
            var users = await context.Users.OrderBy(u => u.Id).Take(2).ToListAsync(); // Reduce to 2 users
            if (!users.Any())
            {
                Console.WriteLine("No users found for attendance seeding");
                return;
            }
            
            Console.WriteLine($"Found {users.Count} users for attendance seeding");

            Console.WriteLine($"Seeding attendance data for {users.Count} users");

            var today = DateTime.UtcNow.Date; // Use UTC date

            // Create only 3 days of data instead of 7
            for (int i = 0; i < 3; i++)
            {
                var date = today.AddDays(-i);
                foreach (var user in users)
                {
                    try
                    {
                        var record = new AttendanceRecord
                        {
                            UserId = user.Id,
                            Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
                            CheckInTime = DateTime.SpecifyKind(date.AddHours(8), DateTimeKind.Utc),
                            CheckOutTime = DateTime.SpecifyKind(date.AddHours(17), DateTimeKind.Utc),
                            OvertimeHours = i % 2 == 0 ? 2.0m : 0.0m,
                            OvertimeStartTime = i % 2 == 0 ? DateTime.SpecifyKind(date.AddHours(17), DateTimeKind.Utc) : (DateTime?)null,
                            OvertimeEndTime = i % 2 == 0 ? DateTime.SpecifyKind(date.AddHours(19), DateTimeKind.Utc) : (DateTime?)null,
                            IsOvertimeRequired = i % 2 == 0,
                            Status = "present",
                            Notes = $"Attendance for {user.Username}",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        
                        context.AttendanceRecords.Add(record);
                        await context.SaveChangesAsync(); // Save one by one
                        Console.WriteLine($"Created attendance record for user {user.Username} on {date:yyyy-MM-dd}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating attendance record for user {user.Username}: {ex.Message}");
                        // Continue with next record
                    }
                }
            }
        }

        public static async System.Threading.Tasks.Task SeedProductsDataAsync(ApplicationDbContext context)
        {
            if (await context.Products.AnyAsync())
            {
                Console.WriteLine("Products already exist, skipping product seeding");
                return;
            }

            Console.WriteLine("Seeding products data...");

            // Get default company
            var company = await context.Companies.FirstOrDefaultAsync();
            if (company == null)
            {
                Console.WriteLine("No company found, cannot seed products");
                return;
            }

            // Get or create a default category
            var category = await context.ProductCategories.FirstOrDefaultAsync();
            if (category == null)
            {
                // Create a default category
                category = new ProductCategory
                {
                    CategoryName = "Phân bón",
                    Description = "Danh mục phân bón",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.ProductCategories.Add(category);
                await context.SaveChangesAsync();
                Console.WriteLine("Created default category");
            }

            var products = new List<Product>
            {
                new Product
                {
                    CompanyId = company.Id,
                    CategoryId = category.Id,
                    Status = "Active",
                    ProductName = "NPK HaiDuong 20-10-24+50B (Chuyên Cà Phê)",
                    Description = "Phân NPK chuyên dùng cho cà phê",
                    Unit = "kg",
                    MinStockLevel = 1000,
                    MaxStockLevel = 10000,
                    UnitPrice = 0.00m,
                    Price = 0.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCode = "NPK001",
                    SupplierId = 2,
                    BatchNumber = "LO001",
                    ExpiryDate = DateTime.SpecifyKind(DateTime.Parse("2025-10-08"), DateTimeKind.Utc),
                    ProductionDate = DateTime.SpecifyKind(DateTime.Parse("2025-09-23"), DateTimeKind.Utc)
                },
                new Product
                {
                    CompanyId = company.Id,
                    CategoryId = category.Id,
                    Status = "Active",
                    ProductName = "NPK HaiDuong 15-15-15",
                    Description = "Phân NPK cân bằng",
                    Unit = "kg",
                    MinStockLevel = 1000,
                    MaxStockLevel = 10000,
                    UnitPrice = 0.00m,
                    Price = 0.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCode = "NPK002",
                    SupplierId = 3,
                    BatchNumber = "PNK3",
                    ExpiryDate = DateTime.SpecifyKind(DateTime.Parse("2026-02-28"), DateTimeKind.Utc),
                    ProductionDate = DateTime.SpecifyKind(DateTime.Parse("2025-09-01"), DateTimeKind.Utc)
                },
                new Product
                {
                    CompanyId = company.Id,
                    CategoryId = category.Id,
                    Status = "Active",
                    ProductName = "Phân hữu cơ khoáng HD 304",
                    Description = "Phân hữu cơ khoáng cao cấp",
                    Unit = "kg",
                    MinStockLevel = 1000,
                    MaxStockLevel = 10000,
                    UnitPrice = 0.00m,
                    Price = 0.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCode = "NPK003",
                    SupplierId = 2,
                    BatchNumber = "LO002",
                    ExpiryDate = DateTime.SpecifyKind(DateTime.Parse("2025-10-09"), DateTimeKind.Utc),
                    ProductionDate = DateTime.SpecifyKind(DateTime.Parse("2025-09-27"), DateTimeKind.Utc)
                },
                new Product
                {
                    CompanyId = company.Id,
                    CategoryId = category.Id,
                    Status = "Active",
                    ProductName = "Hữu cơ HD BIOMIX",
                    Description = "Phân hữu cơ sinh học",
                    Unit = "kg",
                    MinStockLevel = 1000,
                    MaxStockLevel = 10000,
                    UnitPrice = 0.00m,
                    Price = 0.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCode = "NPK004"
                },
                new Product
                {
                    CompanyId = company.Id,
                    CategoryId = category.Id,
                    Status = "Active",
                    ProductName = "HD Strong",
                    Description = "Phân bón HD Strong",
                    Unit = "kg",
                    MinStockLevel = 1000,
                    MaxStockLevel = 10000,
                    UnitPrice = 0.00m,
                    Price = 0.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCode = "NPK005"
                },
                new Product
                {
                    CompanyId = company.Id,
                    CategoryId = category.Id,
                    Status = "Active",
                    ProductName = "HD Active",
                    Description = "Phân bón HD Active",
                    Unit = "kg",
                    MinStockLevel = 1000,
                    MaxStockLevel = 10000,
                    UnitPrice = 0.00m,
                    Price = 0.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCode = "NPK006"
                },
                new Product
                {
                    CompanyId = company.Id,
                    CategoryId = category.Id,
                    Status = "Active",
                    ProductName = "HD 302",
                    Description = "Phân bón HD 302",
                    Unit = "kg",
                    MinStockLevel = 1000,
                    MaxStockLevel = 10000,
                    UnitPrice = 0.00m,
                    Price = 0.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCode = "NPK007"
                },
                new Product
                {
                    CompanyId = company.Id,
                    CategoryId = category.Id,
                    Status = "Active",
                    ProductName = "HD GREEN",
                    Description = "Phân bón HD GREEN",
                    Unit = "kg",
                    MinStockLevel = 1000,
                    MaxStockLevel = 10000,
                    UnitPrice = 0.00m,
                    Price = 0.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCode = "NPK008"
                },
                new Product
                {
                    CompanyId = company.Id,
                    CategoryId = category.Id,
                    Status = "Active",
                    ProductName = "HD GOLD",
                    Description = "Phân bón HD GOLD",
                    Unit = "kg",
                    MinStockLevel = 1000,
                    MaxStockLevel = 10000,
                    UnitPrice = 0.00m,
                    Price = 0.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCode = "NPK009"
                },
                new Product
                {
                    CompanyId = company.Id,
                    CategoryId = category.Id,
                    Status = "Active",
                    ProductName = "Nitrate Calcium - Boronica HaiDuong",
                    Description = "Canxi Nitrate Boronica",
                    Unit = "kg",
                    MinStockLevel = 1000,
                    MaxStockLevel = 10000,
                    UnitPrice = 0.00m,
                    Price = 0.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCode = "NPK010"
                }
            };

            // Add more products (continuing the pattern)
            for (int i = 11; i <= 60; i++)
            {
                products.Add(new Product
                {
                    CompanyId = company.Id,
                    CategoryId = category.Id,
                    Status = "Active",
                    ProductName = $"Product {i}",
                    Description = $"Description for product {i}",
                    Unit = "kg",
                    MinStockLevel = 1000,
                    MaxStockLevel = 10000,
                    UnitPrice = 0.00m,
                    Price = 0.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProductCode = $"NPK{i:D3}"
                });
            }

            try
            {
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
                Console.WriteLine($"Successfully seeded {products.Count} products");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding products: {ex.Message}");
            }
        }
    }
}
