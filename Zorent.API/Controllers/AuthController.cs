using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Zorent.BLL.DTOs.Auth;
using Zorent.BLL.Interfaces;

using Zorent.Common.Responses;

using Zorent.DAL.Data;

namespace Zorent.API.Controllers
{
    [ApiController]

    [Route("api/auth")]

    public class AuthController
        : ControllerBase
    {
        private readonly IAuthService
            _authService;

        private readonly ApplicationDbContext
            _context;

        public AuthController(
            IAuthService authService,
            ApplicationDbContext context)
        {
            _authService = authService;

            _context = context;
        }

        // =====================================================
        // REGISTER
        // =====================================================

        [HttpPost("register")]

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
        Register(RegisterDto dto)
        {
            try
            {
                var result =
                    await _authService
                        .Register(dto);

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

                        Data = result.Data
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
        // LOGIN
        // =====================================================

        [HttpPost("login")]

        [ProducesResponseType(
            typeof(SuccessResponseDto),
            StatusCodes.Status200OK)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status401Unauthorized)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult>
        Login(LoginDto dto)
        {
            try
            {
                var result =
                    await _authService
                        .Login(dto);

                if (!result.Success)
                {
                    return Unauthorized(
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

                        Data = new
                        {
                            accessToken =
                                result.Data
                                    ?.AccessToken,

                            refreshToken =
                                result.Data
                                    ?.RefreshToken,

                            id =
                                result.Data
                                    ?.Id,

                            username =
                                result.Data
                                    ?.Username,

                            fullName =
                                result.Data
                                    ?.FullName,

                            email =
                                result.Data
                                    ?.Email,

                            phone =
                                result.Data
                                    ?.Phone,

                            address =
                                result.Data
                                    ?.Address
                        }
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
        // FORGOT PASSWORD
        // =====================================================

        [HttpPost("forgot-password")]

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
        ForgotPassword(
            ForgotPasswordDto dto)
        {
            try
            {
                var result =
                    await _authService
                        .ForgotPassword(dto);

                if (
                    result ==
                    "Email is not registerd!!"
                )
                {
                    return BadRequest(
                        new ErrorResponseDto
                        {
                            Success = false,

                            Message = result
                        });
                }

                return Ok(
                    new SuccessResponseDto
                    {
                        Success = true,

                        Message = result
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
        // RESET PASSWORD
        // =====================================================

        [HttpPost("reset-password")]

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
        ResetPassword(
            [FromBody]
            ResetPasswordDto dto)
        {
            try
            {
                var result =
                    await _authService
                        .ResetPasswordAsync(
                            dto
                        );

                if (
                    result !=
                    "Password reset successful"
                )
                {
                    return BadRequest(
                        new ErrorResponseDto
                        {
                            Success = false,

                            Message = result
                        });
                }

                return Ok(
                    new SuccessResponseDto
                    {
                        Success = true,

                        Message = result
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
        // VALIDATE ACCOUNT
        // =====================================================

        [Authorize]

        [HttpGet(
            "validate-account/{accountNumber}"
        )]

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
        ValidateAccount(
            string accountNumber)
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(
                        accountNumber
                    )
                )
                {
                    return BadRequest(
                        new ErrorResponseDto
                        {
                            Success = false,

                            Message =
                                "Account number is required"
                        });
                }

                if (
                    accountNumber.Length != 10
                )
                {
                    return BadRequest(
                        new ErrorResponseDto
                        {
                            Success = false,

                            Message =
                                "Account number must be 10 digits"
                        });
                }

                if (
                    !accountNumber.All(
                        char.IsDigit
                    )
                )
                {
                    return BadRequest(
                        new ErrorResponseDto
                        {
                            Success = false,

                            Message =
                                "Account number must contain only numbers"
                        });
                }

                var account =
                    await _context.Accounts
                        .FirstOrDefaultAsync(
                            a =>
                                a.AccountNumber
                                    .Trim()
                                ==
                                accountNumber
                                    .Trim()
                        );

                if (account == null)
                {
                    return Ok(
                        new SuccessResponseDto
                        {
                            Success = false,

                            Message =
                                "Account does not exist"
                        });
                }

                return Ok(
                    new SuccessResponseDto
                    {
                        Success = true,

                        Message =
                            "Account exists"
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
        // VALIDATE RESET TOKEN
        // =====================================================

        [HttpGet(
            "validate-reset-token"
        )]

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
        ValidateResetToken(
            string token)
        {
            try
            {
                var user =
                    await _context.Users
                        .FirstOrDefaultAsync(
                            u =>
                                u.ResetToken
                                == token
                        );

                if (user == null)
                {
                    return BadRequest(
                        new ErrorResponseDto
                        {
                            Success = false,

                            Message =
                                "Reset link has expired or already been used"
                        });
                }

                if (
                    user.ResetTokenExpiry
                    == null
                    ||
                    user.ResetTokenExpiry
                    < DateTime.UtcNow
                )
                {
                    return BadRequest(
                        new ErrorResponseDto
                        {
                            Success = false,

                            Message =
                                "Reset link has expired"
                        });
                }

                return Ok(
                    new SuccessResponseDto
                    {
                        Success = true,

                        Message =
                            "Valid token"
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