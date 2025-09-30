using AutoMapper;
using FertilizerWarehouseAPI.DTOs;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();
        CreateMap<User, UserProfileDto>();
        CreateMap<UpdateUserProfileDto, User>();

        // Product mappings
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<CreateProductCategoryDto, ProductCategory>();
        CreateMap<UpdateProductCategoryDto, ProductCategory>();
        CreateMap<ProductBatch, ProductBatchDto>();
        CreateMap<CreateProductBatchDto, ProductBatch>();
        CreateMap<UpdateProductBatchDto, ProductBatch>();

        // Warehouse mappings
        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<CreateWarehouseDto, Warehouse>();
        CreateMap<UpdateWarehouseDto, Warehouse>();

        // Company mappings
        CreateMap<Company, CompanyDto>();
        CreateMap<CreateCompanyDto, Company>();
        CreateMap<UpdateCompanyDto, Company>();

        // Department mappings
        CreateMap<Department, DepartmentDto>();
        CreateMap<CreateDepartmentDto, Department>();
        CreateMap<UpdateDepartmentDto, Department>();

        // Supplier mappings
        CreateMap<Supplier, SupplierDto>();
        CreateMap<CreateSupplierDto, Supplier>();
        CreateMap<UpdateSupplierDto, Supplier>();

        // Customer mappings
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerDto, Customer>();
        CreateMap<UpdateCustomerDto, Customer>();

        // Order mappings
        CreateMap<PurchaseOrder, PurchaseOrderDto>();
        CreateMap<CreatePurchaseOrderDto, PurchaseOrder>();
        CreateMap<UpdatePurchaseOrderDto, PurchaseOrder>();
        CreateMap<SalesOrder, SalesOrderDto>();
        CreateMap<CreateSalesOrderDto, SalesOrder>();
        CreateMap<UpdateSalesOrderDto, SalesOrder>();

        // Stock mappings
        CreateMap<StockMovement, StockMovementDto>();
        CreateMap<CreateStockMovementDto, StockMovement>();
        CreateMap<UpdateStockMovementDto, StockMovement>();
        CreateMap<StockTransfer, StockTransferDto>();
        CreateMap<CreateStockTransferDto, StockTransfer>();
        CreateMap<UpdateStockTransferDto, StockTransfer>();
        CreateMap<StockTake, StockTakeDto>();
        CreateMap<CreateStockTakeDto, StockTake>();
        CreateMap<UpdateStockTakeDto, StockTake>();

        // Production mappings
        CreateMap<Production, ProductionDto>();
        CreateMap<CreateProductionDto, Production>();
        CreateMap<UpdateProductionDto, Production>();
        CreateMap<ProductionMachine, ProductionMachineDto>();
        CreateMap<CreateProductionMachineDto, ProductionMachine>();
        CreateMap<UpdateProductionMachineDto, ProductionMachine>();

        // Employee mappings
        CreateMap<Employee, EmployeeDto>();
        CreateMap<CreateEmployeeDto, Employee>();
        CreateMap<UpdateEmployeeDto, Employee>();
        CreateMap<Attendance, AttendanceDto>();
        CreateMap<CreateAttendanceDto, Attendance>();
        CreateMap<UpdateAttendanceDto, Attendance>();
        CreateMap<LeaveRequest, LeaveRequestDto>();
        CreateMap<CreateLeaveRequestDto, LeaveRequest>();
        CreateMap<UpdateLeaveRequestDto, LeaveRequest>();

        // Task mappings
        CreateMap<Models.Entities.Task, TaskDto>();
        CreateMap<CreateTaskDto, Models.Entities.Task>();
        CreateMap<UpdateTaskDto, Models.Entities.Task>();

        // Notification mappings
        CreateMap<Notification, NotificationDto>();
        CreateMap<CreateNotificationDto, Notification>();
        CreateMap<UpdateNotificationDto, Notification>();

        // Security mappings
        CreateMap<AuditLog, AuditLogDto>();
        CreateMap<CreateAuditLogDto, AuditLog>();
        CreateMap<SecurityEvent, SecurityEventDto>();
        CreateMap<CreateSecurityEventDto, SecurityEvent>();
        CreateMap<UserSession, UserSessionDto>();
        CreateMap<Role, RoleDto>();
        CreateMap<CreateRoleDto, Role>();
        CreateMap<UpdateRoleDto, Role>();
        CreateMap<Permission, PermissionDto>();
        CreateMap<CreatePermissionDto, Permission>();
        CreateMap<UpdatePermissionDto, Permission>();
        CreateMap<SystemSetting, SystemSettingDto>();
        CreateMap<CreateSystemSettingDto, SystemSetting>();
        CreateMap<UpdateSystemSettingDto, SystemSetting>();
    }
}
