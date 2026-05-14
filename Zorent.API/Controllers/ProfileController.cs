using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using Zorent.DAL.Data;

using Zorent.BLL.DTOs;

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

        // GET PROFILE
        [HttpGet]
        public async Task<IActionResult> GetProfile()
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
                return NotFound(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            return Ok(new
            {
                success = true,

                data = new
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

        // UPDATE PROFILE
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileDto dto
        )
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
                return NotFound(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            // UPDATE DATA
            user.FullName =
                dto.FullName;

            user.Phone =
                dto.Phone;

            user.Address =
                dto.Address;

            user.DOB =
                dto.DateOfBirth;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,

                message =
                    "Profile updated successfully"
            });
        }
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(
    ChangePasswordDto dto)
        {
            var username = User.Identity?.Name;

            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Username == username);

            if (user == null)
            {
                return NotFound();
            }

            // VERIFY CURRENT PASSWORD
            bool isValid =
                BCrypt.Net.BCrypt.Verify(
                    dto.CurrentPassword,
                    user.PasswordHash
                );

            if (!isValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Current password is incorrect"
                });
            }

            // HASH NEW PASSWORD
            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    dto.NewPassword
                );

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Password changed successfully"
            });
        }
    }
}