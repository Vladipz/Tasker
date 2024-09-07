using System.Text.Json.Serialization;

using Tasker.API.Contracts.Enums;

namespace Tasker.API.Contracts.Responses
{
    public class TaskResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskStatusType Status { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskPriorityType Priority { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}