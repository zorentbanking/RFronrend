using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zorent.BLL.DTOs.Transaction;
using Zorent.BLL.Interfaces;
using Zorent.BLL.Services;

namespace Zorent.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionsController(ITransactionService service)
        {
            _service = service;
        }

        //  HISTORY (FR-HIST)
        [HttpGet]
        public async Task<IActionResult> Get(
            int accountId,
            int page = 1,
            string sortBy = "date",
            string order = "desc")
        {
            var result = await _service.GetTransactions(accountId, page, sortBy, order);
            return Ok(result);
        }

        //  SEARCH (FR-SRCH-01 to 04)
        [Authorize]
        [HttpPost("search")]
        public async Task<IActionResult> Search(
     [FromBody] TransactionSearchDto dto)
        {
            var userId =
                int.Parse(
                    User.FindFirst("UserId")!.Value
                );

            var result =
                await _service.Search(dto, userId);

            return Ok(result);
        }

        // ✅ EXPORT CSV (FR-SRCH-05)
        [Authorize]
        [HttpPost("export")]
        public async Task<IActionResult> Export(
     [FromBody] TransactionSearchDto dto)
        {
            var userId =
                int.Parse(
                    User.FindFirst("UserId")!.Value
                );

            var file =
                await _service.ExportToCsv(dto, userId);

            return File(
                file,
                "text/csv",
                "transactions.csv"
            );
        }

        [HttpGet("statement/{accountNumber}")]
        public async Task<IActionResult> GetStatement(
    string accountNumber)
        {
            var result =
                await _service
    .GetStatement(accountNumber);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}