using System;
using System.Collections.Generic;
using System.Text;
using Zorent.Domain.Entities;

namespace Zorent.DAL.Interfaces
{
    public interface ITransactionRepository
    {
        Task Add(Transaction t);
        Task<List<Transaction>> GetByAccount(int accountId, int page);
        IQueryable<Transaction> Query();
        Task Save();
    }
}
