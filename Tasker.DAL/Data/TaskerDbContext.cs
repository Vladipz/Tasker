using Microsoft.EntityFrameworkCore;

using Tasker.DAL.Entities;

namespace Tasker.DAL.Data
{
    public class TaskerDbContext : DbContext
    {
        public TaskerDbContext(DbContextOptions<TaskerDbContext> options) : base(options)
        {
        }

        public DbSet<TodoTask> Tasks { get; set; }
    }
}