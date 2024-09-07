using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssignment.Dal.Filters;
using TestAssignment.DAL;
using TestAssignment.DAL.Entities;
using TestAssignment.DAL.Enums;
using TestAssignment.DAL.Repositories;

namespace TestAssignment.Tests.TaskTests
{
    public class TaskRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly AppDbContext _context;
        private readonly TaskRepository _taskRepository;

        public TaskRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(_dbContextOptions);
            _taskRepository = new TaskRepository(_context);
        }

        private UserTask CreateTask(Guid userId, int status, int priority)
        {
            return new UserTask
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = "Test Task",
                Status = (StatusEnum)status,
                Priority = (PriorityEnum)priority,
                DueDate = DateTime.Today,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task AddAsync_ShouldAddTask()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var task = CreateTask(userId, 1, 2);

            //Act
            await _taskRepository.AddAsync(task);
            var existedTask = await _context.Tasks.FirstOrDefaultAsync(task => task.Id == task.Id);

            //Assert
            Assert.NotNull(existedTask);
            Assert.Equal(task, existedTask);
        }

        [Fact]
        public async Task GetTasksAsync_ShouldReturnFilteredTasks()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var task1 = CreateTask(userId, 0, 2);
            var task2 = CreateTask(userId, 1, 1);
            
            await _context.AddRangeAsync(task1, task2);
            await _context.SaveChangesAsync();

            var filter = new TaskFilter { Status = StatusEnum.Pending, DueDate = DateTime.Today,
                PageNumber = 1, PageSize = 10 };

            var result = await _taskRepository.GetTasksAsync(userId, filter);

            //Assert
            Assert.Single(result); // Only task2 matches the filter (DueDate = Today)
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnTaskById()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var task = CreateTask(userId, 0, 2);
            await _context.AddAsync(task);
            await _context.SaveChangesAsync();

            //Act
            var result = await _taskRepository.GetTaskByIdAsync(userId, task.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(task.Id, result.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTask()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var task = CreateTask(userId, 0, 2);
            await _context.AddAsync(task);
            await _context.SaveChangesAsync();

            //Act
            task.Title = "Updated Task";
            await _taskRepository.UpdateAsync(task);
            var updatedTask = await _context.Tasks.FindAsync(task.Id);

            // Assert
            Assert.Equal("Updated Task", updatedTask?.Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteTask()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var task = CreateTask(userId, 1, 2);
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            // Act
            await _taskRepository.DeleteAsync(task);
            var deletedTask = await _context.Tasks.FindAsync(task.Id);

            // Assert
            Assert.Null(deletedTask);
        }
    }
}
