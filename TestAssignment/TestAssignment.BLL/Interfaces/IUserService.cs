using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssignment.BLL.Models;

namespace TestAssignment.BLL.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterUserDto userDto);
        Task<UserDto> LoginAsync(LoginUserDto loginUserDto);
        string GenerateToken(UserDto user);
    }
}
