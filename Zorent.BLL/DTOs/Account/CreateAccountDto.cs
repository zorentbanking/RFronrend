using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.BLL.DTOs.Account
{
    public class CreateAccountDto
    {
        public string Type { get; set; } = string.Empty;

        public decimal InitialDeposit { get; set; }

       

        

        public int? TenureMonths { get; set; }
    }
}
