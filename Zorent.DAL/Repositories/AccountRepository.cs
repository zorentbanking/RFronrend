using System;
using System.Collections.Generic;
using System.Text;
using Zorent.DAL.Data;
using Zorent.DAL.Interfaces;
using Zorent.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Zorent.DAL.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountByUser(int userId)
            => await _context.Accounts.CountAsync(a => a.UserId == userId);

        public async Task Add(Account account)
            => await _context.Accounts.AddAsync(account);

        public async Task<bool> AccountNumberExists(string accNo)
            => await _context.Accounts.AnyAsync(a => a.AccountNumber == accNo);

        public async Task<List<Account>> GetByUser(int userId)
            => await _context.Accounts.Where(a => a.UserId == userId).ToListAsync();

        public async Task<Account?> GetById(int id)
            => await _context.Accounts.FindAsync(id);

        public async Task<Account?> GetByAccountNumber(string accNo)
            => await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accNo);

        public async Task Save()
            => await _context.SaveChangesAsync();
    }
}
