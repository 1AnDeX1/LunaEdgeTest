using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestAssignment.BLL.Interfaces;
using TestAssignment.BLL.Models;

namespace TestAssignment.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService,
            IConfiguration config)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registeruserDto)
        {
            try
            {
                await _userService.RegisterAsync(registeruserDto);
                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error registering user: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("login")] 
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            var user = await _userService.LoginAsync(loginUserDto);

            if (user != null)
            {
                var token = _userService.GenerateToken(user);
                return Ok(token);
            }

            return NotFound("User not found");
        }
    }
}
