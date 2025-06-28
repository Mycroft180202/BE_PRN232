using BE_PRN232.Entities;
using BE_PRN232.RequestDTO;

namespace BE_PRN232.Service
{
    public interface IAuthService
    {
        bool Login(LoginRequest request, User user);
        Task<bool> register(RegisterRequest request, EcommerceClothingDbContext _context);
    }
}
