using System;
using System.Collections.Generic;
using System.Text;
using Zorent.BLL.DTOs.Transaction;
using Zorent.Common.Responses;

namespace Zorent.BLL.Interfaces
{
    public interface ITransactionService
    {
        Task<ApiResponse> Transfer(TransferDto dto);

        Task<ApiResponse<object>> GetTransactions(
            int accountId,
            int page = 1,
            string? sortBy = "date",
            string? order = "desc");

      
        Task<ApiResponse<object>> Search(TransactionSearchDto filter);

        Task<byte[]> ExportToCsv(TransactionSearchDto filter);
    }
}
