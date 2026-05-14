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

        public async Task<ApiResponse> CreateAccount(
            CreateAccountDto dto,
            int userId)
        {
            var validTypes = new[]
            {
                "Savings",
                "Checking",
                "Fixed Deposit",
                "Recurring Deposit"
            };

            if (!validTypes.Contains(dto.Type))
                return Fail("Invalid account type");

            if (dto.InitialDeposit <= 0)
                return Fail("Amount must be greater than 0");

            decimal minBalance = 0;
            decimal interestRate = 0;
            decimal maturityAmount = 0;

            DateTime? maturityDate = null;

            switch (dto.Type)
            {
                // SAVINGS
                case "Savings":

                    minBalance = 500;
                    interestRate = 3.5m;

                    break;

                // CHECKING
                case "Checking":

                    minBalance = 100;
                    interestRate = 0;

                    break;

                // FIXED DEPOSIT
                case "Fixed Deposit":

                    minBalance = 1000;
                    interestRate = 7.5m;

                    // VALIDATE TENURE
                    if (
                        dto.TenureMonths == null ||
                        dto.TenureMonths < 6
                    )
                    {
                        return Fail(
                            "Fixed Deposit tenure must be minimum 6 months"
                        );
                    }

                    // MATURITY DATE
                    maturityDate =
                        DateTime.Now.AddMonths(
                            dto.TenureMonths.Value
                        );

                    // SIMPLE INTEREST FD CALCULATION
                    maturityAmount =
                        dto.InitialDeposit +
                        (
                            dto.InitialDeposit *
                            interestRate *
                            dto.TenureMonths.Value
                            / 12
                            / 100
                        );

                    break;

                // RECURRING DEPOSIT
                case "Recurring Deposit":

                    minBalance = 100;
                    interestRate = 6.5m;

                    // VALIDATE TENURE
                    if (
                        dto.TenureMonths == null ||
                        dto.TenureMonths < 6
                    )
                    {
                        return Fail(
                            "Recurring Deposit tenure must be minimum 6 months"
                        );
                    }

                    // MATURITY DATE
                    maturityDate =
                        DateTime.Now.AddMonths(
                            dto.TenureMonths.Value
                        );

                    // RD TOTAL INVESTMENT
                    decimal totalInvestment =
                        dto.InitialDeposit *
                        dto.TenureMonths.Value;

                    // RD INTEREST
                    decimal rdInterest =
                        (
                            totalInvestment *
                            interestRate *
                            dto.TenureMonths.Value
                        )
                        / (12 * 100);

                    // RD MATURITY
                    maturityAmount =
                        totalInvestment + rdInterest;

                    break;
            }

            // MINIMUM BALANCE VALIDATION
            if (dto.InitialDeposit < minBalance)
            {
                return Fail(
                    $"Minimum deposit required is ₹{minBalance}"
                );
            }

            // MAX 5 ACCOUNTS
            if (
                await _context.Accounts
                    .CountAsync(a => a.UserId == userId) >= 5
            )
            {
                return Fail(
                    "Maximum 5 accounts allowed"
                );
            }

            // GENERATE UNIQUE ACCOUNT NUMBER
            string accNo;

            var rnd = new Random();

            do
            {
                accNo = rnd
                    .Next(1000000000, 1999999999)
                    .ToString();

            }
            while (
                await _context.Accounts
                    .AnyAsync(a =>
                        a.AccountNumber == accNo)
            );

            // CREATE ACCOUNT
            var account = new Account
            {
                AccountNumber = accNo,

                AccountType = dto.Type,

                Balance = dto.InitialDeposit,

                UserId = userId,

                Status = "Active",

                CreatedAt = DateTime.Now,

                // NEW FD/RD LOGIC
                MinimumBalance = minBalance,

                InterestRate = interestRate,

                TenureMonths = dto.TenureMonths,

                MaturityDate = maturityDate,

                MaturityAmount = maturityAmount,
                InstallmentDate =
    dto.Type == "Recurring Deposit"
        ? dto.InstallmentDate
        : null,

                TotalInstallments =
    dto.Type == "Recurring Deposit"
        ? dto.TenureMonths
        : null,

                PaidInstallments =
    dto.Type == "Recurring Deposit"
        ? 0
        : 0,

                RemainingInstallments =
    dto.Type == "Recurring Deposit"
        ? dto.TenureMonths
        : null
            };

            _context.Accounts.Add(account);

            // CREATE INITIAL TRANSACTION
            _context.Transactions.Add(
                new Transaction
                {
                    Account = account,

                    Type = "Credit",

                    Amount = dto.InitialDeposit,

                    Description = "Initial Deposit",

                    BalanceAfter = dto.InitialDeposit,

                    CreatedAt = DateTime.Now
                });

            await _context.SaveChangesAsync();

            // SUCCESS MESSAGE
            if (
                dto.Type == "Fixed Deposit" ||
                dto.Type == "Recurring Deposit"
            )
            {
                return new ApiResponse
                {
                    Success = true,
                    Message = "Account created successfully",

                    Data = new
                    {
                        accountNumber = account.AccountNumber,

                        accountType = account.AccountType,

                        balance = account.Balance,

                        createdAt = account.CreatedAt,

                        interestRate = account.InterestRate,

                        tenureMonths = account.TenureMonths,

                        maturityAmount = account.MaturityAmount,

                        maturityDate = account.MaturityDate,

                        installmentDate = account.InstallmentDate,

                        totalInstallments = account.TotalInstallments,

                        paidInstallments = account.PaidInstallments,

                        remainingInstallments = account.RemainingInstallments
                    }
                };
            }

            return new ApiResponse
            {
                Success = true,
                Message = "Account created successfully",

                Data = new
                {
                    accountNumber = account.AccountNumber,

                    accountType = account.AccountType,

                    balance = account.Balance,

                    createdAt = account.CreatedAt,

                    interestRate = account.InterestRate,

                    tenureMonths = account.TenureMonths,

                    maturityAmount = account.MaturityAmount,

                    maturityDate = account.MaturityDate,

                    installmentDate = account.InstallmentDate,

                    totalInstallments = account.TotalInstallments,

                    paidInstallments = account.PaidInstallments,

                    remainingInstallments = account.RemainingInstallments
                }
            };
        }

        public async Task<ApiResponse<List<AccountDto>>> GetUserAccounts(
            int userId)
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
                })
                .ToListAsync();

            return new ApiResponse<List<AccountDto>>
            {
                Success = true,
                Data = data
            };
        }

        public async Task CheckAccountStatus(int accountId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
                return;

            // GET LAST TRANSACTION
            var lastTransaction = await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastTransaction == null)
                return;

            // 2 DAYS INACTIVE
            if (
                lastTransaction.CreatedAt
                < DateTime.Now.AddDays(-2)
            )
            {
                account.Status = "Inactive";
            }
            else
            {
                account.Status = "Active";
            }

            await _context.SaveChangesAsync();
        }

        private ApiResponse Fail(string msg) =>
            new()
            {
                Success = false,
                Message = msg
            };

        private ApiResponse Success(string msg) =>
            new()
            {
                Success = true,
                Message = msg
            };
    }
}