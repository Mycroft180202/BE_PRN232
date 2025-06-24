using System;
using System.Collections.Generic;

namespace BE_PRN232.Entities;

public partial class Address
{
    public int AddressId { get; set; }

    public Guid UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string StreetAddress { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;

    public bool IsDefault { get; set; }

    public virtual User User { get; set; } = null!;
}
