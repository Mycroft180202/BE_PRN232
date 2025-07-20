using BE_PRN232.Entities;
using BE_PRN232.RequestDTO;
using BE_PRN232.ResponseDTO;

namespace BE_PRN232.Service
{
    public interface IUserService
    {
        Task<string?> UpdateUserProfile(UserDTO userDTO, string userId, EcommerceClothingDbContext _context);
        Task<List<UserDTO>?>  GetAllUser(EcommerceClothingDbContext _context, string? search, bool? isActive);
        Task<UserDTO> GetUser(EcommerceClothingDbContext _context, string Email);
        Task<string?> AdminUpdateUser(AdminUpdateUserRequest userDTO, string Email, EcommerceClothingDbContext _context);
        Task<string?> UpdateUserProfileAdress(UserAddressDTO addressRequest, EcommerceClothingDbContext _context);
        Task<string?> CreateUserProfileAdress(AddressRequest addressRequest, string userId, EcommerceClothingDbContext _context);
        Task<List<UserAddressDTO>> GetAllUserAdress(EcommerceClothingDbContext _context, string? userId);
        Task<UserAddressDTO> GetUserAdress(EcommerceClothingDbContext _context, int Id);
        Task<string?> RemoveUserAddress(EcommerceClothingDbContext _context, int Id);
    }
}
