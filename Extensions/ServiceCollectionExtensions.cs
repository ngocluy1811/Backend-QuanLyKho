using FertilizerWarehouseAPI.Services;

namespace FertilizerWarehouseAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register service interfaces and implementations
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            // services.AddScoped<IWarehouseService, WarehouseService>();
            
            // TODO: Add more services as they are created
            // services.AddScoped<IProductService, ProductService>();
            // services.AddScoped<IEmployeeService, EmployeeService>();
            // services.AddScoped<IOrderService, OrderService>();
            // services.AddScoped<IReportService, ReportService>();
            // services.AddScoped<INotificationService, NotificationService>();
            // services.AddScoped<IAlertService, AlertService>();
            // services.AddScoped<ITaskService, TaskService>();
            // services.AddScoped<ICompanyService, CompanyService>();
            // services.AddScoped<ISupplierService, SupplierService>();
            // services.AddScoped<ICustomerService, CustomerService>();
            // services.AddScoped<IVehicleService, VehicleService>();

            return services;
        }
    }
}
