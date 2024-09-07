using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssignment.BLL.Interfaces;
using TestAssignment.BLL.Models;
using TestAssignment.Dal.Filters;
using TestAssignment.DAL.Entities;
using TestAssignment.DAL.Enums;
using TestAssignment.DAL.Interfaces;

namespace TestAssignment.BLL.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public TaskService(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<UserTaskDto> CreateAsync(CreateTaskDto createTaskDto, Guid userId)
        {
            var task = _mapper.Map<UserTask>(createTaskDto);
            task.UserId = userId;
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.AddAsync(task);

            return _mapper.Map<UserTaskDto>(task);
        }

        public async Task<IEnumerable<UserTaskDto>> GetTasksAsync(Guid userId, TaskFilter filter)
        {
            var tasks = await _taskRepository.GetTasksAsync(userId, filter);
            return _mapper.Map<IEnumerable<UserTaskDto>>(tasks);
        }

        public async Task<UserTaskDto?> GetTaskByIdAsync(Guid userId, Guid taskId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(userId, taskId);
            return _mapper.Map<UserTaskDto>(task);
        }

        public async Task<bool> UpdateTaskAsync(Guid userId, Guid taskId, UpdateTaskDto updateTaskDto)
        {
            var task = await _taskRepository.GetTaskByIdAsync(userId, taskId);
            if (task == null)
            {
                return false;
            }

            _mapper.Map(updateTaskDto, task);
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid taskId, Guid userId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(userId, taskId);

            if (task == null)
            {
                return false;
            }

            await _taskRepository.DeleteAsync(task);
            return true;
        }
    }
}
