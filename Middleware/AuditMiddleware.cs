using System.Security.Claims;
using System.Text;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
    {
        // Only audit specific endpoints
        if (ShouldAudit(context.Request.Path))
        {
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var startTime = DateTime.UtcNow;

            try
            {
                await _next(context);

                // Log successful requests
                if (context.Response.StatusCode < 400)
                {
                    await LogAuditAsync(context, startTime, "SUCCESS");
                }
            }
            catch (Exception ex)
            {
                // Log failed requests
                await LogAuditAsync(context, startTime, "ERROR");
                _logger.LogError(ex, "Request failed: {Method} {Path}", context.Request.Method, context.Request.Path);
                throw;
            }
            finally
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        else
        {
            await _next(context);
        }
    }

    private static bool ShouldAudit(PathString path)
    {
        var auditPaths = new[]
        {
            "/api/auth/login",
            "/api/auth/logout",
            "/api/users",
            "/api/products",
            "/api/warehouses",
            "/api/stock-movements",
            "/api/purchase-orders",
            "/api/sales-orders"
        };

        return auditPaths.Any(auditPath => path.StartsWithSegments(auditPath));
    }

    private async System.Threading.Tasks.Task LogAuditAsync(HttpContext context, DateTime startTime, string status)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = GetUserId(context);
            var companyId = GetCompanyId(context);

            // Skip audit log if no valid user ID (for anonymous requests)
            if (userId == null || userId == 0)
            {
                return;
            }

            var auditLog = new AuditLog
            {
                CompanyId = companyId,
                UserId = userId.Value,
                Action = $"{context.Request.Method} {context.Request.Path}",
                EntityType = "HTTP_REQUEST",
                EntityId = null,
                OldValues = null,
                NewValues = status,
                IpAddress = GetClientIpAddress(context),
                UserAgent = context.Request.Headers.UserAgent.ToString(),
                Timestamp = startTime
            };

            dbContext.AuditLogs.Add(auditLog);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create audit log");
        }
    }

    private static int? GetUserId(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }

    private static int GetCompanyId(HttpContext context)
    {
        var companyIdClaim = context.User.FindFirst("CompanyId");
        if (companyIdClaim != null && int.TryParse(companyIdClaim.Value, out var companyId))
        {
            return companyId;
        }
        return 1; // Default company
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        var xForwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            return xForwardedFor.Split(',')[0].Trim();
        }

        var xRealIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp))
        {
            return xRealIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}
