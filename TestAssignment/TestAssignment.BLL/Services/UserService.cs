using AutoMapper;
using TestAssignment.BLL.Interfaces;
using TestAssignment.BLL.Interfaces.Auth;
using TestAssignment.DAL.Entities;
using TestAssignment.DAL.Interfaces;
using TestAssignment.BLL.Models;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace TestAssignment.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IConfiguration config,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _config = config;
        }


        public async Task RegisterAsync(RegisterUserDto registerUserDto)
        {
            if (await _userRepository.ExistsByEmailAsync(registerUserDto.Email) 
                || await _userRepository.ExistsByUsernameAsync(registerUserDto.Username))
            {
                throw new AppException("User with this email or username already exists");
            }

            try
            {
                ValidatePassword(registerUserDto.Password);
            }
            catch (AppException ex)
            {
                throw new AppException($"Password validation failed: {ex.Message}");
            }

            var hashedPassword = _passwordHasher.Generate(registerUserDto.Password);

            var user = _mapper.Map<User>(registerUserDto);
            user.PasswordHash = hashedPassword;

            await _userRepository.AddAsync(user);
        }

        public async Task<UserDto> LoginAsync(LoginUserDto loginUserDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginUserDto.Email);

            if (user == null)
            {
                throw new AppException("User not found");
            }

            if (!_passwordHasher.VerifyPassword(loginUserDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }

            return _mapper.Map<UserDto>(user);
        }

        private static void ValidatePassword(string password)
        {
            if (password.Length < 8)
            {
                throw new AppException("Password must be at least 8 characters long.");
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                throw new AppException("Password must contain at least one uppercase letter.");
            }

            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                throw new AppException("Password must contain at least one lowercase letter.");
            }

            if (!Regex.IsMatch(password, @"\d"))
            {
                throw new AppException("Password must contain at least one digit.");
            }

            if (!Regex.IsMatch(password, @"[\W_]"))
            {
                throw new AppException("Password must contain at least one special character (e.g. !@#$%^&*).");
            }
        }

        public string GenerateToken(UserDto user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
