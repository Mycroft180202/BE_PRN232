using BE_PRN232.Entities;
using BE_PRN232.RequestDTO;
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

        public async Task<string?> UpdateUserProfile(UserDTO userDTO, string userId, EcommerceClothingDbContext _context)
        {
            DateTime now = DateTime.Now;
            User user = await _context.Users.FindAsync(Guid.Parse(userId));
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

        public async Task<List<UserDTO>> GetAllUser(EcommerceClothingDbContext _context, string? search, bool? isActive)
        {

            var userList = await _context.Users.ToListAsync();

            if (!string.IsNullOrEmpty(search))
                userList = userList.Where( u => u.FirstName.ToLower().Contains(search) 
                || u.LastName.ToLower().Contains(search)
                || u.Email.Contains(search))
                    .ToList();

            if(isActive.HasValue)
                userList = userList.Where(u => u.IsActive ==  isActive).ToList();


            var result =  userList.Select(u => UserDTO.ToUserDTO(u)).ToList();

            

            return result;
        }

        public async Task<UserDTO> GetUser(EcommerceClothingDbContext _context, string Email)
        {

            var userList = await _context.Users.FirstAsync(u => u.Email.Equals(Email));

            return UserDTO.ToUserDTO(userList);
        }


        public async Task<string?> AdminUpdateUser(AdminUpdateUserRequest userDTO, string Email, EcommerceClothingDbContext _context)
        {
            User user = await _context.Users.FirstAsync(u => u.Email.Equals(Email));
            bool gmailVerified = user.EmailVerified;
            
            if (userDTO.EmailVerified == false && gmailVerified == true) return _configuration["Error:Code603"];
            
                try
            {
                AdminUpdateUserRequest.UpdateUserFromDTO(user, userDTO);

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


        public async Task<List<UserAddressDTO>> GetAllUserAdress(EcommerceClothingDbContext _context, string? userId)
        {
            
            var userAdressList = await _context.Addresses.Where( a => a.UserId == Guid.Parse(userId)).ToListAsync();

            var result = userAdressList.Select(a => UserAddressDTO.FromEntityToDTO(a)).ToList();

            return result;
        }
        public async Task<UserAddressDTO> GetUserAdress(EcommerceClothingDbContext _context, int Id)
        {
            
            var userAdressList = await _context.Addresses.FindAsync(Id);

            var result = UserAddressDTO.FromEntityToDTO(userAdressList);

            return result;
        }
       

        public async Task<string?> UpdateUserProfileAdress(UserAddressDTO addressRequest, EcommerceClothingDbContext _context)
        {
            var address = await _context.Addresses.FindAsync(addressRequest.AddressId);
            if (address == null) return _configuration["Error:Code403"];
            try
            {
                UserAddressDTO.UpdateAddress(address, addressRequest);

            }
            catch (Exception ex)
            {
                return _configuration["Error:Code303"];
            }
          
                try
            {
                _context.Addresses.Update(address);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return _configuration["Error:Code301"];
            }
            return _configuration["Error:Code201"];

        }


        public async Task<string?> CreateUserProfileAdress(AddressRequest addressRequest,string userId, EcommerceClothingDbContext _context)
        {

            User user = await _context.Users.FindAsync(Guid.Parse(userId));
            var address = new Address();
            try
            {
                address = AddressRequest.FromRequestToEntity( addressRequest, user);

            }
            catch (Exception ex)
            {
                return _configuration["Error:Code303"];
            }
          
                try
            {
                _context.Addresses.Add(address);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return _configuration["Error:Code301"];
            }
            return _configuration["Error:Code201"];
        }

        public async Task<string?> RemoveUserAddress(EcommerceClothingDbContext _context, int Id)
        {
            var userAdressList = await _context.Addresses.FindAsync(Id);
            if (userAdressList == null) return _configuration["Error:Code403"];
            try
            {
                _context.Addresses.Remove(userAdressList);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                return _configuration["Error:Code301"];
            }
           return _configuration["Error:Code201"];
        }
    }
}
