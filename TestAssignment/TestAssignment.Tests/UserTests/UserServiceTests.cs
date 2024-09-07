using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssignment.BLL;
using TestAssignment.BLL.Interfaces.Auth;
using TestAssignment.BLL.Models;
using TestAssignment.BLL.Services;
using TestAssignment.DAL.Entities;
using TestAssignment.DAL.Interfaces;

namespace TestAssignment.Tests.UserTests
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConfiguration> _configMock;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _mapperMock = new Mock<IMapper>();
            _configMock = new Mock<IConfiguration>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _configMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenUserExists()
        {
            // Arrange
            var registerUserDto = new RegisterUserDto
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "Password123!"
            };

            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(registerUserDto.Email))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _userService.RegisterAsync(registerUserDto));
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser_WhenDataIsValid()
        {
            // Arrange
            var registerUserDto = new RegisterUserDto
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "Password123!"
            };

            _userRepositoryMock.Setup(r => r.ExistsByUsernameAsync(registerUserDto.Email))
                .ReturnsAsync(false);

            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(registerUserDto.Email))
                .ReturnsAsync(false);

            _passwordHasherMock.Setup(p => p.Generate(registerUserDto.Password))
                .Returns("hashedpassword");

            var user = new User
            {
                Email = registerUserDto.Email,
                Username = registerUserDto.Username,
                PasswordHash = "hashedpassword"
            };

            _mapperMock.Setup(m => m.Map<User>(registerUserDto))
                .Returns(user);

            // Act
            await _userService.RegisterAsync(registerUserDto);
            // Assert
            _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == registerUserDto.Email)), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var loginUserDto = new LoginUserDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(loginUserDto.Email))
                .ReturnsAsync(null as User);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _userService.LoginAsync(loginUserDto));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenPasswordInvalid()
        {
            // Arrange
            var loginUserDto = new LoginUserDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = new User 
            { 
                Username = "testuser",
                Email = loginUserDto.Email, 
                PasswordHash = "hashedpassword" 
            };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(loginUserDto.Email))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(p => p.VerifyPassword(loginUserDto.Password, user.PasswordHash))
                .Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginAsync(loginUserDto));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnUserDto_WhenDataIsValid()
        {
            // Arrange
            var loginUserDto = new LoginUserDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Username = "testuser",
                Email = loginUserDto.Email, 
                PasswordHash = "hashedpassword" 
            };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(loginUserDto.Email))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(p => p.VerifyPassword(loginUserDto.Password, user.PasswordHash))
                .Returns(true);

            var userDto = new UserDto 
            { 
                Username = user.Username,
                Email = user.Email,
                Password = user.PasswordHash,
            };
            _mapperMock.Setup(m => m.Map<UserDto>(user))
                .Returns(userDto);

            // Act
            var result = await _userService.LoginAsync(loginUserDto);

            // Assert
            Assert.Equal(userDto.Username, result.Username);
            Assert.Equal(userDto.Email, result.Email);
            Assert.Equal(userDto.Password, result.Password);
        }

        [Fact]
        public void GenerateToken_ShouldReturnToken()
        {
            // Arrange
            var userDto = new UserDto
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!"
            };

            _configMock.SetupGet(c => c["Jwt:Key"])
                .Returns("ThisIsASecretKeyThatIsAtLeast32CharactersLong");
            _configMock.SetupGet(c => c["Jwt:Issuer"])
                .Returns("http://localhost");
            _configMock.SetupGet(c => c["Jwt:Audience"])
                .Returns("http://localhost");

            // Act
            var token = _userService.GenerateToken(userDto);

            // Assert
            Assert.NotNull(token);
        }
    }
}
