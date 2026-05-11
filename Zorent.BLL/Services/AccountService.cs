using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Zorent.BLL.DTOs.Account;
using Zorent.BLL.Interfaces;
using Zorent.Common.Responses;
using Zorent.DAL.Data;
using Zorent.Domain.Entities;

namespace Zorent.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> CreateAccount(CreateAccountDto dto, int userId)
        {
           
            var validTypes = new[] { "Savings", "Checking", "Fixed Deposit", "Recurring Deposit" };

            if (!validTypes.Contains(dto.Type))
                return Fail("Invalid account type");

            
            if (dto.InitialDeposit <= 0)
                return Fail("Amount must be greater than 0");

            
            decimal min = dto.Type switch
            {
                "Savings" => 500,
                "Checking" => 100,
                "Fixed Deposit" => 1000,
                "Recurring Deposit" => 100,
                _ => 0
            };

            if (dto.InitialDeposit < min)
                return Fail($"Minimum balance is {min}");

            
            if (await _context.Accounts.CountAsync(a => a.UserId == userId) >= 5)
                return Fail("Max 5 accounts allowed");

            
            string accNo;
            var rnd = new Random();

            do
            {
                accNo = rnd.Next(1000000000, 1999999999).ToString();
            }
            while (await _context.Accounts.AnyAsync(a => a.AccountNumber == accNo));

            
            var account = new Account
            {
                AccountNumber = accNo,
                AccountType = dto.Type,
                Balance = dto.InitialDeposit,
                UserId = userId,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            _context.Accounts.Add(account);

            
            _context.Transactions.Add(new Transaction
            {
                Account = account,
                Type = "Credit",
                Amount = dto.InitialDeposit,
                Description = "Initial Deposit",
                BalanceAfter = dto.InitialDeposit,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Success("Account created successfully");
        }

        public async Task<ApiResponse<List<AccountDto>>> GetUserAccounts(int userId)
        {
            var data = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new AccountDto
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    AccountType = a.AccountType,
                    Balance = a.Balance,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                }).ToListAsync();

            return new ApiResponse<List<AccountDto>> { Success = true, Data = data };
        }

        private ApiResponse Fail(string msg) => new() { Success = false, Message = msg };
        private ApiResponse Success(string msg) => new() { Success = true, Message = msg };
    }
}
