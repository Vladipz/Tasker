using Tasker.BLL.Models;
using Tasker.BLL.Models.Enums;
using Tasker.DAL.Entities;

namespace Tasker.BLL.Mappings
{
    public static class ToDoTaskMappingExtentions
    {
        public static TodoTask ToEntity(this TodoTaskCreateModel model)
        {
            return new TodoTask
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Status = (DAL.Entities.Enums.TaskStatusType)model.Status,
                Priority = (DAL.Entities.Enums.TaskPriorityType)model.Priority,
            };
        }

        public static TodoTaskModel ToModel(this TodoTask entity)
        {
            return new TodoTaskModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                DueDate = entity.DueDate,
                Status = (TaskStatusType)entity.Status,
                Priority = (TaskPriorityType)entity.Priority,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }
    }
}