using Tasker.BLL.Models.Enums;

namespace Tasker.BLL.Models
{
    public class TodoTaskCreateModel
    {
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskStatusType Status { get; set; } = TaskStatusType.Pending;

        public TaskPriorityType Priority { get; set; } = TaskPriorityType.None;
    }
}