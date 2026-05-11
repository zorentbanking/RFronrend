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

            if (source == null || dest == null)
                return Fail("Invalid accounts");

            if (source.Id == dest.Id)
                return Fail("Cannot transfer to same account");

            if (dest.Status != "Active")
                return Fail("Destination inactive");

            if (dto.Amount <= 0 || dto.Amount > source.Balance)
                return Fail("Invalid amount");

            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                source.Balance -= dto.Amount;
                dest.Balance += dto.Amount;

                _context.Transactions.Add(new Transaction
                {
                    AccountId = source.Id,
                    Type = "Debit",
                    Amount = dto.Amount,
                    Description = dto.Description,
                    BalanceAfter = source.Balance
                });

                _context.Transactions.Add(new Transaction
                {
                    AccountId = dest.Id,
                    Type = "Credit",
                    Amount = dto.Amount,
                    Description = dto.Description,
                    BalanceAfter = dest.Balance
                });

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Success("Transfer successful");
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

        public async Task<ApiResponse<object>> Search(TransactionSearchDto f)
        {
            var query = _context.Transactions.AsQueryable();

            // filters
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

            // total count
            var total = await query.CountAsync();

            var data = await query
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
        public async Task<byte[]> ExportToCsv(TransactionSearchDto f)
        {
            var query = _context.Transactions.AsQueryable();

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
            sb.AppendLine("TransactionId,Type,Amount,Description,Date,BalanceAfter");

            foreach (var t in data)
            {
                sb.AppendLine($"{t.TransactionId},{t.Type},{t.Amount},{t.Description},{t.CreatedAt},{t.BalanceAfter}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private ApiResponse Fail(string m) => new() { Success = false, Message = m };
        private ApiResponse Success(string m) => new() { Success = true, Message = m };
    }
}
