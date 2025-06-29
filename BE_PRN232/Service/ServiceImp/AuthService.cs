using BE_PRN232.Entities;
using BE_PRN232.RequestDTO;
using BE_PRN232.ResponseDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json.Linq;
using System.Text;

namespace BE_PRN232.Service.ServiceImp
{
    public class AuthService : IAuthService
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;


        public AuthService() { }
        public AuthService(IConfiguration configuration, IEmailService emailService)
        {
            _emailService = emailService;
            _configuration = configuration;
        }

        public bool Login(LoginRequest request, User user)
        {

            if (user == null || !user.IsActive)
                return false;

            //Check xem email đã được kích hoạt chưa
            bool isEmailActive = user.EmailVerified;
            if (!isEmailActive)
                return false;

            // So sánh mật khẩu thô request.Password với hash trong database
            //bool isValid = request.Password.Equals(user.PasswordHash); test only 
            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            Console.WriteLine("isvalid:" + isValid);
            if (!isValid)
                return false;

            return true;
        }

        public async Task<string?> register(RegisterRequest request, EcommerceClothingDbContext _context)
        {

            //Kiểm tra xem email đã tồn tại chưa
            var userExists = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (userExists)
                return _configuration["Error:Code501"];


            // Đổi giữ liệu từ request sang user
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                EmailVerified = false,
                IsActive = true
            };

            // Token dạng chuỗi ngẫu nhiên
            var token = Guid.NewGuid().ToString();

            var verifyToken = new Entities.EmailVerificationToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiredAt = DateTime.UtcNow.AddMinutes(10),
                Purpose = "VerifyEmail"
            };

            //Tương tác với database
            try
            {
                await _context.Users.AddAsync(user);
                await _context.EmailVerificationTokens.AddAsync(verifyToken);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return _configuration["Error:Code301"];
            }

            // Gửi email xác minh với link
            try
            {
                var verifyLink = $"https://localhost:7217/api/Authentication/verify-email?userId={user.UserId}&token={token}";
                await _emailService.SendEmailVerificationLinkAsync(request.Email, verifyLink);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                _context.Users.Remove(user);
                _context.EmailVerificationTokens.Remove(verifyToken);
                await _context.SaveChangesAsync();
                return _configuration["Error:Code302"];
            }

            return _configuration["Error:Code201"];
        }


        public async Task<string?> resetPassword(ResetPasswordRequest request, EcommerceClothingDbContext _context)
        {
            if (request.NewPassword != request.ConfirmPassword)
                return _configuration["Error:Code602"];
            try
            {
                    var tokenEntry = await _context.EmailVerificationTokens.FirstOrDefaultAsync(t =>
                t.UserId == request.UserId &&
                t.Token == request.Token &&
                t.Purpose == request.Purpose &&
                t.ExpiredAt > DateTime.UtcNow);

                if (tokenEntry == null)
                    return _configuration["Error:Code601"];


                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                    return _configuration["Error:Code404"];

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                _context.EmailVerificationTokens.Remove(tokenEntry);

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return _configuration["Error:Code301"];
            }
            return _configuration["Error:Code201"];

        }
    }
}
