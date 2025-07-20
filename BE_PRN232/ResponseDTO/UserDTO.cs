using BE_PRN232.Entities;

namespace BE_PRN232.ResponseDTO
{
    public class UserDTO
    {

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public bool EmailVerified { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public UserDTO() { }
        public static UserDTO ToUserDTO(User user)
        {
            return new UserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailVerified = user.EmailVerified,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
        public static User ToUser(UserDTO userDTO , Guid Id)
        {
            return new User
            {
                UserId = Id,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                PhoneNumber = userDTO.PhoneNumber,
                EmailVerified = userDTO.EmailVerified,
                IsActive = userDTO.IsActive,
                CreatedAt = userDTO.CreatedAt,
                UpdatedAt = userDTO.UpdatedAt
            };
        }

        public static void UpdateUserFromDTO(User user, UserDTO dto)
        {
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.EmailVerified = dto.EmailVerified;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.Now;
        }
    }
}
