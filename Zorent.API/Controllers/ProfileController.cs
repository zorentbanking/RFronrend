using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zorent.BLL.DTOs;
using Zorent.BLL.DTOs.Auth;
using Zorent.DAL.Data;

namespace Zorent.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(
            ApplicationDbContext context
        )
        {
            _context = context;
        }

        // =====================================================
        // GET PROFILE
        // =====================================================

        [HttpGet]

        [ProducesResponseType(
            typeof(SuccessResponseDto),
            200)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            404)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            500)]

        public async Task<IActionResult>
            GetProfile()
        {
            try
            {
                var username =
                    User.Identity?.Name;

                var user =
                    await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Username == username
                    );

                if (user == null)
                {
                    return NotFound(
                        new ErrorResponseDto
                        {
                            Success = false,
                            Message = "User not found"
                        });
                }

                return Ok(
                    new SuccessResponseDto
                    {
                        Success = true,

                        Message =
                            "Profile fetched successfully",

                        Data = new
                        {
                            fullName = user.FullName,

                            username = user.Username,

                            email = user.Email,

                            phone = user.Phone,

                            address = user.Address,

                            dateOfBirth = user.DOB
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
                        Message = ex.Message
                    });
            }
        }


        // =====================================================
        // UPDATE PROFILE
        // =====================================================

        [HttpPut("update")]

        [ProducesResponseType(
            typeof(SuccessResponseDto),
            200)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            404)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            500)]

        public async Task<IActionResult>
            UpdateProfile(
                [FromBody]
                UpdateProfileDto dto
            )
        {
            try
            {
                var username =
                    User.Identity?.Name;

                var user =
                    await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Username == username
                    );

                if (user == null)
                {
                    return NotFound(
                        new ErrorResponseDto
                        {
                            Success = false,
                            Message = "User not found"
                        });
                }

                user.FullName =
                    dto.FullName;

                user.Phone =
                    dto.Phone;

                user.Address =
                    dto.Address;

                user.DOB =
                    dto.DateOfBirth;

                await _context
                    .SaveChangesAsync();

                return Ok(
                    new SuccessResponseDto
                    {
                        Success = true,

                        Message =
                            "Profile updated successfully",

                        Data = ""
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ErrorResponseDto
                    {
                        Success = false,
                        Message = ex.Message
                    });
            }
        }

        // =====================================================
        // CHANGE PASSWORD
        // =====================================================

        [HttpPut("change-password")]

        [ProducesResponseType(
            typeof(SuccessResponseDto),
            200)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            400)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            404)]

        [ProducesResponseType(
            typeof(ErrorResponseDto),
            500)]

        public async Task<IActionResult>
            ChangePassword(
                ChangePasswordDto dto
            )
        {
            try
            {
                var username =
                    User.Identity?.Name;

                var user =
                    await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Username == username
                    );

                if (user == null)
                {
                    return NotFound(
                        new ErrorResponseDto
                        {
                            Success = false,
                            Message = "User not found"
                        });
                }

                bool isValid =
                    BCrypt.Net.BCrypt.Verify(
                        dto.CurrentPassword,
                        user.PasswordHash
                    );

                if (!isValid)
                {
                    return BadRequest(
                        new ErrorResponseDto
                        {
                            Success = false,
                            Message =
                                "Current password is incorrect"
                        });
                }

                user.PasswordHash =
                    BCrypt.Net.BCrypt
                    .HashPassword(
                        dto.NewPassword
                    );

                await _context
                    .SaveChangesAsync();

                return Ok(
                    new SuccessResponseDto
                    {
                        Success = true,

                        Message =
                            "Password changed successfully",

                        Data = ""
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ErrorResponseDto
                    {
                        Success = false,
                        Message = ex.Message
                    });
            }
        }
    }
}