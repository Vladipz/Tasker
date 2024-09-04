using ErrorOr;

using Tasker.BLL.Models;

namespace Tasker.BLL.Interfaces
{
    public interface IToDoTaskService
    {
        Task<ErrorOr<TodoTaskModel>> CreateAsync(TodoTaskCreateModel model, Guid userId);

        Task<ErrorOr<TodoTaskModel>> ReadAsync(Guid id, Guid userId);

        Task<IEnumerable<TodoTaskModel>> ReadAllAsync(Guid userId);

        Task<ErrorOr<Updated>> UpdateAsync(TodoTaskCreateModel model, Guid userId, Guid id);

        Task<ErrorOr<Deleted>> DeleteAsync(Guid id, Guid userId);
    }
}