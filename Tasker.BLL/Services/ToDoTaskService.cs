using System.Linq.Expressions;

using ErrorOr;

using FluentValidation;

using Microsoft.Extensions.Logging;

using Tasker.BLL.Builders;
using Tasker.BLL.Interfaces;
using Tasker.BLL.Mappings;
using Tasker.BLL.Models;
using Tasker.DAL.Entities;
using Tasker.DAL.Interfaces;

namespace Tasker.BLL.Services
{
    public class ToDoTaskService : IToDoTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<PaginationParameters> _pageValidator;
        private readonly ILogger<ToDoTaskService> _logger;
        private readonly IValidator<TodoTaskCreateModel> _createToDoTaskValidator;

        public ToDoTaskService(
            IUnitOfWork unitOfWork,
            IValidator<TodoTaskCreateModel> createToDoTaskValidator,
            IValidator<PaginationParameters> pageValidator,
            ILogger<ToDoTaskService> logger)
        {
            _pageValidator = pageValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _createToDoTaskValidator = createToDoTaskValidator;
        }

        public async Task<ErrorOr<TodoTaskModel>> CreateAsync(TodoTaskCreateModel model, Guid userId)
        {
            var validationResult = await _createToDoTaskValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return validationResult.ToValidationError<TodoTaskModel>();
            }

            var entity = model.ToEntity();

            // TODO: Check if user exists
            entity.UserId = userId;

            await _unitOfWork.TodoTaskRepository.AddTaskAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with Id {id} created", entity.Id);

            return entity.ToModel();
        }

        public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id, Guid userId)
        {
            var task = await _unitOfWork.TodoTaskRepository.GetTaskAsync(id);
            if (task == null)
            {
                return Error.NotFound();
            }

            _unitOfWork.TodoTaskRepository.DeleteTask(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with Id {id} deleted", id);
            return Result.Deleted;
        }

        public async Task<ErrorOr<PagedList<TodoTaskModel>>> ReadAllAsync(
            Guid userId,
            string? priorityQuery,
            string? statusQuery,
            DateTime? dueDate,
            string? sortColumn,
            string? sortOrder,
            int page,
            int pageSize)
        {
            var pageValidationResult = await _pageValidator.ValidateAsync(new PaginationParameters { Page = page, PageSize = pageSize });
            if (!pageValidationResult.IsValid)
            {
                return pageValidationResult.ToValidationError<PagedList<TodoTaskModel>>();
            }

            var initialQuery = _unitOfWork.TodoTaskRepository.GetInitialQuery();

            // create filter and pagination query
            var taskQuery = new TodoTaskQueryBuilder(initialQuery)
                .FilterByStatus(statusQuery)
                .FilterByDueDate(dueDate)
                .FilterByPriority(priorityQuery)
                .FilterByUserId(userId)
                .Build();

            // create sort query
            if (sortOrder?.ToLower() == "desc")
            {
                taskQuery = taskQuery.OrderByDescending(GetSortProperty(sortColumn));
            }
            else
            {
                taskQuery = taskQuery.OrderBy(GetSortProperty(sortColumn));
            }

            // paginate
            var taskQueryResponse = taskQuery.Select(t => t.ToModel());

            return await PagedList<TodoTaskModel>.CreateAsync(taskQueryResponse, page, pageSize);
        }

        private static Expression<Func<TodoTask, object>> GetSortProperty(string? sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "date" => task => task.DueDate,
                "priority" => task => task.Priority,
                _ => task => task.DueDate,
            };
        }

        public async Task<ErrorOr<TodoTaskModel>> ReadAsync(Guid id, Guid userId)
        {
            var task = await _unitOfWork.TodoTaskRepository.GetTaskAsync(id);
            if (task == null)
            {
                return Error.NotFound();
            }

            if (task.UserId != userId)
            {
                return Error.Forbidden();
            }

            return task.ToModel();
        }

        public async Task<ErrorOr<Updated>> UpdateAsync(TodoTaskCreateModel model, Guid userId, Guid id)
        {
            // TODO: change to update model
            var validationResult = await _createToDoTaskValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return validationResult.ToValidationError<Updated>();
            }

            var task = await _unitOfWork.TodoTaskRepository.GetTaskAsync(id);

            if (task == null)
            {
                return Error.NotFound();
            }

            if (task.UserId != userId)
            {
                return Error.Forbidden();
            }

            UpdateTask(task, model.ToEntity());

            _unitOfWork.TodoTaskRepository.UpdateTask(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with Id {id} updated", id);

            return Result.Updated;
        }

        public static void UpdateTask(TodoTask task, TodoTask model)
        {
            task.Title = model.Title;
            task.Description = model.Description;
            task.DueDate = model.DueDate;
            task.Priority = model.Priority;
            task.Status = model.Status;
        }
    }
}