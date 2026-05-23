using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Filters;

using Zorent.API.Swagger;

using Zorent.BLL.DTOs.Account;
using Zorent.BLL.DTOs.Auth;
using Zorent.BLL.Interfaces;

namespace Zorent.API.Controllers
{
    [Authorize]

    [ApiController]

    [Route("api/accounts")]

    public class AccountsController
        : ControllerBase
    {
        private readonly IAccountService
            _service;

        public AccountsController(
            IAccountService service)
        {
            _service = service;
        }

        // =====================================================
        // CREATE ACCOUNT
        // =====================================================

        [HttpPost("create")]

        [ProducesResponseType(
            typeof(SuccessResponseDto),
            StatusCodes.Status200OK)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status400BadRequest)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status500InternalServerError)]

        [SwaggerResponseExample(
            200,
            typeof(SuccessResponseExample))]

        [SwaggerResponseExample(
            400,
            typeof(ErrorResponseExample))]

        [SwaggerResponseExample(
            500,
            typeof(ErrorResponseExample))]

        public async Task<IActionResult>
        Create(
            [FromBody]
            CreateAccountDto dto)
        {
            try
            {
                var userId =
                    int.Parse(
                        User.FindFirst("UserId")!
                            .Value);

                var result =
                    await _service
                        .CreateAccount(
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
                            ex.InnerException?.Message
                            ?? ex.Message
                    });
            }
        }

        // =====================================================
        // GET MY ACCOUNTS
        // =====================================================

        [HttpGet("my")]

        [ProducesResponseType(
            typeof(SuccessResponseDto),
            StatusCodes.Status200OK)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status401Unauthorized)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status500InternalServerError)]

        [SwaggerResponseExample(
            200,
            typeof(SuccessResponseExample))]

        [SwaggerResponseExample(
            401,
            typeof(ErrorResponseExample))]

        [SwaggerResponseExample(
            500,
            typeof(ErrorResponseExample))]

        public async Task<IActionResult>
        Get()
        {
            try
            {
                var userId =
                    int.Parse(
                        User.FindFirst("UserId")!
                            .Value);

                var result =
                    await _service
                        .GetUserAccounts(
                            userId);

                return Ok(
                    new SuccessResponseDto
                    {
                        Success = true,

                        Message =
                            "Accounts fetched successfully",

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
                            ex.InnerException?.Message
                            ?? ex.Message
                    });
            }
        }

        // =====================================================
        // CLOSE DEPOSIT
        // =====================================================

        [HttpPost("close-deposit")]

        [ProducesResponseType(
            typeof(SuccessResponseDto),
            StatusCodes.Status200OK)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status400BadRequest)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status500InternalServerError)]

        [SwaggerResponseExample(
            200,
            typeof(SuccessResponseExample))]

        [SwaggerResponseExample(
            400,
            typeof(ErrorResponseExample))]

        [SwaggerResponseExample(
            500,
            typeof(ErrorResponseExample))]

        public async Task<IActionResult>
        CloseDeposit(
            [FromBody]
            CloseDepositDto dto)
        {
            try
            {
                var userId =
                    int.Parse(
                        User.FindFirst("UserId")!
                            .Value);

                var result =
                    await _service
                        .CloseDeposit(
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
                            ex.InnerException?.Message
                            ?? ex.Message
                    });
            }
        }

        // =====================================================
        // DEPOSIT MONEY
        // =====================================================

        [HttpPost("deposit")]

        [ProducesResponseType(
            typeof(SuccessResponseDto),
            StatusCodes.Status200OK)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status400BadRequest)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status500InternalServerError)]

        [SwaggerResponseExample(
            200,
            typeof(SuccessResponseExample))]

        [SwaggerResponseExample(
            400,
            typeof(ErrorResponseExample))]

        [SwaggerResponseExample(
            500,
            typeof(ErrorResponseExample))]

        public async Task<IActionResult>
        Deposit(
            [FromBody]
            DepositDto dto)
        {
            try
            {
                var userId =
                    int.Parse(
                        User.FindFirst("UserId")!
                            .Value);

                var result =
                    await _service
                        .DepositMoney(
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
                            ex.InnerException?.Message
                            ?? ex.Message
                    });
            }
        }

        // =====================================================
        // GET ACCOUNT BY NUMBER
        // =====================================================

        [HttpGet("{accountNumber}")]

        [ProducesResponseType(
            typeof(SuccessResponseDto),
            StatusCodes.Status200OK)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status400BadRequest)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status500InternalServerError)]

        [SwaggerResponseExample(
            200,
            typeof(SuccessResponseExample))]

        [SwaggerResponseExample(
            400,
            typeof(ErrorResponseExample))]

        [SwaggerResponseExample(
            500,
            typeof(ErrorResponseExample))]

        public async Task<IActionResult>
        GetAccount(
            string accountNumber)
        {
            try
            {
                var userId =
                    int.Parse(
                        User.FindFirst("UserId")!
                            .Value);

                var result =
                    await _service
                        .GetAccountByNumber(
                            accountNumber,
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
                            ex.InnerException?.Message
                            ?? ex.Message
                    });
            }
        }
    }
}