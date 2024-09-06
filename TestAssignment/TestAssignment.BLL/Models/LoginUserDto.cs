using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAssignment.BLL.Models
{
    public class LoginUserDto
    {
        public required string Email { get; set; }

        public required string Password { get; set; }
    }
}
