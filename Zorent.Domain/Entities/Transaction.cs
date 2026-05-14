using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        public string TransactionId { get; set; } =
            GenerateTransactionId();

        public int AccountId { get; set; }

        // ✅ FIXED

        public string? FromAccountNumber { get; set; }

        public string? ToAccountNumber { get; set; }
        public Account Account { get; set; } = default!;

        public string Type { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public decimal BalanceAfter { get; set; }
        private static string GenerateTransactionId()
        {
            Random random =
                new Random(Guid.NewGuid().GetHashCode());

            string transactionId = "";

            for (int i = 0; i < 12; i++)
            {
                transactionId += random.Next(0, 10).ToString();
            }

            return transactionId;
        }
    }
}
