using System;
using System.Collections.Generic;

namespace Zorent.BLL.DTOs.Transaction
{
    public class StatementDto
    {
        // ACCOUNT DETAILS
        public string CustomerName { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public string AccountNumber { get; set; } = string.Empty;

        public string AccountType { get; set; } = string.Empty;

        public decimal AvailableBalance { get; set; }

        // FD / RD
        public decimal InterestRate { get; set; }

        public int? DurationMonths { get; set; }

        public DateTime? MaturityDate { get; set; }

        public decimal? MaturityAmount { get; set; }

        // RD ONLY
        public DateTime? InstallmentDate { get; set; }

        public int? TotalInstallments { get; set; }

        public int PaidInstallments { get; set; }

        public int? RemainingInstallments { get; set; }

        // TRANSACTIONS
        public List<StatementTransactionDto> Transactions { get; set; }
            = new();
    }

    public class StatementTransactionDto
    {
        public string TransactionId { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public decimal Balance { get; set; }

        public string FromAccount { get; set; } = "-";

        public string ToAccount { get; set; } = "-";

        public decimal InterestEarned { get; set; }
    }
}