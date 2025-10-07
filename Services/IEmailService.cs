namespace FertilizerWarehouseAPI.Services
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string username, string newPassword);
        Task<bool> SendPasswordResetRequestNotificationAsync(string adminEmail, string username, string userEmail, string message);
        Task<bool> SendPasswordResetApprovalEmailAsync(string toEmail, string username);
        Task<bool> SendPasswordResetRejectionEmailAsync(string toEmail, string username, string reason);
    }
}
