using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.Services;
using FertilizerWarehouseAPI.DTOs;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Models.Enums;
using FertilizerWarehouseAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;
    private readonly ISecurityService _securityService;
    private readonly ILogger<AuthController> _logger;
    private readonly ApplicationDbContext _context;

    public AuthController(IAuthService authService, IJwtService jwtService, ISecurityService securityService, ILogger<AuthController> logger, ApplicationDbContext context)
    {
        _authService = authService;
        _jwtService = jwtService;
        _securityService = securityService;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var ipAddress = GetClientIpAddress();
            var userAgent = Request.Headers["User-Agent"].ToString();

            var user = await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                // Log failed login attempt
                // Temporarily disabled due to missing LoginHistories table
                /*
                if (user != null)
                {
                    await _securityService.LogFailedLoginAsync(user.Id, ipAddress, userAgent, "Invalid password");
                }
                */

                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid credentials"
                });
            }

            // Check if account is locked
            if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
            {
                // Temporarily disabled due to missing LoginHistories table
                // await _securityService.LogFailedLoginAsync(user.Id, ipAddress, userAgent, "Account locked");
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = $"Account is locked until {user.LockedUntil:yyyy-MM-dd HH:mm:ss} UTC"
                });
            }

            // Check if account should be locked due to suspicious activity
            // Temporarily disabled due to missing LoginHistories table
            /*
            if (await _securityService.ShouldLockAccountAsync(user.Id))
            {
                await _securityService.LockAccountAsync(user.Id, "Multiple failed login attempts", 30);
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = "Account temporarily locked due to suspicious activity"
                });
            }
            */

            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Log successful login with security monitoring
            // Temporarily disabled due to missing LoginHistories table
            // var loginHistory = await _securityService.LogLoginAsync(user.Id, ipAddress, userAgent, true);

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var result = new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiry = DateTime.UtcNow.AddHours(24),
                User = new UserDto
                {
                    Id = user.Id,
                    CompanyId = user.CompanyId,
                    CompanyName = user.Company?.CompanyName ?? "",
                    DepartmentId = user.DepartmentId,
                    DepartmentName = user.Department?.Name ?? "",
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = GetRoleString(user.Role),
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                }
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", loginDto.Username);
            return StatusCode(500, new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during login"
            });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        if (string.IsNullOrEmpty(refreshTokenDto.RefreshToken))
        {
            return BadRequest(new { message = "Refresh token is required" });
        }

        var ipAddress = GetClientIpAddress();
        var userAgent = Request.Headers.UserAgent.ToString();

        var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken, ipAddress, userAgent);

        if (!result.Success)
        {
            return Unauthorized(new { message = result.Message });
        }

        return Ok(result);
    }

    /// <summary>
    /// Logout current user
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var token = GetTokenFromHeader();
        var userId = GetCurrentUserId();

        if (token == null || userId == null)
        {
            return BadRequest(new { message = "Invalid token" });
        }

        var result = await _authService.LogoutAsync(token, userId.Value);

        if (!result)
        {
            return BadRequest(new { message = "Logout failed" });
        }

        // Log logout with security monitoring
        var sessionId = GetSessionIdFromToken(token);
        if (!string.IsNullOrEmpty(sessionId))
        {
            await _securityService.LogLogoutAsync(userId.Value, sessionId);
        }

        return Ok(new { message = "Logout successful" });
    }


    /// <summary>
    /// Request password reset via email
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);

        // Always return success for security reasons
        return Ok(new { message = "If your email is registered, you will receive a password reset link." });
    }

    /// <summary>
    /// Reset password using reset token
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.ResetPasswordAsync(resetPasswordDto);

        if (!result)
        {
            return BadRequest(new { message = "Invalid or expired reset token" });
        }

        return Ok(new { message = "Password reset successfully" });
    }

    /// <summary>
    /// Reset user password by admin
    /// </summary>
    [HttpPost("reset-user-password")]
    [Authorize]
    public async Task<IActionResult> ResetUserPassword([FromBody] ResetUserPasswordRequest request)
    {
        try
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy người dùng" });
            }

            // Hash the new password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.PasswordHash = hashedPassword;
            user.UpdatedAt = DateTime.UtcNow;
            user.MustChangePassword = true; // Force user to change password on next login

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password reset for user {Username} by admin", user.Username);

            return Ok(new
            {
                success = true,
                message = "Mật khẩu đã được cập nhật thành công"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user {UserId}", request.UserId);
            return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi cập nhật mật khẩu" });
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Không xác định được người dùng" });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy người dùng" });
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(new { success = false, message = "Mật khẩu hiện tại không đúng" });
            }

            // Hash new password
            var hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.PasswordHash = hashedNewPassword;
            user.MustChangePassword = false; // User has changed password
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password changed for user {Username}", user.Username);

            return Ok(new
            {
                success = true,
                message = "Mật khẩu đã được thay đổi thành công"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user");
            return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi thay đổi mật khẩu" });
        }
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Không xác định được người dùng" });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy người dùng" });
            }

            // Update user information
            if (!string.IsNullOrEmpty(updateProfileDto.FullName))
                user.FullName = updateProfileDto.FullName;
            
            if (!string.IsNullOrEmpty(updateProfileDto.Email))
                user.Email = updateProfileDto.Email;
            
            if (!string.IsNullOrEmpty(updateProfileDto.Phone))
                user.Phone = updateProfileDto.Phone;

            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Profile updated for user {Username}", user.Username);

            return Ok(new
            {
                success = true,
                message = "Thông tin cá nhân đã được cập nhật thành công",
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    fullName = user.FullName,
                    email = user.Email,
                    phone = user.Phone,
                    role = user.Role.ToString()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user");
            return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi cập nhật thông tin" });
        }
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Không xác định được người dùng" });
            }

            var user = await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy người dùng" });
            }

            var profile = new
            {
                success = true,
                data = new
                {
                    id = user.Id,
                    username = user.Username,
                    fullName = user.FullName,
                    email = user.Email,
                    phone = user.Phone,
                    role = user.Role.ToString(),
                    companyName = user.Company?.CompanyName ?? "",
                    departmentName = user.Department?.Name ?? "",
                    isActive = user.IsActive,
                    createdAt = user.CreatedAt,
                    lastLogin = user.LastLogin
                }
            };

            return Ok(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi lấy thông tin cá nhân" });
        }
    }


    private string? GetTokenFromHeader()
    {
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        return null;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }

    private string GetClientIpAddress()
    {
        var xForwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            return xForwardedFor.Split(',')[0].Trim();
        }

        var xRealIp = Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp))
        {
            return xRealIp;
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    /// <summary>
    /// Register new user
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if username already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == registerDto.Username);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Check if email already exists
            var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            if (existingEmail != null)
            {
                return BadRequest(new { message = "Email already exists" });
            }

            // Get or create default company
            var company = await _context.Companies.FirstOrDefaultAsync();
            if (company == null)
            {
                company = new Company
                {
                    CompanyName = "CÔNG TY CỔ PHẦN A9",
                    Code = "A9",
                    Address = "Địa chỉ Công ty A9",
                    Phone = "0123456789",
                    Email = "admin@default.com",
                    TaxCode = "0000000000",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
            }

            // Get or create default department
            var department = await _context.Departments.FirstOrDefaultAsync();
            if (department == null)
            {
                department = new Department
                {
                    Name = "IT",
                    Description = "IT Department",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
            }

            // Create new user
            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                Role = registerDto.Role,
                CompanyId = company.Id,
                DepartmentId = department.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully", userId = user.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return StatusCode(500, new { message = "Error registering user" });
        }
    }

    /// <summary>
    /// Test database connection
    /// </summary>
    [HttpGet("test-db")]
    [AllowAnonymous]
    public async Task<IActionResult> TestDatabase()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            return Ok(new { 
                canConnect = canConnect,
                message = canConnect ? "Database connection successful" : "Database connection failed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed");
            return StatusCode(500, new { 
                message = "Database connection test failed",
                error = ex.Message,
                innerError = ex.InnerException?.Message
            });
        }
    }

    /// <summary>
    /// Check admin user status
    /// </summary>
    [HttpGet("check-admin")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckAdmin()
    {
        try
        {
            var adminUser = await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Username == "admin");

            if (adminUser == null)
            {
                return Ok(new { 
                    exists = false,
                    message = "Admin user not found"
                });
            }

            return Ok(new { 
                exists = true,
                username = adminUser.Username,
                fullName = adminUser.FullName,
                email = adminUser.Email,
                role = adminUser.Role.ToString(),
                isActive = adminUser.IsActive,
                failedLoginAttempts = adminUser.FailedLoginAttempts,
                lockoutEnd = adminUser.LockoutEnd,
                lastLogin = adminUser.LastLogin,
                companyName = adminUser.Company?.CompanyName,
                departmentName = adminUser.Department?.Name
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking admin user");
            return StatusCode(500, new { 
                message = "Error checking admin user",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Debug login endpoint
    /// </summary>
    [HttpGet("debug-login")]
    [AllowAnonymous]
    public async Task<IActionResult> DebugLogin(string username, string password)
    {
        try
        {
            _logger.LogInformation($"Debug login attempt for username: {username}");
            
            var user = await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                _logger.LogWarning($"User not found: {username}");
                return Ok(new { 
                    success = false, 
                    message = "User not found",
                    username = username,
                    userExists = false
                });
            }

            _logger.LogInformation($"User found: {user.Username}, Role: {user.Role}, IsActive: {user.IsActive}");

            // Check if user is locked out
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                _logger.LogWarning($"User is locked out until: {user.LockoutEnd}");
                return Ok(new { 
                    success = false, 
                    message = "Account is locked out",
                    lockoutEnd = user.LockoutEnd,
                    isLockedOut = true
                });
            }

            // Verify password
            var passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            _logger.LogInformation($"Password verification result: {passwordValid}");

            if (!passwordValid)
            {
                _logger.LogWarning($"Invalid password for user: {username}");
                return Ok(new { 
                    success = false, 
                    message = "Invalid password",
                    passwordValid = false
                });
            }

            _logger.LogInformation($"Login successful for user: {username}");
            return Ok(new { 
                success = true, 
                message = "Login successful",
                user = new {
                    id = user.Id,
                    username = user.Username,
                    role = user.Role.ToString(),
                    isActive = user.IsActive
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in debug login");
            return StatusCode(500, new { 
                message = "Error in debug login",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Unlock admin user
    /// </summary>
    [HttpPost("unlock-admin")]
    [AllowAnonymous]
    public async Task<IActionResult> UnlockAdmin()
    {
        try
        {
            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == "admin");

            if (adminUser == null)
            {
                return NotFound(new { message = "Admin user not found" });
            }

            // Reset failed attempts and unlock account
            adminUser.FailedLoginAttempts = 0;
            adminUser.LockoutEnd = null;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Admin user unlocked successfully");

            return Ok(new { 
                message = "Admin user unlocked successfully",
                username = adminUser.Username
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking admin user");
            return StatusCode(500, new { 
                message = "Error unlocking admin user",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Reset database and create fresh admin user
    /// </summary>
    [HttpPost("reset-database")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetDatabase([FromBody] CreateAdminDto adminDto)
    {
        try
        {
            _logger.LogInformation("Starting database reset process");

            if (string.IsNullOrEmpty(adminDto.Username) || string.IsNullOrEmpty(adminDto.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            // Delete all existing data in correct order (child tables first)
            _context.AuditLogs.RemoveRange(_context.AuditLogs);
            _context.Users.RemoveRange(_context.Users);
            _context.Departments.RemoveRange(_context.Departments);
            _context.Companies.RemoveRange(_context.Companies);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Cleared existing data");

            // Create company
            var company = new Company
            {
                CompanyName = adminDto.CompanyName ?? "Fertilizer Warehouse Company",
                Code = "FWC",
                Address = adminDto.CompanyAddress ?? "Default Address",
                Phone = adminDto.CompanyPhone ?? "0000000000",
                Email = adminDto.CompanyEmail ?? "info@fertilizer.com",
                TaxCode = "1234567890",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Company created with ID: {company.Id}");

            // Create department
            var department = new Department
            {
                Name = "Administration",
                Description = "Administration Department",
                CompanyId = company.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Department created with ID: {department.Id}");

            // Create admin user
            var adminUser = new User
            {
                Username = adminDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminDto.Password),
                FullName = adminDto.FullName ?? "System Administrator",
                Email = adminDto.Email ?? "admin@system.com",
                Phone = adminDto.Phone,
                Role = Models.Enums.UserRole.Admin,
                CompanyId = company.Id,
                DepartmentId = department.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Admin user created with ID: {adminUser.Id}");

            return Ok(new { 
                message = "Database reset and admin user created successfully", 
                username = adminDto.Username,
                fullName = adminDto.FullName ?? "System Administrator",
                email = adminDto.Email ?? "admin@system.com"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database reset: {Message}", ex.Message);
            return StatusCode(500, new { 
                message = "Error during database reset",
                error = ex.Message,
                innerError = ex.InnerException?.Message
            });
        }
    }

    /// <summary>
    /// Create default admin user for testing
    /// </summary>
    [HttpPost("create-admin")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto adminDto)
    {
        try
        {
            _logger.LogInformation("Starting create-admin process");

            if (string.IsNullOrEmpty(adminDto.Username) || string.IsNullOrEmpty(adminDto.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            // Check if admin already exists
            var existingAdmin = await _context.Users.FirstOrDefaultAsync(u => u.Username == adminDto.Username);
            if (existingAdmin != null)
            {
                _logger.LogInformation("Admin user already exists");
                return Ok(new { message = "Admin user already exists", username = adminDto.Username });
            }

            // Get or create company
            var company = await _context.Companies.FirstOrDefaultAsync();
            if (company == null)
            {
                _logger.LogInformation("Creating default company");
                company = new Company
                {
                    CompanyName = adminDto.CompanyName ?? "Fertilizer Warehouse Company",
                    Code = "FWC",
                    Address = adminDto.CompanyAddress ?? "Default Address",
                    Phone = adminDto.CompanyPhone ?? "0000000000",
                    Email = adminDto.CompanyEmail ?? "info@fertilizer.com",
                    TaxCode = "1234567890",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Company created with ID: {company.Id}");
            }

            // Get or create department
            var department = await _context.Departments.FirstOrDefaultAsync();
            if (department == null)
            {
                _logger.LogInformation("Creating default department");
                department = new Department
                {
                    Name = "Administration",
                    Description = "Administration Department",
                    CompanyId = company.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Department created with ID: {department.Id}");
            }

            // Create admin user
            var adminUser = new User
            {
                Username = adminDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminDto.Password),
                FullName = adminDto.FullName ?? "System Administrator",
                Email = adminDto.Email ?? "admin@system.com",
                Phone = adminDto.Phone,
                Role = Models.Enums.UserRole.Admin,
                CompanyId = company.Id,
                DepartmentId = department.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Admin user created with ID: {adminUser.Id}");

            return Ok(new { 
                message = "Admin user created successfully", 
                username = adminDto.Username,
                fullName = adminDto.FullName ?? "System Administrator",
                email = adminDto.Email ?? "admin@system.com"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating admin user: {Message}", ex.Message);
            return StatusCode(500, new { 
                message = "Error creating admin user",
                error = ex.Message,
                innerError = ex.InnerException?.Message
            });
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



    private string? GetSessionIdFromToken(string token)
    {
        try
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            return jsonToken.Claims.FirstOrDefault(x => x.Type == "sessionId")?.Value;
        }
        catch
        {
            return null;
        }
    }
}

public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class ForgotPasswordDto
{
    public string Email { get; set; } = string.Empty;
}
