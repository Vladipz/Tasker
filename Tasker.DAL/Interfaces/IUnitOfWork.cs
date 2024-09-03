namespace Tasker.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IToDoTaskRepository TaskRepository { get; }

        Task<int> SaveChangesAsync();
    }
}