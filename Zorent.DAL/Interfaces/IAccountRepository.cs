using System;
using System.Collections.Generic;
using System.Text;
using Zorent.Domain.Entities;

namespace Zorent.DAL.Interfaces
{
    public interface IAccountRepository
    {
        Task<int> CountByUser(int userId);
        Task Add(Account account);
        Task<bool> AccountNumberExists(string accNo);
        Task<List<Account>> GetByUser(int userId);
        Task<Account?> GetById(int id);
        Task<Account?> GetByAccountNumber(string accNo);
        Task Save();
    }
}
