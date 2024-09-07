using ErrorOr;

using Tasker.BLL.Models;

namespace Tasker.BLL.Interfaces
{
    public interface IToDoTaskService
    {
        Task<ErrorOr<TodoTaskModel>> CreateAsync(TodoTaskCreateModel model, Guid userId);

        Task<ErrorOr<TodoTaskModel>> ReadAsync(Guid id, Guid userId);

        Task<ErrorOr<PagedList<TodoTaskModel>>> ReadAllAsync(
            Guid userId,
            string? priorityQuery,
            string? statusQuery,
            DateTime? dueDate,
            string? sortColumn,
            string? sortOrder,
            int page,
            int pageSize);

        Task<ErrorOr<Updated>> UpdateAsync(TodoTaskCreateModel model, Guid userId, Guid id);

        Task<ErrorOr<Deleted>> DeleteAsync(Guid id, Guid userId);
    }
}