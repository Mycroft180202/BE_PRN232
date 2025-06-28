using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace BE_PRN232.Service.ServiceImp
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailVerificationLinkAsync(string toEmail, string userId, string token)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("EcommerceClothing", _configuration["Email:From"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Xác minh địa chỉ email";

            // Đường link xác minh (thay yourdomain.com bằng frontend hoặc backend của bạn)
            var verifyLink = $"https://localhost:7217/api/Authentication/verify-email?userId={userId}&token={token}";

            message.Body = new TextPart("html")
            {
                Text = $@"<p>Chào bạn,</p>
                  <p>Vui lòng nhấp vào liên kết dưới đây để xác minh địa chỉ email của bạn:</p>
                  <p><a href='{verifyLink}'>Nhấn để xác nhận</a></p>
                  <p>Liên kết sẽ hết hạn sau 10 phút.</p>"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["Email:SmtpServer"],
                int.Parse(_configuration["Email:Port"]),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(_configuration["Email:From"], _configuration["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
