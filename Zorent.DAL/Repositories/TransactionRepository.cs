using System;
using System.Collections.Generic;
using System.Text;
using Zorent.DAL.Data;
using Zorent.DAL.Interfaces;
using Zorent.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Zorent.DAL.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Transaction t)
            => await _context.Transactions.AddAsync(t);

        public async Task<List<Transaction>> GetByAccount(int accountId, int page)
            => await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * 20)
                .Take(20)
                .ToListAsync();

        public IQueryable<Transaction> Query()
            => _context.Transactions.AsQueryable();

        public async Task Save()
            => await _context.SaveChangesAsync();
    }
}
