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
    }
}
