using BE_PRN232.Entities;
using BE_PRN232.ResponseDTO;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BE_PRN232.Service.ServiceImp
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        public UserService() { }
        public UserService(IConfiguration configuration)
        {

            _configuration = configuration;
        }

        public string UpdateUserProfile(UserDTO userDTO, string userId, EcommerceClothingDbContext _context)
        {
            DateTime now = DateTime.Now;
            User user = _context.Users.Find(Guid.Parse(userId));
            try
            {
                UserDTO.UpdateUserFromDTO(user, userDTO);
                
            }
            catch (Exception ex)
            {
                return _configuration["Error:Code303"];
            }
            try
            {
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return _configuration["Error:Code301"];
            }
            return _configuration["Error:Code201"];

        }
    }
}
