using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssignment.BLL.Models;
using TestAssignment.Dal.Filters;

namespace TestAssignment.BLL.Interfaces
{
    public interface ITaskService
    {
        Task<UserTaskDto> CreateAsync(CreateTaskDto createTaskDto, Guid userId);
        Task<IEnumerable<UserTaskDto>> GetTasksAsync(Guid userId, TaskFilter filter);
        Task<UserTaskDto?> GetTaskByIdAsync(Guid userId, Guid taskId);
        Task<bool> UpdateTaskAsync(Guid userId, Guid taskId, UpdateTaskDto updateTaskDto);
        Task<bool> DeleteAsync(Guid taskId, Guid userId);
    }
}
