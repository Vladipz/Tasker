using ErrorOr;

using FluentValidation;

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
        private readonly IValidator<TodoTaskCreateModel> _createToDoTaskValidator;

        public ToDoTaskService(IUnitOfWork unitOfWork, IValidator<TodoTaskCreateModel> createToDoTaskValidator)
        {
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
            return Result.Deleted;
        }

        public async Task<IEnumerable<TodoTaskModel>> ReadAllAsync(Guid userId)
        {
            var tasks = await _unitOfWork.TodoTaskRepository.GetTasksByUserIdAsync(userId);
            return tasks.Select(t => t.ToModel()).ToList();
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
