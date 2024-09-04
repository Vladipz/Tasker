using Tasker.BLL.Interfaces;
using Tasker.BLL.Models.Enums;

namespace Tasker.BLL.Models
{
    public class TodoTaskModel : IDatedEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskStatusType Status { get; set; } = TaskStatusType.Pending;

        public TaskPriorityType Priority { get; set; } = TaskPriorityType.None;

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}