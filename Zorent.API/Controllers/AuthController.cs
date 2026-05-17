using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zorent.BLL.DTOs.Auth;
using Zorent.BLL.Interfaces;
using Zorent.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace Zorent.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;
        public AuthController(IAuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.Register(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.Login(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            await _authService.Logout(username);

            return Ok("Logged out successfully");
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshToken(refreshToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("You are authorized");
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
    ForgotPasswordDto dto)
        {
            var result =
                await _authService.ForgotPassword(dto);

            if (result == "Email is not registerd!!")
            {
                return BadRequest(new
                {
                    message = result
                });
            }
            return Ok(new
            {
                message = result
            });
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
    [FromBody] ResetPasswordDto dto)
        {
            var result =
                await _authService.ResetPasswordAsync(dto);

            if (result != "Password reset successful")
            {
                return BadRequest(new
                {
                    message = result
                });
            }

            return Ok(new
            {
                message = result
            });
        }

        [Authorize]

        [HttpGet("validate-account/{accountNumber}")]
        public async Task<IActionResult> ValidateAccount(
           string accountNumber)
        {
            // CHECK EMPTY
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Account number is required"
                });
            }

            // CHECK LENGTH
            if (accountNumber.Length != 10)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Account number must be 10 digits"
                });
            }

            // CHECK NUMBERS ONLY
            if (!accountNumber.All(char.IsDigit))
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Account number must contain only numbers"
                });
            }

            // FIND ACCOUNT
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                return null;
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber.Trim() == accountNumber.Trim()
                );

            // ACCOUNT NOT FOUND
            if (account == null)
            {
                return Ok(new
                {
                    success = false,
                    message = "Account does not exist"
                });
            }

            // SUCCESS
            return Ok(new
            {
                success = true,
                message = "Account exists"
            });
        }

        [HttpGet("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken(
    string token)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.ResetToken == token
                );

            // TOKEN INVALID
            if (user == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Reset link has expired or already been used"
                });
            }

            // TOKEN EXPIRED
            if (user.ResetTokenExpiry == null ||
                user.ResetTokenExpiry < DateTime.UtcNow)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Reset link has expired"
                });
            }

            return Ok(new
            {
                success = true
            });
        }




    }
}