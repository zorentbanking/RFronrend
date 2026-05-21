using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Zorent.BLL.DTOs.Transaction;
using Zorent.BLL.Interfaces;
using Zorent.Common.Responses;
using Zorent.DAL.Data;
using Zorent.Domain.Entities;

namespace Zorent.BLL.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
       

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
            
        }

        public async Task<ApiResponse> Transfer(TransferDto dto)
        {

            var source = await _context.Accounts.FindAsync(dto.SourceId);
            var dest = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == dto.DestinationAccount);

            // AUTO INACTIVE CHECK
            if (
                source.LastTransactionDate
                < DateTime.Now.AddDays(-2)
            )
            {
                source.Status = "Inactive";

                await _context.SaveChangesAsync();
            }

            if (source.Id == dest.Id)
                return Fail("Cannot transfer to same account");
            if (source.AccountType == "Fixed Deposit")
            {
                return Fail(
                    "Transactions are not allowed from Fixed Deposit accounts"
                );
            }
            if (dest.Status != "Active")
                return Fail("Receivers Account is Closed");

            if (dest.AccountType == "Fixed Deposit")
            {
                return Fail("Cannot Transfer to Fixed Deposit");
            }

            if (dest.AccountType == "Recurring Deposit")
            {
                return Fail("Cannot Transfer to Recurring Deposit");
            }


            if (dest.Status != "Active")
                return Fail("Receivers Account is Closed");

            if (source.Status != "Active")
            {
                return Fail(
                    "Your account is inactive. Please deposit money to activate it."
                );
            }


            if (dto.Amount <= 0 || dto.Amount > source.Balance)
                return Fail("Invalid amount");

            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                source.Balance -= dto.Amount;
                dest.Balance += dto.Amount;

                source.LastTransactionDate = DateTime.Now;

                dest.LastTransactionDate = DateTime.Now;


                var debitTransaction = new Transaction
                {
                    AccountId = source.Id,

                   

                    Type = "Debit",

                    Amount = dto.Amount,

                    Description = dto.Description,

                    BalanceAfter = source.Balance,

                    CreatedAt = DateTime.Now,

                    FromAccountNumber = source.AccountNumber,

                    ToAccountNumber = dest.AccountNumber
                };

                var creditTransaction = new Transaction
                {
                    AccountId = dest.Id,

                    

                    Type = "Credit",

                    Amount = dto.Amount,

                    Description = dto.Description,

                    BalanceAfter = dest.Balance,

                    CreatedAt = DateTime.Now,

                    FromAccountNumber = source.AccountNumber,

                    ToAccountNumber = dest.AccountNumber
                };

                _context.Transactions.Add(debitTransaction);

                _context.Transactions.Add(creditTransaction);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return new ApiResponse
                {
                    Success = true,
                    Message = "Transfer successful",
                    Data = new
                    {
                        transactionId = debitTransaction.TransactionId
                    }
                };
            }
            catch
            {
                await tx.RollbackAsync();
                return Fail("Transfer failed");
            }
        }

        public async Task<ApiResponse<object>> GetTransactions(
    int accountId,
    int page = 1,
    string? sortBy = "date",
    string? order = "desc")
        {
           
            if (page < 1) page = 1;

            sortBy = sortBy?.ToLower();
            order = order?.ToLower();

            var query = _context.Transactions
                .Where(t => t.AccountId == accountId &&
                            t.CreatedAt >= DateTime.Now.AddDays(-30));

            //  SORTING
            if (sortBy == "amount")
            {
                query = order == "asc"
                    ? query.OrderBy(t => t.Amount)
                    : query.OrderByDescending(t => t.Amount);
            }
            else
            {
                query = order == "asc"
                    ? query.OrderBy(t => t.CreatedAt)
                    : query.OrderByDescending(t => t.CreatedAt);
            }

            //  TOTAL COUNT (NEW)
            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * 20)
                .Take(20)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    Type = t.Type,
                    Amount = t.Amount,
                    Description = t.Description,
                    Date = t.CreatedAt,
                    BalanceAfter = t.BalanceAfter
                })
                .ToListAsync();

            return new ApiResponse<object>
            {
                Success = true,
                Data = new
                {
                    TotalRecords = totalRecords,
                    Page = page,
                    PageSize = 20,
                    Data = data
                }
            };
        }

        public async Task<ApiResponse<object>> Search(
     TransactionSearchDto f,
     int userId)
        {
            // GET ONLY LOGGED-IN USER ACCOUNTS
            var userAccountIds = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .ToListAsync();

            // ONLY USER TRANSACTIONS
            var query = _context.Transactions
                .Where(x => userAccountIds.Contains(x.AccountId));

            // FILTERS
            if (f.AccountId != null)
                query = query.Where(x => x.AccountId == f.AccountId);

            if (f.FromDate != null)
                query = query.Where(x => x.CreatedAt >= f.FromDate);

            if (f.ToDate != null)
                query = query.Where(x => x.CreatedAt <= f.ToDate);

            if (!string.IsNullOrEmpty(f.Type))
                query = query.Where(x => x.Type == f.Type);

            if (f.MinAmount != null)
                query = query.Where(x => x.Amount >= f.MinAmount);

            if (f.MaxAmount != null)
                query = query.Where(x => x.Amount <= f.MaxAmount);

            if (!string.IsNullOrEmpty(f.Keyword))
                query = query.Where(x =>
                    x.Description.Contains(f.Keyword));

            // TOTAL
            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((f.Page - 1) * f.PageSize)
                .Take(f.PageSize)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    Type = t.Type,
                    Amount = t.Amount,
                    Description = t.Description,
                    Date = t.CreatedAt,
                    BalanceAfter = t.BalanceAfter
                })
                .ToListAsync();

            return new ApiResponse<object>
            {
                Success = true,
                Data = new
                {
                    TotalRecords = total,
                    Page = f.Page,
                    PageSize = f.PageSize,
                    Data = data
                }
            };
        }
        public async Task<byte[]> ExportToCsv(
     TransactionSearchDto f,
     int userId
 )
        {
            var userAccountIds = await _context.Accounts
     .Where(a => a.UserId == userId)
     .Select(a => a.Id)
     .ToListAsync();

            var query = _context.Transactions
                .Where(x => userAccountIds.Contains(x.AccountId));

            // same filters (copy paste)
            if (f.AccountId != null)
                query = query.Where(x => x.AccountId == f.AccountId);

            if (f.FromDate != null)
                query = query.Where(x => x.CreatedAt >= f.FromDate);

            if (f.ToDate != null)
                query = query.Where(x => x.CreatedAt <= f.ToDate);

            if (!string.IsNullOrEmpty(f.Type))
                query = query.Where(x => x.Type == f.Type);

            if (f.MinAmount != null)
                query = query.Where(x => x.Amount >= f.MinAmount);

            if (f.MaxAmount != null)
                query = query.Where(x => x.Amount <= f.MaxAmount);

            if (!string.IsNullOrEmpty(f.Keyword))
                query = query.Where(x => x.Description.Contains(f.Keyword));

            var data = await query.ToListAsync();

            var sb = new StringBuilder();

            // Header
            sb.AppendLine("TransactionId,Type,Amount,Description,Date,Closed Balance");

            foreach (var t in data)
            {
                sb.AppendLine($"{t.TransactionId},{t.Type},{t.Amount},{t.Description},{t.CreatedAt},{t.BalanceAfter}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }


        public async Task<ApiResponse<object>> GetStatement(
    string accountNumber)
        {
            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == accountNumber);

            if (account == null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Account not found"
                };
            }

            var transactions = await _context.Transactions
     .Where(t =>
         t.AccountId == account.Id)
     .OrderByDescending(t => t.CreatedAt)
     .Select(t => new StatementTransactionDto
     {
         TransactionId = t.TransactionId,

         Type = t.Type,

         Amount = t.Amount,

         Description = t.Description ?? "",

         Date = t.CreatedAt,

         Balance = t.BalanceAfter,

         FromAccount =
             t.FromAccountNumber ?? "-",

         ToAccount =
             t.ToAccountNumber ?? "-",

         InterestEarned = 0
     })
     .ToListAsync();

            var data = new StatementDto
            {
                CreatedAt = account.CreatedAt,
                CustomerName = account.User.FullName,

                CustomerId = account.UserId,

                AccountNumber = account.AccountNumber,

                AccountType = account.AccountType,

                AvailableBalance = account.Balance,

                InterestRate = account.InterestRate,

                DurationMonths = account.TenureMonths,

                MaturityDate = account.MaturityDate,

                MaturityAmount = account.MaturityAmount,

                InstallmentDate = account.InstallmentDate,

                TotalInstallments = account.TotalInstallments,

                PaidInstallments = account.PaidInstallments,

                RemainingInstallments = account.RemainingInstallments,

                Transactions = transactions
            };

            return new ApiResponse<object>
            {
                Success = true,
                Data = data
            };
        }

        private ApiResponse Fail(string m) => new() { Success = false, Message = m };
        private ApiResponse Success(string m) => new() { Success = true, Message = m };
    }
}
