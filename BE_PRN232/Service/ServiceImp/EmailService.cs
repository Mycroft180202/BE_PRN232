using BE_PRN232.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json.Linq;
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
        public async Task SendEmailVerificationLinkAsync(string toEmail, string verifyLink)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("EcommerceClothing", _configuration["Email:From"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Xác minh địa chỉ email";


            message.Body = new TextPart("html")
            {
                Text = $@"<p>Chào bạn,</p>
                  <p>Vui lòng nhấp vào liên kết dưới đây để xác minh địa chỉ email của bạn:</p>
                  <p><a href='{verifyLink}'>Nhấn để xác nhận</a></p>
                  <p>Liên kết sẽ hết hạn sau 10 phút.</p>"
            };

            await SendMail(message);
        }
        public async Task SendResetPasswordLinkAsync(string toEmail, string resetLink)
        {   

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("EcommerceClothing", _configuration["Email:From"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Khôi phục mật khẩu";


            message.Body = new TextPart("html")
            {
                Text = $@"<p>Chúng tôi nhận được yêu cầu lấy lại mật khẩu từ bạn.</p>
                  <p>Bấm vào liên kết bên dưới để lấy lại mật khẩu:</p>
                  <p><a href='{resetLink}'>Nhấn để lấy lại</a></p>
                  <p>Liên kết này sẽ hết hạn sau 10 phút.</p>"
            };

            await SendMail(message);
        }

        public async Task SendChangePasswordLinkAsync(string toEmail, string resetLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("EcommerceClothing", _configuration["Email:From"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Đổi mật khẩu";

            message.Body = new TextPart("html")
            {
                Text = $@"<p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu từ bạn.</p>
                  <p>Bấm vào liên kết bên dưới để thay đổi mật khẩu:</p>
                  <p><a href='{resetLink}'>Nhấn để đổi</a></p>
                  <p>Liên kết này sẽ hết hạn sau 10 phút.</p>"
            };

            await SendMail( message);
        }


        public async Task SendMail(MimeMessage message)
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration["Email:SmtpServer"], int.Parse(_configuration["Email:Port"])
                , SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_configuration["Email:From"], _configuration["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

        }
    }
}
