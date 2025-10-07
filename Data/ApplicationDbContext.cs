using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Models.Enums;
using FertilizerWarehouseAPI.Models;

namespace FertilizerWarehouseAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Company & Organization
    public DbSet<Company> Companies { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Models.Entities.UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }
        public DbSet<SecurityAlert> SecurityAlerts { get; set; }
        public DbSet<TrustedDevice> TrustedDevices { get; set; }
        
        // Attendance & Leave Management
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<LeaveRequestModel> LeaveRequests { get; set; }

    // Warehouse & Positions
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<WarehouseZone> WarehouseZones { get; set; }
    public DbSet<WarehousePosition> WarehousePositions { get; set; }
    public DbSet<WarehouseCell> WarehouseCells { get; set; }
    public DbSet<WarehouseCellProduct> WarehouseCellProducts { get; set; }
    public DbSet<WarehouseCluster> WarehouseClusters { get; set; }
    public DbSet<WarehouseActivity> WarehouseActivities { get; set; }
    public DbSet<UserWarehouse> UserWarehouses { get; set; }

    // Maintenance & Inventory
    public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
    public DbSet<MaintenanceHistory> MaintenanceHistories { get; set; }
    public DbSet<InventoryCheck> InventoryChecks { get; set; }
    public DbSet<InventoryCheckItem> InventoryCheckItems { get; set; }

    // Products
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<ProductBatch> ProductBatches { get; set; }
    public DbSet<ProductComposition> ProductCompositions { get; set; }

    // Import/Export Orders
    public DbSet<ImportOrder> ImportOrders { get; set; }
    public DbSet<ImportOrderDetail> ImportOrderDetails { get; set; }

    // Partners
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }

    // Transactions
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    public DbSet<SalesOrder> SalesOrders { get; set; }
    public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }

    // Stock Management
    public DbSet<StockItem> StockItems { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<StockTransfer> StockTransfers { get; set; }
    public DbSet<StockTransferDetail> StockTransferDetails { get; set; }
    public DbSet<StockAdjustment> StockAdjustments { get; set; }
    public DbSet<StockAdjustmentDetail> StockAdjustmentDetails { get; set; }

    // Stock Take
    public DbSet<StockTake> StockTakes { get; set; }
    public DbSet<StockTakeDetail> StockTakeDetails { get; set; }
    public DbSet<StockTakeAdjustment> StockTakeAdjustments { get; set; }

    // Tasks & Reports
    public DbSet<Models.Entities.Task> Tasks { get; set; }
    public DbSet<TaskComment> TaskComments { get; set; }
    public DbSet<Report> Reports { get; set; }

    // Notifications & Alerts
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Alert> Alerts { get; set; }
    public DbSet<AlertRule> AlertRules { get; set; }

    // Support & System
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<FileAttachment> FileAttachments { get; set; }
    public DbSet<Barcode> Barcodes { get; set; }
    public DbSet<SecurityEvent> SecurityEvents { get; set; }

    // Tags
    public DbSet<Tag> Tags { get; set; }
    public DbSet<EntityTag> EntityTags { get; set; }

    // Production & Manufacturing
    public DbSet<Production> Productions { get; set; }
    public DbSet<ProductionMachine> ProductionMachines { get; set; }

    // Employee Management
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure decimal precision
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        // Role configurations
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.RoleName).IsUnique();
        });

        // Permission configurations
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // User configurations
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Role).HasConversion<int>();
        });

        // UserRole configurations  
        modelBuilder.Entity<Models.Entities.UserRole>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ur => ur.AssignedByUser)
                .WithMany()
                .HasForeignKey(ur => ur.AssignedBy)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // RolePermission configurations
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasIndex(e => new { e.Role, e.PermissionKey }).IsUnique();
        });

        // UserPermission configurations
        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.PermissionKey }).IsUnique();
            entity.HasOne(up => up.User)
                .WithMany()
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserWarehouse configurations
        modelBuilder.Entity<UserWarehouse>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.WarehouseId }).IsUnique();
            entity.HasOne(uw => uw.User)
                .WithMany(u => u.UserWarehouses)
                .HasForeignKey(uw => uw.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(uw => uw.Warehouse)
                .WithMany()
                .HasForeignKey(uw => uw.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(uw => uw.AssignedByUser)
                .WithMany()
                .HasForeignKey(uw => uw.AssignedBy)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // StockItem configurations
        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.HasIndex(e => new { e.WarehouseId, e.ProductId, e.BatchId, e.PositionId }).IsUnique();
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
            entity.Property(e => e.ReservedQuantity).HasColumnType("decimal(18,3)");
        });

        // SecurityEvent configurations
        modelBuilder.Entity<SecurityEvent>(entity =>
        {
            entity.HasOne(se => se.User)
                .WithMany(u => u.SecurityEvents)
                .HasForeignKey(se => se.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Department configurations
        modelBuilder.Entity<Department>()
            .HasOne(d => d.Manager)
            .WithMany()
            .HasForeignKey(d => d.ManagerId)
            .OnDelete(DeleteBehavior.NoAction);

        // ProductCategory self-referencing
        modelBuilder.Entity<ProductCategory>()
            .HasOne(p => p.ParentCategory)
            .WithMany(p => p.SubCategories)
            .HasForeignKey(p => p.ParentCategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure enum conversions
        modelBuilder.Entity<User>()
            .Property(e => e.Role)
            .HasConversion<int>();

        modelBuilder.Entity<ProductBatch>()
            .Property(e => e.QualityStatus)
            .HasConversion<int>();

        modelBuilder.Entity<StockMovement>()
            .Property(e => e.MovementType)
            .HasConversion<int>();

        modelBuilder.Entity<PurchaseOrder>()
            .Property(e => e.Status)
            .HasConversion<int>();

        modelBuilder.Entity<SalesOrder>()
            .Property(e => e.Status)
            .HasConversion<int>();

        modelBuilder.Entity<StockTransfer>()
            .Property(e => e.Status)
            .HasConversion<int>();

        modelBuilder.Entity<Models.Entities.Task>()
            .Property(e => e.Status)
            .HasConversion<int>();

        // Configure ALL relationships to prevent cascade delete cycles
        
        // User relationships
        modelBuilder.Entity<User>()
            .HasOne(u => u.Company)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Department)
            .WithMany(d => d.Users)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        // Department relationships
        modelBuilder.Entity<Department>()
            .HasOne(d => d.Company)
            .WithMany(c => c.Departments)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Department>()
            .HasOne(d => d.Manager)
            .WithMany()
            .HasForeignKey(d => d.ManagerId)
            .OnDelete(DeleteBehavior.NoAction);

        // Warehouse relationships
        modelBuilder.Entity<Warehouse>()
            .HasOne(w => w.Company)
            .WithMany(c => c.Warehouses)
            .HasForeignKey(w => w.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Warehouse>()
            .HasOne(w => w.Manager)
            .WithMany()
            .HasForeignKey(w => w.ManagerId)
            .OnDelete(DeleteBehavior.NoAction);

        // Product relationships
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Company)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.CategoryNavigation)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        // Tag relationships
        modelBuilder.Entity<EntityTag>()
            .HasOne(et => et.Tag)
            .WithMany(t => t.EntityTags)
            .HasForeignKey(et => et.TagId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<EntityTag>()
            .HasOne(et => et.Assigner)
            .WithMany()
            .HasForeignKey(et => et.AssignedBy)
            .OnDelete(DeleteBehavior.NoAction);

        // Order relationships
        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(p => p.Approver)
            .WithMany()
            .HasForeignKey(p => p.ApprovedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(p => p.Creator)
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<StockMovement>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<StockMovement>()
            .HasOne(s => s.FromPosition)
            .WithMany()
            .HasForeignKey(s => s.FromPositionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<StockMovement>()
            .HasOne(s => s.ToPosition)
            .WithMany()
            .HasForeignKey(s => s.ToPositionId)
            .OnDelete(DeleteBehavior.NoAction);

        // Additional relationships
        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(p => p.Company)
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.PurchaseOrders)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SalesOrder>()
            .HasOne(s => s.Company)
            .WithMany()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SalesOrder>()
            .HasOne(s => s.Customer)
            .WithMany(c => c.SalesOrders)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SalesOrder>()
            .HasOne(s => s.SalesPerson)
            .WithMany()
            .HasForeignKey(s => s.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        // All other relationships
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }

        // Seed initial data
        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = 1,
                Code = "FWC",
                CompanyName = "Fertilizer Warehouse Company",
                Address = "123 Main Street",
                Phone = "0123456789",
                Email = "admin@fwc.com",
                TaxCode = "123456789",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );

        // No default admin user - must be created via API
    }
}
