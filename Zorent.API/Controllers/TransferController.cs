using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Zorent.BLL.DTOs.Auth;
using Zorent.BLL.DTOs.Transaction;

using Zorent.BLL.Interfaces;

namespace Zorent.API.Controllers
{
    [Authorize]

    [ApiController]

    [Route("api/transfer")]

    public class TransferController
        : ControllerBase
    {
        private readonly ITransactionService
            _service;

        public TransferController(
            ITransactionService service
        )
        {
            _service = service;
        }

        // =====================================================
        // TRANSFER MONEY
        // =====================================================

        [HttpPost]

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
        Transfer(
            TransferDto dto
        )
        {
            try
            {
                var result =
                    await _service
                        .Transfer(dto);

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
                            result.Message,

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