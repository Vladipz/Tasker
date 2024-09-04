using Tasker.DAL.Interfaces;
using Tasker.DAL.Repositories;

namespace Tasker.DAL.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskerDbContext _context;
        private IToDoTaskRepository _toDoTaskRepository;

        public UnitOfWork(TaskerDbContext context)
        {
            _context = context;
        }

        public IToDoTaskRepository TodoTaskRepository => _toDoTaskRepository ??= new ToDoTaskRepository(_context);

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}