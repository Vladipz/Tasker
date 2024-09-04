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
    }
}