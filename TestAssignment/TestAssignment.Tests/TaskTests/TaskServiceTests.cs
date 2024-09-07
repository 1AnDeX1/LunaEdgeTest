using Moq;
using Xunit;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestAssignment.BLL.Services;
using TestAssignment.BLL.Models;
using TestAssignment.DAL.Entities;
using TestAssignment.DAL.Interfaces;
using TestAssignment.Dal.Filters;
using TestAssignment.DAL.Enums;

namespace TestAssignment.Tests.TaskTests
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _mapperMock = new Mock<IMapper>();
            _taskService = new TaskService(_taskRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnTaskDto_WhenTaskIsCreated()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                Title = "New Task",
                Description = "Task Description",
                DueDate = DateTime.UtcNow,
                Status = StatusEnum.Pending,
                Priority = PriorityEnum.High
            };
            var userId = Guid.NewGuid();
            var task = new UserTask
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                DueDate = createTaskDto.DueDate,
                Status = createTaskDto.Status,
                Priority = createTaskDto.Priority,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var taskDto = new UserTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status,
                Priority = task.Priority,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };

            _mapperMock.Setup(m => m.Map<UserTask>(createTaskDto)).Returns(task);
            _mapperMock.Setup(m => m.Map<UserTaskDto>(task)).Returns(taskDto);
            _taskRepositoryMock.Setup(r => r.AddAsync(task)).Returns(Task.CompletedTask);

            // Act
            var result = await _taskService.CreateAsync(createTaskDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskDto.Title, result.Title);
            Assert.Equal(taskDto.Description, result.Description);
            _taskRepositoryMock.Verify(r => r.AddAsync(It.Is<UserTask>(t => t.Title == createTaskDto.Title)), Times.Once);
        }

        [Fact]
        public async Task GetTasksAsync_ShouldReturnTasks_WhenFilterIsApplied()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var filter = new TaskFilter
            {
                Status = StatusEnum.InProgress,
                Priority = PriorityEnum.High,
                PageNumber = 1,
                PageSize = 10
            };
            var tasks = new List<UserTask>
        {
            new UserTask
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = "Task 1",
                Status = StatusEnum.InProgress,
                Priority = PriorityEnum.High
            }
        };
            var taskDtos = new List<UserTaskDto>
        {
            new UserTaskDto
            {
                Id = tasks[0].Id,
                Title = tasks[0].Title,
                Status = tasks[0].Status,
                Priority = tasks[0].Priority
            }
        };

            _taskRepositoryMock.Setup(r => r.GetTasksAsync(userId, filter)).ReturnsAsync(tasks);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserTaskDto>>(tasks)).Returns(taskDtos);

            // Act
            var result = await _taskService.GetTasksAsync(userId, filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(taskDtos[0].Title, result.First().Title);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnTaskDto_WhenTaskExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var task = new UserTask
            {
                Id = taskId,
                UserId = userId,
                Title = "Task 1"
            };
            var taskDto = new UserTaskDto
            {
                Id = taskId,
                Title = "Task 1"
            };

            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(userId, taskId)).ReturnsAsync(task);
            _mapperMock.Setup(m => m.Map<UserTaskDto>(task)).Returns(taskDto);

            // Act
            var result = await _taskService.GetTaskByIdAsync(userId, taskId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskDto.Title, result.Title);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnTrue_WhenTaskIsUpdated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var updateTaskDto = new UpdateTaskDto
            {
                Title = "Updated Task"
            };
            var task = new UserTask
            {
                Id = taskId,
                UserId = userId,
                Title = "Old Task"
            };

            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(userId, taskId)).ReturnsAsync(task);
            _mapperMock.Setup(m => m.Map(updateTaskDto, task));
            _taskRepositoryMock.Setup(r => r.UpdateAsync(task)).Returns(Task.CompletedTask);

            // Act
            var result = await _taskService.UpdateTaskAsync(userId, taskId, updateTaskDto);

            // Assert
            Assert.True(result);
            _mapperMock.Verify(m => m.Map(updateTaskDto, task), Times.Once);
            Assert.NotEqual("Updated Task", task.Title);
            _taskRepositoryMock.Verify(r => r.UpdateAsync(task), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenTaskIsDeleted()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var task = new UserTask
            {
                Id = taskId,
                UserId = userId
            };

            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(userId, taskId)).ReturnsAsync(task);
            _taskRepositoryMock.Setup(r => r.DeleteAsync(task)).Returns(Task.CompletedTask);

            // Act
            var result = await _taskService.DeleteAsync(taskId, userId);

            // Assert
            Assert.True(result);
            _taskRepositoryMock.Verify(r => r.DeleteAsync(task), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(userId, taskId)).ReturnsAsync((UserTask?)null);

            // Act
            var result = await _taskService.DeleteAsync(taskId, userId);

            // Assert
            Assert.False(result);
            _taskRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<UserTask>()), Times.Never);
        }
    }
}