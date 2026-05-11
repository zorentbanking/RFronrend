using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.Domain.Entities
{
    public class Account
    {
        public int Id { get; set; }

        public string AccountNumber { get; set; } = string.Empty;

        public string AccountType { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int UserId { get; set; }

        // ✅ FIXED
        public User User { get; set; } = default!;

        // ✅ FIXED
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}