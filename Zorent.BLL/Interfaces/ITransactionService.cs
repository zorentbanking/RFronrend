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


        Task<ApiResponse<object>> Search(
     TransactionSearchDto f,
     int userId
 );

        Task<ApiResponse<object>> GetStatement(
     string accountNumber);

        Task<byte[]> ExportToCsv(TransactionSearchDto f, int userId);
    }
}
