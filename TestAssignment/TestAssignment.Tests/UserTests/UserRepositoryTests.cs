using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestAssignment.DAL;
using TestAssignment.DAL.Entities;
using TestAssignment.DAL.Repositories;
using Xunit;

namespace TestAssignment.Tests.UserTests
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly AppDbContext _context;
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            // Use InMemory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(_dbContextOptions);
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Username = "testuser", PasswordHash = "hashedpassword" };

            // Act
            await _userRepository.AddAsync(user);
            var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            // Assert
            Assert.NotNull(userFromDb);
            Assert.Equal(user.Email, userFromDb.Email);
            Assert.Equal(user.Username, userFromDb.Username);
        }

        [Fact]
        public async Task ExistsByEmailAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Username = "testuser", PasswordHash = "hashedpassword" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.ExistsByEmailAsync(user.Email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsByEmailAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Act
            var result = await _userRepository.ExistsByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsByUsernameAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Username = "testuser", PasswordHash = "hashedpassword" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.ExistsByUsernameAsync(user.Username);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsByUsernameAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Act
            var result = await _userRepository.ExistsByUsernameAsync("nonexistentuser");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Username = "testuser", PasswordHash = "hashedpassword" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var userFromDb = await _userRepository.GetByEmailAsync(user.Email);

            // Assert
            Assert.NotNull(userFromDb);
            Assert.Equal(user.Email, userFromDb.Email);
            Assert.Equal(user.Username, userFromDb.Username);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Act
            var userFromDb = await _userRepository.GetByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(userFromDb);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
