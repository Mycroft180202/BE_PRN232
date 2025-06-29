using BE_PRN232.Entities;
using BE_PRN232.ResponseDTO;
using BE_PRN232.Service;
using BE_PRN232.Service.ServiceImp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        private readonly IConfiguration _configuration;
        public AuthenticationController(JWTService jwtService, IAuthService authService, IEmailService emailService, EcommerceClothingDbContext context, IConfiguration configuration)
        {
            _jwtService = jwtService;
            _authService = authService;
            _emailService = emailService;
            _context = context;
            _configuration = configuration;
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
            //Thực hiện register trong Service
            var isDone = await _authService.register(request, _context);
            if (isDone.Equals(_configuration["Error:Code501"]))
                return BadRequest("Email đã tồn tại");


            if (isDone.Equals(_configuration["Error:Code301"]))
                return Ok("Tài khoản chưa được tạo. Vui lòng kiểm tra lại thông tin.");


            if (isDone.Equals(_configuration["Error:Code302"]))
                return Ok("Tài khoản chưa được tạo. Vui lòng kiểm tra lại thông tin.");
            

            return Ok("Tài khoản được tạo. Vui lòng kiểm tra email để xác minh.");
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token)
        {
            var record = await _context.EmailVerificationTokens
                .FirstOrDefaultAsync(t => t.Token == token && t.UserId == userId && t.Purpose == "VerifyEmail");

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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] RequestDTO.ForgotPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return BadRequest(_configuration["Error:Code405"]);

            var token = Guid.NewGuid().ToString();

            var tokenEntry = new Entities.EmailVerificationToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiredAt = DateTime.UtcNow.AddMinutes(10),
                Purpose = "ResetPassword"
            };

            _context.EmailVerificationTokens.Add(tokenEntry);
            await _context.SaveChangesAsync();


            //Đây chỉ là link minh họa cần link front end để gửi
            var resetLink = $"https://localhost:7217/api/Authentication/reset-password?userId={user.UserId}&token={token}";
            // Gửi link có token và userId
            await _emailService.SendResetPasswordLinkAsync(user.Email, resetLink);

            return Ok("Hướng dẫn lấy lại mật khẩu đã được gửi đến email.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] RequestDTO.ResetPasswordRequest request)
        {
           var isResetpassword = await _authService.resetPassword(request , _context);
            Console.WriteLine($"isResetpassword:{isResetpassword} + {_configuration["Error:Code602"]}");
            if (isResetpassword.Equals(_configuration["Error:Code602"]))
                return BadRequest(_configuration["Error:Code602"]);

            if(isResetpassword.Equals(_configuration["Error:Code601"]))
                return BadRequest(_configuration["Error:Code601"]);

            if (isResetpassword.Equals(_configuration["Error:Code404"]))
                return BadRequest(_configuration["Error:Code404"]);

            if (isResetpassword.Equals(_configuration["Error:Code301"]))
                return BadRequest(_configuration["Error:Code301"]);

            return Ok("Đổi mật khẩu thành công.");
        }
        //[Authorize]
        [HttpPost("request-change-password")]
        public async Task<IActionResult> RequestChangePassword()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return Unauthorized();

            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user == null || !user.EmailVerified) return BadRequest(_configuration["Error:Code404"]);

            // Tạo token mới
            var token = Guid.NewGuid().ToString();


            // Thêm token vào bảng EmailVerificationToken
            var emailToken = new Entities.EmailVerificationToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiredAt = DateTime.UtcNow.AddMinutes(10),
                Purpose = "ResetPassword"
            };

            _context.EmailVerificationTokens.Add(emailToken);
            await _context.SaveChangesAsync();

            // Gửi mail
            //Đây chỉ là link minh họa cần link front end để gửi
            var resetLink = $"https://localhost:7217/api/Authentication/reset-password?userId={userId}&token={token}";
            await _emailService.SendChangePasswordLinkAsync(user.Email, resetLink);

            return Ok("Email đã đuợc gửi.");
        }



    }

}

