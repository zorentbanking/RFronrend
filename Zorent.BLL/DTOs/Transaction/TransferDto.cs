using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.BLL.DTOs.Transaction
{
    public class TransferDto
    {
        public int SourceId { get; set; }

        public string DestinationAccount { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }
}
