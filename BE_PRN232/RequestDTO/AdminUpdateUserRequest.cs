using BE_PRN232.Entities;
using BE_PRN232.ResponseDTO;

namespace BE_PRN232.RequestDTO
{
    public class AdminUpdateUserRequest
    {

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public bool EmailVerified { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public static void UpdateUserFromDTO(User user, AdminUpdateUserRequest request)
        {
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.EmailVerified = request.EmailVerified;
            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.Now;
        }
    }
}
