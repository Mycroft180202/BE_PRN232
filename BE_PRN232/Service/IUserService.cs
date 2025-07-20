using BE_PRN232.Entities;
using BE_PRN232.ResponseDTO;

namespace BE_PRN232.Service
{
    public interface IUserService
    {
        string UpdateUserProfile(UserDTO userDTO, string userId, EcommerceClothingDbContext _context);
    }
}
