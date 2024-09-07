using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssignment.Dal.Filters;
using TestAssignment.DAL.Entities;

namespace TestAssignment.DAL.Interfaces
{
    public interface ITaskRepository
    {
        Task AddAsync(UserTask task);
        Task<IEnumerable<UserTask>> GetTasksAsync(Guid userId, TaskFilter filter);
        Task<UserTask?> GetTaskByIdAsync(Guid userId, Guid taskId);
        Task UpdateAsync(UserTask task);
        Task DeleteAsync(UserTask task);
    }
}
