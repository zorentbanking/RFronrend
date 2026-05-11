using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.BLL.DTOs.Transaction
{
    public class TransactionDto
    {
        public string TransactionId { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public DateTime Date { get; set; }

        public decimal BalanceAfter { get; set; }
    }
}
