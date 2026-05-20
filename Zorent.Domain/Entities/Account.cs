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

        public decimal InterestRate { get; set; }

        public int? TenureMonths { get; set; }

        public DateTime? MaturityDate { get; set; }

        public decimal? MaturityAmount { get; set; }

        public decimal MinimumBalance { get; set; }

        public DateTime? LowBalanceSince { get; set; }
        public DateTime LastTransactionDate { get; set; } = DateTime.Now;

        // ✅ FIXED

        public DateTime? InstallmentDate { get; set; }

        public int? TotalInstallments { get; set; }

        public int PaidInstallments { get; set; } = 0;

        public int? RemainingInstallments { get; set; }

        public decimal MonthlyInstallment { get; set; } = 0;

        public DateTime? ClosedAt { get; set; }
        public User User { get; set; } = default!;

        // ✅ FIXED
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}