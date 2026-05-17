using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.BLL.DTOs.Account
{
    public class CloseDepositDto
    {
        public string DepositAccountNumber { get; set; } = string.Empty;

        public string TargetAccountNumber { get; set; } = string.Empty;


    }
}