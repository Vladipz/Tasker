using Tasker.DAL.Entities.Enums;
using Tasker.DAL.Interfaces;

namespace Tasker.DAL.Entities
{
    public class TodoTask : IDatedEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskStatusType Status { get; set; } = TaskStatusType.Pending;

        public TaskPriorityType Priority { get; set; } = TaskPriorityType.None;

        public Guid UserId { get; set; }

        public TaskerUser User { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}