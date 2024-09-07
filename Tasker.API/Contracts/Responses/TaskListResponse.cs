namespace Tasker.API.Contracts.Responses
{
    public class TaskListResponse
    {
        public int TotalCount { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public IEnumerable<TaskResponse> Tasks { get; set; }
    }
}