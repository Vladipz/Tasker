using Tasker.API.Contracts.Requests;
using Tasker.API.Helpers;
using Tasker.API.Mapping;
using Tasker.BLL.Interfaces;

namespace Tasker.API.Endpoints
{
    public static class TaskEndpoint
    {
        public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/tasks").RequireAuthorization();

            group.MapGet("", async (IToDoTaskService toDoTaskService, HttpContext context) =>
            {
                var userId = context.GetUserId();

                var result = await toDoTaskService.ReadAllAsync(userId);
                var response = result.ToAllTasksResponse();

                return Results.Ok(response);
            });

            group.MapPost("", async (TaskRequest request, IToDoTaskService toDoTaskService, HttpContext context) =>
            {
                var userId = context.GetUserId();

                var result = await toDoTaskService.CreateAsync(request.ToCreateModel(), userId);
                return result.Match(
                    task => Results.Created($"/api/tasks/{task.Id}", task.ToResponse()),
                    error => error.ToResponse());
            });

            group.MapGet("/{id}", async (Guid id, IToDoTaskService toDoTaskService, HttpContext context) =>
            {
                var userId = context.GetUserId();

                var result = await toDoTaskService.ReadAsync(id, userId);
                return result.Match(
                    task => Results.Ok(task.ToResponse()),
                    error => error.ToResponse());
            });

            group.MapPut("/{id}", async (Guid id, TaskRequest request, IToDoTaskService toDoTaskService, HttpContext context) =>
            {
                var userId = context.GetUserId();

                var result = await toDoTaskService.UpdateAsync(request.ToCreateModel(), userId, id);
                return result.Match(
                    task => Results.NoContent(),
                    error => error.ToResponse());
            });

            group.MapDelete("/{id}", async (Guid id, IToDoTaskService toDoTaskService, HttpContext context) =>
            {
                var userId = context.GetUserId();

                var result = await toDoTaskService.DeleteAsync(id, userId);
                return result.Match(
                    deleted => Results.NoContent(),
                    error => error.ToResponse());
            });
        }
    }
}