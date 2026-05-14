using System;
using System.Collections.Generic;
using System.Text;

namespace Zorent.BLL.DTOs
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
