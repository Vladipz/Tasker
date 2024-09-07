using Tasker.API.Contracts.Responses;
using Tasker.BLL.Models;

namespace Tasker.API.Mapping
{
    public static class ListMappingExtensions
    {
        public static IEnumerable<TaskResponse> ToAllTasksResponse(this IEnumerable<TodoTaskModel> tasks)
        {
            return tasks.Select(t => t.ToResponse());
        }

        public static TaskListResponse ToResponse(this PagedList<TodoTaskModel> tasks)
        {
            return new TaskListResponse
            {
                Tasks = tasks.Items.ToAllTasksResponse(),
                TotalCount = tasks.TotalCount,
                Page = tasks.Page,
                PageSize = tasks.PageSize,
                HasNextPage = tasks.HasNext,
                HasPreviousPage = tasks.HasPrevious,
            };
        }
    }
}