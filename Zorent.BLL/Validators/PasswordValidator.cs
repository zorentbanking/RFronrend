using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Zorent.BLL.Validators
{
    public static class PasswordValidator
    {
        public static bool IsValid(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => "!@#$%^&*".Contains(ch));
        }
    }
}