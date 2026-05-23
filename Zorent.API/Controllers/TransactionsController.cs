using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Zorent.BLL.DTOs.Auth;
using Zorent.BLL.DTOs.Transaction;

using Zorent.BLL.Interfaces;

namespace Zorent.API.Controllers
{
    [Authorize]

    [ApiController]

    [Route("api/transactions")]

    public class TransactionsController
        : ControllerBase
    {
        private readonly ITransactionService
            _service;

        public TransactionsController(
            ITransactionService service)
        {
            _service = service;
        }

        // =====================================================
        // TRANSACTION HISTORY
        // =====================================================

        [HttpGet]

        [ProducesResponseType(
            typeof(SuccessResponseDto),
            StatusCodes.Status200OK)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status400BadRequest)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult>
        Get(
            int accountId,
            int page = 1,
            string sortBy = "date",
            string order = "desc")
        {
            try
            {
                var result =
                    await _service
                        .GetTransactions(
                            accountId,
                            page,
                            sortBy,
                            order);

                if (!result.Success)
                {
                    return BadRequest(
                        new ErrorResponseDto
                        {
                            Success = false,

                            Message =
                                result.Message
                        });
                }

                return Ok(
                    new SuccessResponseDto
                    {
                        Success = true,

                        Message =
                            "Transactions fetched successfully",

                        Data =
                            result.Data
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,

                    new ErrorResponseDto
                    {
                        Success = false,

                        Message =
                            ex.Message
                    });
            }
        }

        // =====================================================
        // SEARCH TRANSACTIONS
        // =====================================================

        [HttpPost("search")]

        [ProducesResponseType(
            typeof(SuccessResponseDto),
            StatusCodes.Status200OK)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status400BadRequest)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult>
        Search(
            [FromBody]
            TransactionSearchDto dto)
        {
            try
            {
                var userId =
                    int.Parse(
                        User.FindFirst(
                            "UserId")!.Value);

                var result =
                    await _service
                        .Search(
                            dto,
                            userId);

                if (!result.Success)
                {
                    return BadRequest(
                        new ErrorResponseDto
                        {
                            Success = false,

                            Message =
                                result.Message
                        });
                }

                return Ok(
                    new SuccessResponseDto
                    {
                        Success = true,

                        Message =
                            "Search successful",

                        Data =
                            result.Data
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,

                    new ErrorResponseDto
                    {
                        Success = false,

                        Message =
                            ex.Message
                    });
            }
        }

        // =====================================================
        // EXPORT CSV
        // =====================================================

        [HttpPost("export")]

        [ProducesResponseType(
            typeof(FileContentResult),
            StatusCodes.Status200OK)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult>
        Export(
            [FromBody]
            TransactionSearchDto dto)
        {
            try
            {
                var userId =
                    int.Parse(
                        User.FindFirst(
                            "UserId")!.Value);

                var file =
                    await _service
                        .ExportToCsv(
                            dto,
                            userId);

                return File(
                    file,
                    "text/csv",
                    "transactions.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,

                    new ErrorResponseDto
                    {
                        Success = false,

                        Message =
                            ex.Message
                    });
            }
        }

        // =====================================================
        // ACCOUNT STATEMENT
        // =====================================================

        [HttpGet(
            "statement/{accountNumber}")]

        [ProducesResponseType(
            typeof(SuccessResponseDto),
            StatusCodes.Status200OK)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status400BadRequest)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult>
        GetStatement(
            string accountNumber)
        {
            try
            {
                var result =
                    await _service
                        .GetStatement(
                            accountNumber);

                if (!result.Success)
                {
                    return BadRequest(
                        new ErrorResponseDto
                        {
                            Success = false,

                            Message =
                                result.Message
                        });
                }

                return Ok(
                    new SuccessResponseDto
                    {
                        Success = true,

                        Message =
                            "Statement fetched successfully",

                        Data =
                            result.Data
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,

                    new ErrorResponseDto
                    {
                        Success = false,

                        Message =
                            ex.Message
                    });
            }
        }
    }
}