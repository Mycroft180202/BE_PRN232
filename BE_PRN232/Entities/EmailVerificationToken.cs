using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE_PRN232.Entities
{
    [Table("EmailVerificationToken")]
    public class EmailVerificationToken
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiredAt { get; set; }
        public string Purpose { get; set; }
        public virtual User User { get; set; }
    }
}
