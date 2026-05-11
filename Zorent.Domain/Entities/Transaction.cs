using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        public string TransactionId { get; set; } = Guid.NewGuid().ToString();

        public int AccountId { get; set; }

        // ✅ FIXED
        public Account Account { get; set; } = default!;

        public string Type { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public decimal BalanceAfter { get; set; }
    }
}
