using System;
using System.Collections.Generic;

namespace BE_PRN232.Entities;

public partial class User
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string PasswordHash { get; set; } = null!;

    public bool EmailVerified { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual Cart? Cart { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<UserRefreshToken> UserRefreshTokens { get; set; } = new List<UserRefreshToken>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = new HashSet<EmailVerificationToken>();
}
