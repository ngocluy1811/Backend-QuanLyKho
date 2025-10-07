using System.Net;
using System.Net.Mail;
using System.Text;

namespace FertilizerWarehouseAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string username, string newPassword)
        {
            try
            {
                var subject = "Mật khẩu mới - Hệ thống Quản lý Kho";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #2c5aa0; text-align: center;'>🔐 Mật khẩu mới của bạn</h2>
                            
                            <p>Xin chào <strong>{username}</strong>,</p>
                            
                            <p>Quản trị viên đã cấp lại mật khẩu mới cho tài khoản của bạn:</p>
                            
                            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0; text-align: center;'>
                                <h3 style='color: #2c5aa0; margin: 0;'>Mật khẩu mới: <code style='background-color: #e9ecef; padding: 5px 10px; border-radius: 3px;'>{newPassword}</code></h3>
                            </div>
                            
                            <p><strong>Lưu ý quan trọng:</strong></p>
                            <ul>
                                <li>Vui lòng đăng nhập và thay đổi mật khẩu ngay lập tức</li>
                                <li>Không chia sẻ mật khẩu này với bất kỳ ai</li>
                                <li>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng liên hệ quản trị viên</li>
                            </ul>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='http://localhost:3000/login' style='background-color: #2c5aa0; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>Đăng nhập ngay</a>
                            </div>
                            
                            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                            <p style='font-size: 12px; color: #666; text-align: center;'>
                                Email này được gửi tự động từ Hệ thống Quản lý Kho.<br>
                                Vui lòng không trả lời email này.
                            </p>
                        </div>
                    </body>
                    </html>";

                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetRequestNotificationAsync(string adminEmail, string username, string userEmail, string message)
        {
            try
            {
                var subject = $"Yêu cầu cấp lại mật khẩu - {username}";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #dc3545; text-align: center;'>⚠️ Yêu cầu cấp lại mật khẩu</h2>
                            
                            <p>Quản trị viên thân mến,</p>
                            
                            <p>Có một yêu cầu cấp lại mật khẩu mới từ người dùng:</p>
                            
                            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                                <p><strong>Tên đăng nhập:</strong> {username}</p>
                                <p><strong>Email:</strong> {userEmail}</p>
                                <p><strong>Thời gian:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
                                <p><strong>Nội dung:</strong></p>
                                <div style='background-color: white; padding: 10px; border-left: 4px solid #2c5aa0; margin: 10px 0;'>
                                    {message ?? "Không có nội dung bổ sung"}
                                </div>
                            </div>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='http://localhost:3000/user-management' style='background-color: #2c5aa0; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>Xem chi tiết</a>
                            </div>
                            
                            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                            <p style='font-size: 12px; color: #666; text-align: center;'>
                                Email thông báo từ Hệ thống Quản lý Kho
                            </p>
                        </div>
                    </body>
                    </html>";

                return await SendEmailAsync(adminEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset request notification to admin {Email}", adminEmail);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetApprovalEmailAsync(string toEmail, string username)
        {
            try
            {
                var subject = "Yêu cầu cấp lại mật khẩu đã được duyệt";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #28a745; text-align: center;'>✅ Yêu cầu đã được duyệt</h2>
                            
                            <p>Xin chào <strong>{username}</strong>,</p>
                            
                            <p>Yêu cầu cấp lại mật khẩu của bạn đã được quản trị viên duyệt. Mật khẩu mới sẽ được gửi đến email này trong vài phút tới.</p>
                            
                            <div style='background-color: #d4edda; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #28a745;'>
                                <p style='margin: 0;'><strong>Trạng thái:</strong> Đã duyệt</p>
                                <p style='margin: 5px 0 0 0;'><strong>Thời gian:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
                            </div>
                            
                            <p>Vui lòng kiểm tra email để nhận mật khẩu mới và đăng nhập vào hệ thống.</p>
                            
                            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                            <p style='font-size: 12px; color: #666; text-align: center;'>
                                Email này được gửi tự động từ Hệ thống Quản lý Kho
                            </p>
                        </div>
                    </body>
                    </html>";

                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset approval email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetRejectionEmailAsync(string toEmail, string username, string reason)
        {
            try
            {
                var subject = "Yêu cầu cấp lại mật khẩu bị từ chối";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #dc3545; text-align: center;'>❌ Yêu cầu bị từ chối</h2>
                            
                            <p>Xin chào <strong>{username}</strong>,</p>
                            
                            <p>Yêu cầu cấp lại mật khẩu của bạn đã bị từ chối bởi quản trị viên.</p>
                            
                            <div style='background-color: #f8d7da; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #dc3545;'>
                                <p style='margin: 0;'><strong>Lý do từ chối:</strong></p>
                                <p style='margin: 5px 0 0 0;'>{reason}</p>
                            </div>
                            
                            <p>Nếu bạn cần hỗ trợ thêm, vui lòng liên hệ trực tiếp với quản trị viên.</p>
                            
                            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                            <p style='font-size: 12px; color: #666; text-align: center;'>
                                Email này được gửi tự động từ Hệ thống Quản lý Kho
                            </p>
                        </div>
                    </body>
                    </html>";

                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset rejection email to {Email}", toEmail);
                return false;
            }
        }

        private async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // For development, we'll just log the email content
                // In production, configure actual SMTP settings
                _logger.LogInformation("=== EMAIL SENT ===");
                _logger.LogInformation("To: {ToEmail}", toEmail);
                _logger.LogInformation("Subject: {Subject}", subject);
                _logger.LogInformation("Body: {Body}", body);
                _logger.LogInformation("==================");

                // TODO: Implement actual SMTP sending
                // var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                // smtpClient.Credentials = new NetworkCredential("your-email@gmail.com", "your-password");
                // smtpClient.EnableSsl = true;
                // 
                // var mailMessage = new MailMessage
                // {
                //     From = new MailAddress("your-email@gmail.com"),
                //     Subject = subject,
                //     Body = body,
                //     IsBodyHtml = true
                // };
                // mailMessage.To.Add(toEmail);
                // 
                // await smtpClient.SendMailAsync(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", toEmail);
                return false;
            }
        }
    }
}
