using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.BLL.DTOs.Auth
{
    public class ErrorResponseDto
    {
        public bool Success { get; set; } = false;

        public string Message { get; set; } = string.Empty;
    }
}
