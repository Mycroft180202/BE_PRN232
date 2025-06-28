using BE_PRN232.Entities;
using BE_PRN232.RequestDTO;
using BE_PRN232.ResponseDTO;
using Microsoft.EntityFrameworkCore;

namespace BE_PRN232.Service.ServiceImp
{
    public class AuthService : IAuthService
    {
        public AuthService() { }

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
    }
}
