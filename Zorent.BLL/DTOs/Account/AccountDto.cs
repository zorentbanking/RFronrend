using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.BLL.DTOs.Account
{
    public class AccountDto
    {
        public int Id { get; set; }

        public string AccountNumber { get; set; } = string.Empty;

        public string AccountType { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
