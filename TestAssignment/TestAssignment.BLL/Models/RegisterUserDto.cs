using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAssignment.BLL.Models
{
    public class RegisterUserDto
    {
        public required string Username { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }
    }
}
