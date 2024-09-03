using Tasker.DAL.Entities.Enums;

namespace Tasker.DAL.Entities
{
    public class TodoTask
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskStatusType Status { get; set; } = TaskStatusType.Pending;

        public TaskPriorityType Priority { get; set; } = TaskPriorityType.None;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}