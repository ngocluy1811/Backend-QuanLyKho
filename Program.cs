using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Serilog;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Services;
using FertilizerWarehouseAPI.Services.Interfaces;
using FertilizerWarehouseAPI.Services.Implementations;
using FertilizerWarehouseAPI.Profiles;
using FertilizerWarehouseAPI.Middleware;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var jwtService = context.HttpContext.RequestServices.GetRequiredService<IJwtService>();
            var token = context.Request.Headers.Authorization
                .FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token) && await jwtService.IsTokenBlacklistedAsync(token))
            {
                context.Fail("Token is blacklisted");
            }
        }
    };
});

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Management", policy => 
        policy.RequireRole("Admin", "TeamLeader"));
    options.AddPolicy("Warehouse", policy => 
        policy.RequireRole("Admin", "Warehouse"));
    options.AddPolicy("Sales", policy => 
        policy.RequireRole("Admin", "Sales"));
    options.AddPolicy("TeamLeader", policy => 
        policy.RequireRole("Admin", "TeamLeader"));
});

// CORS Configuration
var securitySettings = builder.Configuration.GetSection("SecuritySettings");
var allowedOrigins = securitySettings.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Rate Limiting
var rateLimitSettings = builder.Configuration.GetSection("RateLimitSettings");
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = rateLimitSettings.GetValue<int>("PermitLimit"),
                Window = TimeSpan.FromSeconds(rateLimitSettings.GetValue<int>("Window"))
            }));
    
    options.RejectionStatusCode = 429;
});

// Register services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Fertilizer Warehouse API", 
        Version = "v1",
        Description = "API for Fertilizer Warehouse Management System",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Email = "dev@fertilizerwarehouse.com"
        }
    });

    // JWT Authentication setup in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure HSTS
if (securitySettings.GetValue<bool>("EnableHsts"))
{
    builder.Services.AddHsts(options =>
    {
        options.Preload = securitySettings.GetValue<bool>("Preload");
        options.IncludeSubDomains = securitySettings.GetValue<bool>("IncludeSubDomains");
        options.MaxAge = TimeSpan.FromSeconds(securitySettings.GetValue<int>("MaxAge"));
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fertilizer Warehouse API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}
else
{
    app.UseHsts();
}

// Security middlewares
// app.UseMiddleware<SecurityHeadersMiddleware>(); // Temporarily disabled for CORS testing
// app.UseMiddleware<AuditMiddleware>(); // Temporarily disabled due to foreign key constraint issues

// CORS must be first
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoints
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });
app.MapGet("/", () => new { 
    Message = "Fertilizer Warehouse API", 
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow 
});

// Auto-migrate database
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        // Tạo database và chạy migrations
        context.Database.Migrate();
        Log.Information("Database migration completed successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while migrating the database");
    }
}

Log.Information("Starting Fertilizer Warehouse API");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
