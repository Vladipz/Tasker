using Tasker.DAL.Entities;
using Tasker.DAL.Entities.Enums;

namespace Tasker.BLL.Builders
{
    public class TodoTaskQueryBuilder
    {
        private IQueryable<TodoTask> _query;

        public TodoTaskQueryBuilder(IQueryable<TodoTask> query)
        {
            _query = query;
        }

        public TodoTaskQueryBuilder FilterByPriority(string? priority)
        {
            if (!string.IsNullOrEmpty(priority) && Enum.TryParse<TaskPriorityType>(priority, true, out var taskPriorityType))
            {
                _query = _query.Where(t => t.Priority == taskPriorityType);
            }

            return this;
        }

        public TodoTaskQueryBuilder FilterByStatus(string? status)
        {
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TaskStatusType>(status, true, out var taskStatusType))
            {
                _query = _query.Where(t => t.Status == taskStatusType);
            }

            return this;
        }

        public TodoTaskQueryBuilder FilterByDueDate(DateTime? dueDate)
        {
            if (dueDate.HasValue)
            {
                var date = dueDate.Value.Date; // Остання дата без часу
                _query = _query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == date);
            }

            return this;
        }

        public IQueryable<TodoTask> Build()
        {
            return _query;
        }

        public TodoTaskQueryBuilder FilterByUserId(Guid userId)
        {
            _query = _query.Where(t => t.UserId == userId);
            return this;
        }

        public TodoTaskQueryBuilder Paginate(int page, int pageSize)
        {
            _query = _query.Skip((page - 1) * pageSize).Take(pageSize);
            return this;
        }
    }
}