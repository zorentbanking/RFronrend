using System;
using System.Collections.Generic;
using System.Text;
using Zorent.BLL.DTOs.Auth;
using Zorent.Common.Responses;

namespace Zorent.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> Register(RegisterDto dto);
        
        Task<ApiResponse<AuthResponseDto>> Login(LoginDto dto);
        Task<ApiResponse<AuthResponseDto>> RefreshToken(string refreshToken);
        Task<string> ForgotPassword(ForgotPasswordDto dto);

        Task<string> ResetPasswordAsync(ResetPasswordDto dto);

        Task Logout(string username);
    }
}