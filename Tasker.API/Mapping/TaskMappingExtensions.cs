using Tasker.API.Contracts.Enums;
using Tasker.API.Contracts.Requests;
using Tasker.API.Contracts.Responses;
using Tasker.BLL.Models;

namespace Tasker.API.Mapping
{
    public static class TaskMappingExtensions
    {
        public static TaskResponse ToResponse(this TodoTaskModel task)
        {
            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Priority = (TaskPriorityType)task.Priority,
                Status = (TaskStatusType)task.Status,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
            };
        }

        public static TodoTaskCreateModel ToCreateModel(this TaskRequest task)
        {
            return new TodoTaskCreateModel
            {
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = (BLL.Models.Enums.TaskPriorityType)task.Priority,
                Status = (BLL.Models.Enums.TaskStatusType)task.Status,
            };
        }
    }
}