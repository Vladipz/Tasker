using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Tasker.DAL.Entities;

namespace Tasker.DAL.Data
{
    public class TaskerDbContext : IdentityDbContext<TaskerUser, IdentityRole<Guid>, Guid>
    {
        public TaskerDbContext(DbContextOptions<TaskerDbContext> options)
            : base(options)
        {
        }

        public DbSet<TodoTask> Tasks { get; set; }

        public DbSet<TaskerUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoTask>()
                .Property(t => t.CreatedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<TodoTask>()
                .Property(t => t.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<TodoTask>()
             .HasOne(t => t.User)
             .WithMany(u => u.Tasks)
             .HasForeignKey(t => t.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}