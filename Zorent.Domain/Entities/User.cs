using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zorent.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        [Column(TypeName="date")]
        public DateTime DOB { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public int FailedAttempts { get; set; }
        public bool IsLocked { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }


        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
