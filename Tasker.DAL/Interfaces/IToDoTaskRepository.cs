using Tasker.DAL.Entities;

namespace Tasker.DAL.Interfaces
{
    public interface IToDoTaskRepository
    {
        Task<IEnumerable<TodoTask>> GetTasksAsync(Guid userId);

        Task<TodoTask> GetTaskAsync(Guid id);

        Task AddTaskAsync(TodoTask task);

        void UpdateTask(TodoTask task);

        void DeleteTask(TodoTask task);
    }
}