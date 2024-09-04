namespace Tasker.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IToDoTaskRepository TodoTaskRepository { get; }

        Task<int> SaveChangesAsync();
    }
}