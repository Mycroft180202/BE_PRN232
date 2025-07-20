using BE_PRN232.Entities;

namespace BE_PRN232.ResponseDTO
{
    public class UserAddressDTO
    {
        public int AddressId { get; set; }
        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string StreetAddress { get; set; } = null!;

        public string Ward { get; set; } = null!;

        public string District { get; set; } = null!;

        public string City { get; set; } = null!;

        public bool IsDefault { get; set; }


        public static Address FromDTOToEntity(UserAddressDTO uar, User user)
        {
            return new Address
            {
                AddressId = uar.AddressId,
                FullName = uar.FullName,
                PhoneNumber = uar.PhoneNumber,
                StreetAddress = uar.StreetAddress,
                Ward = uar.Ward,
                District = uar.District,
                City = uar.City,
                IsDefault = uar.IsDefault,
                User = user
            };
        }
        public static UserAddressDTO FromEntityToDTO(Address address)
        {
            return new UserAddressDTO
            {
                AddressId = address.AddressId,
                FullName = address.FullName,
                PhoneNumber = address.PhoneNumber,
                StreetAddress = address.StreetAddress,
                Ward = address.Ward,
                District = address.District,
                City = address.City,
                IsDefault = address.IsDefault
            };
        }
        public static void UpdateAddress(Address address, UserAddressDTO uar)
        {
            address.AddressId = uar.AddressId;
            address.FullName = uar.FullName;
            address.PhoneNumber = uar.PhoneNumber;
            address.StreetAddress = uar.StreetAddress;
            address.Ward = uar.Ward;
            address.District = uar.District;
            address.City = uar.City;
            address.IsDefault = uar.IsDefault;
        }
    }
}
