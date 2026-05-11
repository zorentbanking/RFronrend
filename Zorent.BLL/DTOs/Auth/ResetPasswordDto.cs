using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.BLL.DTOs.Auth
{
    public class ResetPasswordDto
    {
        public string Token { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
