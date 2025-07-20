using BE_PRN232.Entities;
using BE_PRN232.RequestDTO;
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
            try
            {
                var result = UserDTO.ToUserDTO(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(_configuration["Error:Code303"]);
            }

        }

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UserProfileUpdate([FromBody] UserDTO userDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return Unauthorized();
            string result = await _userService.UpdateUserProfile(userDTO, userId, _context);

            if (result.Equals(_configuration["Error:Code303"]))
                return BadRequest(result);
            if (result.Equals(_configuration["Error:Code301"]))
                return BadRequest(result);

            return Ok("Cập nhật thành công!");
        }



        [HttpGet("ListUser")]
        public async Task<IActionResult> GetAllUser([FromQuery] string? search, [FromQuery] bool? isActive)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            var result = new List<UserDTO>();
            try
            {
                result = await _userService.GetAllUser(_context, search, isActive);
            }
            catch (Exception ex)
            {
                return BadRequest(_configuration["Error:Code301"]);
            }
            if (!result.Any())
                return Ok("Dont have any user!");
            return Ok(result);
        }

        [HttpGet("User")]
        public async Task<IActionResult> GetUser([FromQuery] string Email)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            var result = new UserDTO();
            try
            {
                result = await _userService.GetUser(_context, Email);
            }
            catch (Exception ex)
            {
                return BadRequest(_configuration["Error:Code301"]);
            }
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPut("AdminUpdateUser")]
        public async Task<IActionResult> AdminUserUpdate([FromBody] AdminUpdateUserRequest userDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return Unauthorized();
            string result = await _userService.AdminUpdateUser(userDTO, userDTO.Email, _context);

            if (result.Equals(_configuration["Error:Code303"]))
                return BadRequest(_configuration["Error:Code303"]);
            if (result.Equals(_configuration["Error:Code301"]))
                return BadRequest(_configuration["Error:Code301"]);
            if (result.Equals(_configuration["Error:Code603"]))
                return BadRequest(_configuration["Error:Code603"]);

            return Ok("Cập nhật thành công!");
        }


        [HttpGet("ListUserAdress")]
        public async Task<IActionResult> GetAllUserAdress()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            var result = new List<UserAddressDTO>();
            try
            {
                result = await _userService.GetAllUserAdress(_context, userId);
            }
            catch (Exception ex)
            {
                return BadRequest(_configuration["Error:Code301"]);
            }
            if (!result.Any())
                return Ok("Dont have any address!");
            return Ok(result);
        }

        [HttpGet("UserAdress/{id}")]
        public async Task<IActionResult> GetAllUserAdress([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            var result = new UserAddressDTO();
            try
            {
                result = await _userService.GetUserAdress(_context, id);
            }
            catch (Exception ex)
            {
                return BadRequest(_configuration["Error:Code301"]);
            }
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("UserAdressRemove/{id}")]
        public async Task<IActionResult> RemoveUserAddress([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var result = await _userService.RemoveUserAddress(_context, id);

            if (result.Equals(_configuration["Error:Code301"]))
                return BadRequest(_configuration["Error:Code301"]);
            if (result.Equals(_configuration["Error:Code403"]))
                return BadRequest(_configuration["Error:Code403"]);

            return Ok("Xóa Address thành công!");
        }

        [HttpPut("UpdateUserProfileAdress")]
        public async Task<IActionResult> UpdateUserProfileAdress([FromBody] UserAddressDTO addressRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return Unauthorized();
            string result = await _userService.UpdateUserProfileAdress(addressRequest, _context);

            if (result.Equals(_configuration["Error:Code303"]))
                return BadRequest(result);
            if (result.Equals(_configuration["Error:Code301"]))
                return BadRequest(result);

            return Ok("Cập nhật thành công!");
        }


        [HttpPost("CreateUserProfileAdress")]
        public async Task<IActionResult> CreateUserProfileAdress([FromBody] AddressRequest addressRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return Unauthorized();
            string result = await _userService.CreateUserProfileAdress(addressRequest, userId, _context);

            if (result.Equals(_configuration["Error:Code303"]))
                return BadRequest(result);
            if (result.Equals(_configuration["Error:Code301"]))
                return BadRequest(result);

            return Ok("Tạo mới thành công!");
        }

    }
}
