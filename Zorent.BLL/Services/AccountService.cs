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
                    decimal P = dto.InitialDeposit;

                    int N = dto.TenureMonths.Value;

                    decimal R = interestRate;

                    decimal maturity =
                        P * N +
                        (
                            P * N * (N + 1) * R
                        )
                        /
                        (2 * 12 * 100);

                    maturityAmount = maturity;

                    break;
            }

            // MINIMUM BALANCE VALIDATION
            if (dto.InitialDeposit < minBalance)
            {
                return Fail(
                    $"Minimum deposit required is ₹{minBalance}"
                );
            }

            // MAX 5 ACTIVE ACCOUNTS ONLY
            int activeAccountsCount =
                await _context.Accounts
                    .CountAsync(a =>
                        a.UserId == userId
                        &&
                        a.Status != "Closed"
                    );

            if (activeAccountsCount >= 5)
            {
                return Fail(
                    "Maximum 5 active accounts allowed"
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
        : null,


                MonthlyInstallment =
    dto.Type == "Recurring Deposit"
        ? dto.InitialDeposit
        : 0

            };

            _context.Accounts.Add(account);

            // CREATE INITIAL TRANSACTION
            // CREATE INITIAL TRANSACTION
            var transaction = new Transaction
            {
                Account = account,

                Type = "Credit",

                Amount = dto.InitialDeposit,

                Description = "Initial Deposit",

                BalanceAfter = dto.InitialDeposit,

                CreatedAt = DateTime.Now
            };

            _context.Transactions.Add(transaction);

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

                        remainingInstallments = account.RemainingInstallments,

                        // ADD THIS
                        transactionId = transaction.TransactionId
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

                    remainingInstallments = account.RemainingInstallments,

                    // ADD THIS
                    transactionId = transaction.TransactionId
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

                    CreatedAt = a.CreatedAt,
                    ClosedAt = a.ClosedAt
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

            // DO NOT MODIFY CLOSED ACCOUNTS
            if (account.Status == "Closed")
            {
                return;
            }
           
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
        public async Task<ApiResponse<object>> CloseDeposit(
    CloseDepositDto dto,
    int userId)
        {
            var depositAccount = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == dto.DepositAccountNumber
                    &&
                    a.UserId == userId);

            if (depositAccount == null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Deposit account not found"
                };
            }

            // ALREADY CLOSED
            if (depositAccount.Status == "Closed")
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Account already closed"
                };
            }

            // ONLY FD / RD
            if (
                depositAccount.AccountType != "Fixed Deposit"
                &&
                depositAccount.AccountType != "Recurring Deposit"
                  &&
               depositAccount.AccountType != "Checking"
            )
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Only FD and FD and Checkings accounts can be closed"
                };
            }

            // TARGET ACCOUNT
            var targetAccount = await _context.Accounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == dto.TargetAccountNumber
                    &&
                    a.UserId == userId);

            if (targetAccount == null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Cannot transfer to other User"
                };
            }

            // ONLY SAVINGS / CHECKING
            if (
                targetAccount.AccountType != "Savings"
                &&
                targetAccount.AccountType != "Checking"
            )
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message =
                        "Money can only be transferred to Savings or Checking account"
                };
            }

            // SAME ACCOUNT BLOCK
            if (
                depositAccount.AccountNumber ==
                targetAccount.AccountNumber
            )
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Cannot transfer to same account"
                };
            }

            decimal payoutAmount = 0;
            decimal principalAmount = 0;
            decimal earnedInterest = 0;

            // DAYS COMPLETED
            int daysCompleted =
                (DateTime.Now - depositAccount.CreatedAt).Days;

            if (daysCompleted < 0)
            {
                daysCompleted = 0;
            }

            // =========================
            // SAVINGS / CHECKING
            // =========================
            if (
                depositAccount.AccountType == "Savings"
                ||
                depositAccount.AccountType == "Checking"
            )
            {
                principalAmount =
                    depositAccount.Balance;

                earnedInterest = 0;

                payoutAmount =
                    depositAccount.Balance;
            }

            // =========================
            // FIXED DEPOSIT
            // =========================
            else if (
                depositAccount.AccountType ==
                "Fixed Deposit"
            )
            {
                decimal principal =
                    depositAccount.Balance;

                decimal rate =
                    depositAccount.InterestRate;

                int tenureMonths =
                    depositAccount.TenureMonths ?? 12;

                int totalDays =
                    tenureMonths * 30;

                principalAmount = principal;

                if (daysCompleted >= totalDays)
                {
                    payoutAmount =
                        depositAccount.MaturityAmount
                        ?? principal;

                    earnedInterest =
                        payoutAmount - principal;
                }
                else
                {
                    earnedInterest =
                        (
                            principal
                            * rate
                            * daysCompleted
                        )
                        / (365 * 100);

                    payoutAmount =
                        principal + earnedInterest;
                }
            }

            // =========================
            // RECURRING DEPOSIT
            // =========================
            else if (
    depositAccount.AccountType ==
    "Recurring Deposit"
)
            {
                decimal rate =
                    depositAccount.InterestRate;

                principalAmount =
                    depositAccount.Balance;

                earnedInterest = 0;

                var installments = await _context.Transactions
                    .Where(t =>
                        t.AccountId == depositAccount.Id
                        &&
                        t.Type == "Credit"
                    )
                    .OrderBy(t => t.CreatedAt)
                    .ToListAsync();

                for (int i = 0; i < installments.Count; i++)
                {
                    var installment = installments[i];

                    // CHECK IF NEXT INSTALLMENT EXISTS
                    bool hasNext = (i < installments.Count - 1);

                    if (!hasNext)
                        continue;

                    // FIXED 1 MONTH INTEREST ONLY
                    decimal interest =
                        (installment.Amount * rate * 1)
                        / (12 * 100);

                    earnedInterest += interest;
                }

                payoutAmount =
                    principalAmount + earnedInterest;
            }
        
        

            // ROUNDING
            payoutAmount =
                Math.Round(payoutAmount, 2);

            earnedInterest =
                Math.Round(earnedInterest, 2);

            principalAmount =
                Math.Round(principalAmount, 2);

            // TRANSFER MONEY
            targetAccount.Balance += payoutAmount;

            targetAccount.LastTransactionDate =
                DateTime.Now;

            // CLOSE ACCOUNT
            depositAccount.Status = "Closed";

            depositAccount.ClosedAt =
                DateTime.Now;

            // STORE FINAL PAYOUT
            depositAccount.Balance = 0;

            // TRANSACTION ENTRY
            // TRANSACTION ENTRY FOR TARGET ACCOUNT
            var transaction = new Transaction
            {
                AccountId = targetAccount.Id,

                Type = "Credit",

                Amount = payoutAmount,

                Description =
                    $"{depositAccount.AccountType} Closure Amount Received",

                BalanceAfter = targetAccount.Balance,

                CreatedAt = DateTime.Now,

                FromAccountNumber =
                    depositAccount.AccountNumber,

                ToAccountNumber =
                    targetAccount.AccountNumber
            };

            _context.Transactions.Add(transaction);


            // ADD THIS FOR CLOSED ACCOUNT STATEMENT
            var closureTransaction = new Transaction
            {
                AccountId = depositAccount.Id,

                Type = "Debit",

                Amount = payoutAmount,

                Description =
                    $"{depositAccount.AccountType} Closed and transferred to {targetAccount.AccountNumber}",

                BalanceAfter = 0,

                CreatedAt = DateTime.Now,

                FromAccountNumber =
                    depositAccount.AccountNumber,

                ToAccountNumber =
                    targetAccount.AccountNumber
            };

            _context.Transactions.Add(closureTransaction);

           

            await _context.SaveChangesAsync();

            return new ApiResponse<object>
            {
                Success = true,
                Message = "Deposit closed successfully",

                Data = new
                {
                    transactionId =
                        transaction.TransactionId,

                    principal =
                        principalAmount,

                    earnedInterest =
                        earnedInterest,

                    amount =
                        payoutAmount,

                    targetAccount =
                        targetAccount.AccountNumber,

                    closedAt =
                        depositAccount.ClosedAt
                }
            };
        }

        public async Task<ApiResponse<object>> DepositMoney(
    DepositDto dto,
    int userId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == dto.AccountNumber
                    &&
                    a.UserId == userId);

            if (account == null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Account not found"
                };
            }
            // CLOSED ACCOUNT VALIDATION
            if (account.Status == "Closed")
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Cannot perform transactions on closed account"
                };
            }

            // ONLY SAVINGS / CHECKING / RD
            if (
                account.AccountType == "Fixed Deposit"
            )
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Deposit not allowed for FD"
                };
            }

            if (dto.Amount <= 0)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Amount must be greater than 0"
                };
            }
            if (dto.Amount < 100)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Minimum deposit amount is ₹100"
                };
            }

            if (dto.Amount > 100000)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Maximum deposit amount is ₹100000"
                };
            }

            // UPDATE BALANCE
            account.Balance += dto.Amount;

            account.LastTransactionDate =
                DateTime.Now;

            // RD INSTALLMENT UPDATE
            if (
                account.AccountType ==
                "Recurring Deposit"
            )
            {
                account.PaidInstallments += 1;

                account.RemainingInstallments -= 1;
            }

            // TRANSACTION
            var transaction = new Transaction
            {
                AccountId = account.Id,

                Type = "Credit",

                Amount = dto.Amount,

                Description = "Cash Deposit",

                BalanceAfter = account.Balance,

                CreatedAt = DateTime.Now,

                ToAccountNumber =
                    account.AccountNumber
            };

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return new ApiResponse<object>
            {
                Success = true,
                Message = "Deposit successful",

                Data = new
                {
                    transactionId =
                        transaction.TransactionId,

                    amount =
                        dto.Amount,

                    balance =
                        account.Balance
                }
            };
        }

        public async Task<ApiResponse<object>> GetAccountByNumber(
    string accountNumber,
    int userId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == accountNumber
                    &&
                    a.UserId == userId);

            if (account == null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Account not found"
                };
            }
            // CLOSED ACCOUNT VALIDATION
            if (account.Status == "Closed")
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "This account is closed"
                };
            }

            return new ApiResponse<object>
            {
                Success = true,

                Data = new
                {
                    accountNumber = account.AccountNumber,

                    accountType = account.AccountType,

                    availableBalance = account.Balance,

                    status = account.Status,

                    createdAt = account.CreatedAt,

                    interestRate = account.InterestRate,

                    maturityAmount = account.MaturityAmount,

                    tenureMonths = account.TenureMonths
                }
            };
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