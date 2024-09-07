using Microsoft.EntityFrameworkCore;

using Tasker.DAL.Data;
using Tasker.DAL.Entities;
using Tasker.DAL.Interfaces;

namespace Tasker.DAL.Repositories
{
    public class ToDoTaskRepository : IToDoTaskRepository
    {
        private readonly TaskerDbContext _context;

        public ToDoTaskRepository(TaskerDbContext context)
        {
            _context = context;
        }

        public async Task AddTaskAsync(TodoTask task)
        {
            await _context.Tasks.AddAsync(task);
        }

        public void DeleteTask(TodoTask task)
        {
            _context.Tasks.Remove(task);
        }

        public IQueryable<TodoTask> GetInitialQuery()
        {
            return _context.Tasks.AsQueryable();
        }

        public async Task<TodoTask> GetTaskAsync(Guid id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<IEnumerable<TodoTask>> GetTasksByQueryAsync(IQueryable<TodoTask> taskQuery)
        {
            return await taskQuery.ToListAsync();
        }

        public void UpdateTask(TodoTask task)
        {
            _context.Tasks.Update(task);
        }
    }
}