using System.ComponentModel.DataAnnotations;

namespace TestAssignment.BLL.Models
{
    public class UserDto
    {
        public required string Username { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }
    }
}
