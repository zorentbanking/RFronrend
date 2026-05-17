using System;
using System.Collections.Generic;
using System.Text;
using Zorent.BLL.DTOs.Account;
using Zorent.Common.Responses;

namespace Zorent.BLL.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse> CreateAccount(CreateAccountDto dto, int userId);
        Task<ApiResponse<List<AccountDto>>> GetUserAccounts(int userId);
        Task<ApiResponse<object>> DepositMoney(DepositDto dto, int userId);
        Task<ApiResponse<object>> CloseDeposit(
    CloseDepositDto dto,
    int userId);
        Task<ApiResponse<object>> GetAccountByNumber(
    string accountNumber,
    int userId);
    }
}
