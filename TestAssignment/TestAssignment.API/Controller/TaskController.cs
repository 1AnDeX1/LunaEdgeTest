using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using TestAssignment.BLL.Interfaces;
using TestAssignment.BLL.Models;
using TestAssignment.Dal.Filters;

namespace TestAssignment.API.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized("User is not authenticated");
            }

            var task = await _taskService.CreateAsync(createTaskDto, Guid.Parse(userId));

            return Ok(task);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] TaskFilter filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Invalid user ID.");
            }

            var tasks = await _taskService.GetTasksAsync(parsedUserId, filter);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Invalid user ID.");
            }

            var task = await _taskService.GetTaskByIdAsync(parsedUserId, id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Invalid user ID.");
            }

            var success = await _taskService.UpdateTaskAsync(parsedUserId, id, updateTaskDto);

            if (!success)
            {
                return NotFound("Task not found.");
            }

            return Ok("Task updated successfully");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the authenticated user's ID
            if (userId == null)
            {
                return Unauthorized();
            }

            var isDeleted = await _taskService.DeleteAsync(id, Guid.Parse(userId));

            if (!isDeleted)
            {
                return NotFound("Task not found or not owned by the user");
            }

            return Ok("Task deleted successfully");
        }
    }
}
