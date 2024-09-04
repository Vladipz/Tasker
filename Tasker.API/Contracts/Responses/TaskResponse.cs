using Tasker.API.Contracts.Enums;

namespace Tasker.API.Contracts.Responses
{
    public class TaskResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskStatusType Status { get; set; }

        public TaskPriorityType Priority { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}