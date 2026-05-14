using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Zorent.BLL.DTOs.Auth;
using Zorent.BLL.Interfaces;
using Zorent.BLL.Validators;
using Zorent.Common.Helpers;
using Zorent.Common.Responses;
using Zorent.DAL.Data;
using Zorent.DAL.Interfaces;
using Zorent.Domain.Entities;

namespace Zorent.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private readonly EmailHelper _emailHelper;

        public AuthService(IUserRepository userRepository, JwtService jwtService, ApplicationDbContext context, EmailHelper emailHelper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _context = context;
            _emailHelper = emailHelper;
        }


        public async Task<ApiResponse> Register(RegisterDto dto)
        {
            // FULL NAME VALIDATION
            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Full name is required"
                };
            }

            if (dto.FullName.Trim().Length < 3)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Full name must be at least 3 characters"
                };
            }

            // EMAIL VALIDATION
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Email is required"
                };
            }

            try
            {
                var email = new System.Net.Mail.MailAddress(dto.Email);
            }
            catch
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Invalid email format"
                };
            }

            // PHONE VALIDATION
            if (string.IsNullOrWhiteSpace(dto.Phone))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Phone number is required"
                };
            }

            if (dto.Phone.Length != 10 || !dto.Phone.All(char.IsDigit))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Phone number must contain exactly 10 digits"
                };
            }

            // DOB VALIDATION
            if (dto.DOB == default)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Date of birth is required"
                };
            }

            if (dto.DOB > DateTime.Now)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Date of birth cannot be in future"
                };
            }

            var age = DateTime.Now.Year - dto.DOB.Year;

            if (dto.DOB.Date > DateTime.Now.AddYears(-age))
            {
                age--;
            }

            if (age < 18)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "User must be at least 18 years old"
                };
            }

            // ADDRESS VALIDATION
            if (string.IsNullOrWhiteSpace(dto.Address))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Address is required"
                };
            }

            if (dto.Address.Length > 1500)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Address cannot exceed 1500 characters"
                };
            }

            // USERNAME VALIDATION
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Username is required"
                };
            }

            if (dto.Username.Trim().Length < 4)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Username must be at least 4 characters"
                };
            }

            // PASSWORD VALIDATION
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Password is required"
                };
            }

            if (!PasswordValidator.IsValid(dto.Password))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Password must contain uppercase, lowercase, number, special character and minimum 8 characters"
                };
            }

            // CHECK EXISTING USER
            // CHECK EMAIL EXISTS
            var emailExists =
                await _userRepository
                    .EmailExistsAsync(dto.Email);

            if (emailExists)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }

            // CHECK USERNAME EXISTS
            var usernameExists =
                await _userRepository
                    .UsernameExistsAsync(dto.Username);

            if (usernameExists)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }


            // CREATE USER
            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                Phone = dto.Phone.Trim(),
                DOB = dto.DOB,
                Address = dto.Address.Trim(),
                Username = dto.Username.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                FailedAttempts = 0,
                IsLocked = false,
                LockoutEnd = null
            };

            // SAVE USER
            await _userRepository.AddUserAsync(user);

            await _userRepository.SaveChangesAsync();

            return new ApiResponse
            {
                Success = true,
                Message = "Registration successful"
            };
        }


        public async Task<ApiResponse<AuthResponseDto>> Login(LoginDto dto)
        {
            // USERNAME EMPTY
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return Fail("Username is required");
            }

            // PASSWORD EMPTY
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return Fail("Password is required");
            }

            // FIND USER
            var user = await _userRepository
                .GetByUsernameAsync(dto.Username);

            // USER NOT FOUND
            if (user == null)
            {
                return Fail("Username does not exist");
            }

            // ACCOUNT LOCK CHECK
            if (user.IsLocked && user.LockoutEnd > DateTime.Now)
            {
                return Fail(
                    $"Please Login After  {user.LockoutEnd}"
                );
            }

            // PASSWORD VERIFY
            bool isValid = BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash
            );

            // WRONG PASSWORD
            if (!isValid)
            {
                user.FailedAttempts++;

                // LOCK AFTER 3 ATTEMPTS
                if (user.FailedAttempts >= 3)
                {
                    user.IsLocked = true;

                    user.LockoutEnd =
                        DateTime.Now.AddMinutes(5);

                    await _userRepository.SaveChangesAsync();

                    return Fail(
                        "Account locked for 5 minutes due to multiple failed attempts"
                    );
                }

                await _userRepository.SaveChangesAsync();

                return Fail(
                    $"Incorrect password. Attempts left: {3 - user.FailedAttempts}"
                );
            }

            // RESET FAILED ATTEMPTS
            user.FailedAttempts = 0;
            user.IsLocked = false;
            user.LockoutEnd = null;

            // GENERATE TOKENS
            var accessToken =
                _jwtService.GenerateToken(user);

            var refreshToken =
                Guid.NewGuid().ToString();

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiry =
                DateTime.Now.AddDays(7);

            await _userRepository.SaveChangesAsync();

            // SUCCESS
            return new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Message = "Login successful",

                Data = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,

                    Id = user.Id,

                    Username = user.Username,

                    FullName = user.FullName,

                    Email = user.Email,

                    Phone = user.Phone,

                    Address = user.Address
                }
            };
        }


        public async Task<ApiResponse<AuthResponseDto>> RefreshToken(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

            //  ADD THIS CHECK
            if (user == null ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiry < DateTime.Now)
            {
                return Fail<AuthResponseDto>("Invalid refresh token");
            }

            var newAccessToken = _jwtService.GenerateToken(user);
            var newRefreshToken = Guid.NewGuid().ToString();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7);

            await _userRepository.SaveChangesAsync();

            return new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Data = new AuthResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }
            };
        }


        public async Task Logout(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;

                await _userRepository.SaveChangesAsync();
            }
        }




        private ApiResponse<AuthResponseDto> Fail(string message)
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = message
            };
        }

        private ApiResponse<T> Fail<T>(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message
            };
        }
        public async Task<string> ForgotPassword(ForgotPasswordDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null)
                return "User not found";

            var token = Guid.NewGuid().ToString();

            user.ResetToken = token;

            user.ResetTokenExpiry =
                DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            // FRONTEND RESET PAGE
            var resetLink =
                $"http://localhost:62196/reset-password?token={token}";

            await _emailHelper.SendResetEmailAsync(
                user.Email,
                resetLink
            );

            return "Reset link sent to email";
        }

        public async Task<string> ResetPasswordAsync(
     ResetPasswordDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.ResetToken == dto.Token
                );

            // TOKEN INVALID / ALREADY USED
            if (user == null ||
                string.IsNullOrEmpty(user.ResetToken))
            {
                return "Reset link has expired or already been used";
            }

            // TOKEN EXPIRED
            if (user.ResetTokenExpiry == null ||
                user.ResetTokenExpiry < DateTime.UtcNow)
            {
                return "Reset link has expired or already been used";
            }

            // PASSWORD MISMATCH
            if (dto.NewPassword != dto.ConfirmPassword)
                return "Passwords do not match";

            // UPDATE PASSWORD
            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            // CLEAR TOKEN
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return "Password reset successful";
        }
    }
}
