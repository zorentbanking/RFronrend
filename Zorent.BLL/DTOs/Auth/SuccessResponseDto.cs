using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.BLL.DTOs.Auth
{
    public class SuccessResponseDto
    {
        public bool Success { get; set; } = true;

        public string Message { get; set; } = string.Empty;

        public object? Data { get; set; }
    }
}
