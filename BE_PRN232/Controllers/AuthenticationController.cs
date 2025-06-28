using BE_PRN232.Entities;
using BE_PRN232.ResponseDTO;
using BE_PRN232.Service;
using BE_PRN232.Service.ServiceImp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE_PRN232.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthenticationController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly EcommerceClothingDbContext _context;
        public AuthenticationController(JWTService jwtService, IAuthService authService, IEmailService emailService, EcommerceClothingDbContext context)
        {
            _jwtService = jwtService;
            _authService = authService;
            _emailService = emailService;
            _context = context;
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] RequestDTO.LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            // Kiểm tra login tài khoản mật khẩu
            if (!_authService.Login(request, user))
                return Unauthorized();

            // Xóa token cũ
            var oldTokens = _context.UserRefreshTokens.Where(t => t.UserId == user.UserId);
            _context.UserRefreshTokens.RemoveRange(oldTokens);

            // Tạo token mới
            var token = _jwtService.GenerateToken(user);

            var refreshToken = new UserRefreshToken
            {
                UserId = user.UserId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };

            _context.UserRefreshTokens.Add(refreshToken);
            _context.SaveChanges();

            // Trả về token và thông tin user (có thể chọn lọc trường)
            return Ok(new
            {
                token,
                userDTO = UserDTO.ToUserDTO(user)
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RequestDTO.RegisterRequest request)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (userExists)
                return BadRequest("Email đã tồn tại");

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

            await _context.Users.AddAsync(user);

            var token = Guid.NewGuid().ToString(); // Token dạng chuỗi ngẫu nhiên
            var verifyToken = new Entities.EmailVerificationToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiredAt = DateTime.UtcNow.AddMinutes(10)
            };

            await _context.EmailVerificationTokens.AddAsync(verifyToken);
            await _context.SaveChangesAsync();

            // Gửi email xác minh với link
            await _emailService.SendEmailVerificationLinkAsync(user.Email, user.UserId.ToString(), token);

            return Ok("Tài khoản được tạo. Vui lòng kiểm tra email để xác minh.");
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token)
        {
            var record = await _context.EmailVerificationTokens
                .FirstOrDefaultAsync(t => t.Token == token && t.UserId == userId);

            if (record == null || record.ExpiredAt < DateTime.UtcNow)
                return BadRequest("Liên kết không hợp lệ hoặc đã hết hạn");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            user.EmailVerified = true;

            _context.EmailVerificationTokens.Remove(record);
            await _context.SaveChangesAsync();

            return Ok("Email đã được xác minh thành công!");
        }

    }

}

