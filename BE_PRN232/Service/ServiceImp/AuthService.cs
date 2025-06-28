using BE_PRN232.Entities;
using BE_PRN232.RequestDTO;
using BE_PRN232.ResponseDTO;
using Microsoft.EntityFrameworkCore;

namespace BE_PRN232.Service.ServiceImp
{
    public class AuthService : IAuthService
    {
        private readonly IEmailService _emailService;
        public AuthService() { }
        public AuthService(IEmailService emailService) {
            _emailService = emailService;
        }

        public bool Login(LoginRequest request, User user)
        {
            Console.WriteLine("Login");
            Console.WriteLine("email:" + request.Email + " Pass:" + request.Password);
            Console.WriteLine("email:" + user.Email + " Pass:" + user.PasswordHash);
            if (user == null || !user.IsActive)
                return false;

            //Check xem email đã được kích hoạt chưa
            bool isEmailActive = user.EmailVerified;
            if (!isEmailActive)
                return false;

            // So sánh mật khẩu thô request.Password với hash trong database
            //bool isValid = request.Password.Equals(user.PasswordHash);  
            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            Console.WriteLine("isvalid:" + isValid);
            if (!isValid)
                return false;
            
            return true;
        }

        public async Task<bool> register(RegisterRequest request, EcommerceClothingDbContext _context)
        {
            try
            {
                //Đổi giữ liệu từ request sang user
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

                var token = Guid.NewGuid().ToString(); // Token dạng chuỗi ngẫu nhiên
                var verifyToken = new Entities.EmailVerificationToken
                {
                    UserId = user.UserId,
                    Token = token,
                    ExpiredAt = DateTime.UtcNow.AddMinutes(10)
                };

                await _context.Users.AddAsync(user);
                //Thêm verifyToken vào database để đợi xác nhận từ mail
                await _context.EmailVerificationTokens.AddAsync(verifyToken);
                await _context.SaveChangesAsync();

                // Gửi email xác minh với link
                await _emailService.SendEmailVerificationLinkAsync(request.Email, user.UserId.ToString(), token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return false;
            }
           


            return true;
        }
    }
}
