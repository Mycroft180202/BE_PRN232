using System;
using System.Collections.Generic;

namespace BE_PRN232.Entities;

public partial class UserRefreshToken
{
    public int TokenId { get; set; }

    public Guid UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
