using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssignment.Dal.Filters;
using TestAssignment.DAL.Entities;
using TestAssignment.DAL.Interfaces;

namespace TestAssignment.DAL.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserTask task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserTask>> GetTasksAsync(Guid userId, TaskFilter filter)
        {
            var query = _context.Tasks.AsQueryable().Where(t => t.UserId == userId);

            if (filter.Status.HasValue)
            {
                query = query.Where(t => t.Status == filter.Status.Value);
            }

            if (filter.DueDate.HasValue)
            {
                query = query.Where(t => t.DueDate == filter.DueDate.Value);
            }

            if (filter.Priority.HasValue)
            {
                query = query.Where(t => t.Priority == filter.Priority.Value);
            }

            query = query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);

            return await query.ToListAsync();
        }
        public async Task<UserTask?> GetTaskByIdAsync(Guid userId, Guid taskId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId && t.Id == taskId)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(UserTask task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserTask task)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
