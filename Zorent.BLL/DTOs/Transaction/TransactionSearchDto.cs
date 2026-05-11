using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.BLL.DTOs.Transaction
{
    public class TransactionSearchDto
    {
        public int? AccountId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string? Type { get; set; } 

        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        public string? Keyword { get; set; }

        
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        
        public string? SortBy { get; set; } = "date"; 
        public string? SortOrder { get; set; } = "desc"; 
    }
}
