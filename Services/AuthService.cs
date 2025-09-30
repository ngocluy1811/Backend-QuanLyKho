using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.DTOs;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;

    public AuthService(
        ApplicationDbContext context, 
        IJwtService jwtService, 
        ILogger<AuthService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
        _configuration = configuration;
    }

    public async System.Threading.Tasks.Task<AuthResponseDto> LoginAsync(LoginDto loginDto, string ipAddress, string userAgent)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsActive);

            if (user == null)
            {
                _logger.LogWarning("Login attempt with invalid username: {Username}", loginDto.Username);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Check if account is locked
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Account is locked until {user.LockoutEnd:yyyy-MM-dd HH:mm:ss}"
                };
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                // Increment failed attempts
                user.FailedLoginAttempts++;
                
                // Lock account after 5 failed attempts
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
                    _logger.LogWarning("Account locked for user: {Username} due to multiple failed attempts", user.Username);
                }

                await _context.SaveChangesAsync();

                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Reset failed attempts on successful login
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLoginAt = DateTime.UtcNow;

            // Generate tokens
            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Update refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                Convert.ToInt32(_configuration["JwtSettings:RefreshTokenExpiryInDays"]));

            // Create user session
            var userSession = new UserSession
            {
                UserId = user.Id,
                Token = token,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                LoginTime = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(
                    Convert.ToInt32(_configuration["JwtSettings:TokenExpiryInHours"])),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserSessions.Add(userSession);
            await _context.SaveChangesAsync();

            // Create audit log
            await CreateAuditLog(user.Id, "LOGIN", "User", user.Id, null, null, ipAddress, userAgent);

            var userDto = new UserDto
            {
                Id = user.Id,
                CompanyId = user.CompanyId,
                    CompanyName = user.Company.CompanyName,
                DepartmentId = user.DepartmentId,
                    DepartmentName = user.Department?.Name,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Role = GetRoleString(user.Role),
                Level = user.Level,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            };

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiry = userSession.ExpiresAt,
                User = userDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Username}", loginDto.Username);
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during login"
            };
        }
    }

    public async System.Threading.Tasks.Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress, string userAgent)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.IsActive);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid or expired refresh token"
                };
            }

            // Generate new tokens
            var newToken = _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                Convert.ToInt32(_configuration["JwtSettings:RefreshTokenExpiryInDays"]));

            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                CompanyId = user.CompanyId,
                    CompanyName = user.Company.CompanyName,
                DepartmentId = user.DepartmentId,
                    DepartmentName = user.Department?.Name,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Role = GetRoleString(user.Role),
                Level = user.Level,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            };

            return new AuthResponseDto
            {
                Success = true,
                Message = "Token refreshed successfully",
                Token = newToken,
                RefreshToken = newRefreshToken,
                TokenExpiry = DateTime.UtcNow.AddHours(
                    Convert.ToInt32(_configuration["JwtSettings:TokenExpiryInHours"])),
                User = userDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during token refresh"
            };
        }
    }

    public async System.Threading.Tasks.Task<bool> LogoutAsync(string token, int userId)
    {
        try
        {
            // Blacklist the token
            await _jwtService.BlacklistTokenAsync(token, DateTime.UtcNow.AddHours(24));

            // Clear refresh token
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _context.SaveChangesAsync();
            }

            // Create audit log
            await CreateAuditLog(userId, "LOGOUT", "User", userId, null, null, "", "");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
            return false;
        }
    }

    public async System.Threading.Tasks.Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            // Hash new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            // Create audit log
            await CreateAuditLog(userId, "CHANGE_PASSWORD", "User", userId, null, null, "", "");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
            return false;
        }
    }

    public async System.Threading.Tasks.Task<bool> ForgotPasswordAsync(string email)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
            if (user == null)
            {
                // Don't reveal if email exists or not for security
                return true;
            }

            // Generate reset token (implement email sending logic here)
            var resetToken = Guid.NewGuid().ToString();
            
            // Store reset token in database or cache with expiry
            // Implementation depends on your requirements

            _logger.LogInformation("Password reset requested for email: {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password for email: {Email}", email);
            return false;
        }
    }

    public async System.Threading.Tasks.Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            // Validate reset token (implement token validation logic)
            // This is a simplified implementation
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == resetPasswordDto.Email && u.IsActive);
            if (user == null)
            {
                return false;
            }

            // Hash new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Create audit log
            await CreateAuditLog(user.Id, "RESET_PASSWORD", "User", user.Id, null, null, "", "");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for email: {Email}", resetPasswordDto.Email);
            return false;
        }
    }

    public async System.Threading.Tasks.Task<UserDto> GetUserProfileAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Company)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        return new UserDto
        {
            Id = user.Id,
            CompanyId = user.CompanyId,
                    CompanyName = user.Company.CompanyName,
            DepartmentId = user.DepartmentId,
                    DepartmentName = user.Department?.Name,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            Role = GetRoleString(user.Role),
            Level = user.Level,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt
        };
    }

    public async System.Threading.Tasks.Task<bool> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            var oldValues = new { user.FullName, user.Email, user.Phone };

            user.FullName = updateProfileDto.FullName;
            if (!string.IsNullOrEmpty(updateProfileDto.Email))
                user.Email = updateProfileDto.Email;
            user.Phone = updateProfileDto.Phone;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            var newValues = new { user.FullName, user.Email, user.Phone };

            // Create audit log
            await CreateAuditLog(userId, "UPDATE_PROFILE", "User", userId, 
                System.Text.Json.JsonSerializer.Serialize(oldValues),
                System.Text.Json.JsonSerializer.Serialize(newValues), "", "");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user: {UserId}", userId);
            return false;
        }
    }

    private async System.Threading.Tasks.Task CreateAuditLog(int userId, string action, string entityType, int? entityId, 
        string? oldValues, string? newValues, string ipAddress, string userAgent)
    {
        try
        {
            var auditLog = new AuditLog
            {
                CompanyId = (await _context.Users.FindAsync(userId))?.CompanyId ?? 1,
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating audit log");
        }
    }

    private string GetRoleString(Models.Enums.UserRole role)
    {
        return role switch
        {
            Models.Enums.UserRole.Admin => "admin",
            Models.Enums.UserRole.Warehouse => "warehouse",
            Models.Enums.UserRole.TeamLeader => "team_leader",
            Models.Enums.UserRole.Sales => "sales",
            _ => "unknown"
        };
    }
}
