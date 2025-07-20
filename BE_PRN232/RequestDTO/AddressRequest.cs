using BE_PRN232.Entities;
using BE_PRN232.ResponseDTO;

namespace BE_PRN232.RequestDTO
{
    public class AddressRequest
    {
        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string StreetAddress { get; set; } = null!;

        public string Ward { get; set; } = null!;

        public string District { get; set; } = null!;

        public string City { get; set; } = null!;

        public bool IsDefault { get; set; }

        public static Address FromRequestToEntity(AddressRequest ar, User user)
        {
            return new Address
            {
                FullName = ar.FullName,
                PhoneNumber = ar.PhoneNumber,
                StreetAddress = ar.StreetAddress,
                Ward = ar.Ward,
                District = ar.District,
                City = ar.City,
                IsDefault = ar.IsDefault,
                User = user
            };
        }
    }
}
