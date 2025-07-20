using BE_PRN232.Entities;
using BE_PRN232.ResponseDTO;
using BE_PRN232.Service;
using BE_PRN232.Service.ServiceImp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BE_PRN232.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly IEmailService _emailService;
        private readonly EcommerceClothingDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        public UsersController(JWTService jwtService, IUserService userService, IEmailService emailService, EcommerceClothingDbContext context, IConfiguration configuration)
        {
            _jwtService = jwtService;
            _userService = userService;
            _emailService = emailService;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("UserProfile")]
        public async Task<IActionResult> UserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return Unauthorized();
            User user = new User();
            try
            {
                user = await _context.Users.FindAsync(Guid.Parse(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(_configuration["Error:Code301"]);
            }

            if (user == null || !user.EmailVerified) return BadRequest(_configuration["Error:Code404"]);
            try {
                var result = UserDTO.ToUserDTO(user);
                return Ok(result);
            } catch (Exception ex) 
            {
                return BadRequest(_configuration["Error:Code303"]);
            }
            
        }

        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UserProfileUpdate([FromBody] UserDTO userDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return Unauthorized();
            string result = _userService.UpdateUserProfile(userDTO, userId,_context);
            
            if (result.Equals(_configuration["Error:Code303"]))
                return BadRequest(result);
            if (result.Equals(_configuration["Error:Code301"]))
                return BadRequest(result);
           
            return Ok("Cập nhật thành công!");
        }


    }
}
