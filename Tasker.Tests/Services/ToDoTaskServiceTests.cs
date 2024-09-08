using ErrorOr;

using FluentAssertions;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Tasker.BLL.Interfaces;
using Tasker.BLL.Mappings;
using Tasker.BLL.Models;
using Tasker.BLL.Services;
using Tasker.DAL.Data;
using Tasker.DAL.Entities;
using Tasker.DAL.Interfaces;

namespace Tasker.Tests.Services
{
    public class ToDoTaskServiceTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IValidator<TodoTaskCreateModel> _createTaskValidatorMock;
        private readonly IValidator<PaginationParameters> _pageValidatorMock;
        private readonly ILogger<ToDoTaskService> _loggerMock;
        private readonly IToDoTaskRepository _mockTodoTaskRepository;
        private readonly IToDoTaskService _taskService;
        private readonly DbContextOptions<TaskerDbContext> _dbContextOptions;

        public ToDoTaskServiceTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _mockTodoTaskRepository = Substitute.For<IToDoTaskRepository>();
            _unitOfWorkMock.TodoTaskRepository.Returns(_mockTodoTaskRepository);
            _createTaskValidatorMock = Substitute.For<IValidator<TodoTaskCreateModel>>();
            _pageValidatorMock = Substitute.For<IValidator<PaginationParameters>>();
            _loggerMock = Substitute.For<ILogger<ToDoTaskService>>();

            _taskService = new ToDoTaskService(
                _unitOfWorkMock,
                _createTaskValidatorMock,
                _pageValidatorMock,
                _loggerMock);

            _dbContextOptions = new DbContextOptionsBuilder<TaskerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Унікальна база даних для кожного тесту
            .Options;
        }

        [Fact]
        public async Task CreateAsync_WhenModelIsInvalid_ShouldReturnValidationError()
        {
            // Arrange
            var model = GetTaskCreateModel();
            var userId = Guid.NewGuid();

            _createTaskValidatorMock
                       .ValidateAsync(model)
                       .Returns(Task.FromResult(new ValidationResult([new ValidationFailure()])));

            // Act
            var result = await _taskService.CreateAsync(model, userId);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().HaveCount(1).And.Satisfy(e => e.Type == ErrorType.Validation);
        }

        [Fact]
        public async Task CreateAsync_WhenModelIsValid_ShouldCreateTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var model = GetTaskCreateModel();
            var userId = Guid.NewGuid();
            TodoTask createdTask = null!;
            var expectedEntity = GetExpectedTaskFromModel(taskId, model);

            _createTaskValidatorMock
                       .ValidateAsync(model)
                       .Returns(Task.FromResult(new ValidationResult()));
            await _mockTodoTaskRepository.AddTaskAsync(Arg.Do<TodoTask>(t => createdTask = t));
            _unitOfWorkMock.When(uofw => uofw.SaveChangesAsync()).Do(_ => { createdTask.Id = taskId; });

            // Act
            var result = await _taskService.CreateAsync(model, userId);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(expectedEntity);
        }

        [Fact]
        public async Task DeleteAsync_WhenTaskExists_ShouldDeleteTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingTask = new TodoTask { Id = taskId, UserId = userId };

            _mockTodoTaskRepository
                .GetTaskAsync(taskId)
                .Returns(Task.FromResult(existingTask));

            // Act
            var result = await _taskService.DeleteAsync(taskId, userId);

            // Assert
            result.IsError.Should().BeFalse();
            _mockTodoTaskRepository.Received().DeleteTask(existingTask);
            await _unitOfWorkMock.Received().SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteAsync_WhenTaskDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockTodoTaskRepository
                .GetTaskAsync(taskId)
                .Returns(Task.FromResult<TodoTask?>(null));

            // Act
            var result = await _taskService.DeleteAsync(taskId, userId);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Type == ErrorType.NotFound);
        }

        [Fact]
        public async Task ReadAsync_WhenTaskExists_ShouldReturnTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            TodoTask task = GetTaskEntity(taskId, userId);
            TodoTaskModel expectedModel = task.ToModel();
            _mockTodoTaskRepository
                .GetTaskAsync(taskId)
                .Returns(Task.FromResult(task));

            // Act
            var result = await _taskService.ReadAsync(taskId, userId);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(expectedModel);
        }

        [Fact]
        public async Task ReadAsync_WhenTaskExistsButBelongsToDifferentUser_ShouldReturnNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var task = GetTaskEntity(taskId, Guid.NewGuid());

            _mockTodoTaskRepository
                .GetTaskAsync(taskId)
                .Returns(Task.FromResult(task));

            // Act
            var result = await _taskService.ReadAsync(taskId, userId);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Type == ErrorType.Forbidden);
        }

        [Fact]
        public async Task UpdateAsync_WhenModelIsInvalid_ShouldReturnValidationError()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var model = GetTaskCreateModel();
            var userId = Guid.NewGuid();

            _createTaskValidatorMock
                .ValidateAsync(model)
                .Returns(Task.FromResult(new ValidationResult([new ValidationFailure()])));

            // Act
            var result = await _taskService.UpdateAsync(model, taskId, userId);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().HaveCount(1).And.Satisfy(e => e.Type == ErrorType.Validation);
        }

        [Fact]
        public async Task UpdateAsync_WhenTaskDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var model = GetTaskCreateModel();

            _createTaskValidatorMock
                .ValidateAsync(model)
                .Returns(Task.FromResult(new ValidationResult()));

            _mockTodoTaskRepository
                .GetTaskAsync(taskId)
                .Returns(Task.FromResult<TodoTask?>(null));

            // Act
            var result = await _taskService.UpdateAsync(model, userId, taskId);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Type == ErrorType.NotFound);
        }

        [Fact]
        public async Task UpdateAsync_WhenTaskExistsButBelongsToDifferentUser_ShouldReturnForbidden()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var model = GetTaskCreateModel();
            var task = GetTaskEntity(taskId, Guid.NewGuid());

            _createTaskValidatorMock
                .ValidateAsync(model)
                .Returns(Task.FromResult(new ValidationResult()));

            _mockTodoTaskRepository
                .GetTaskAsync(taskId)
                .Returns(Task.FromResult(task));

            // Act
            var result = await _taskService.UpdateAsync(model, userId, taskId);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Type == ErrorType.Forbidden);
        }

        [Fact]
        public async Task UpdateAsync_WhenValidRequest_ShouldUpdateTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var model = GetTaskCreateModel();
            var task = GetTaskEntity(taskId, userId);

            _createTaskValidatorMock
                .ValidateAsync(model)
                .Returns(Task.FromResult(new ValidationResult()));

            _mockTodoTaskRepository
                .GetTaskAsync(taskId)
                .Returns(Task.FromResult(task));

            // Act
            var result = await _taskService.UpdateAsync(model, userId, taskId);

            // Assert
            result.IsError.Should().BeFalse();
            _mockTodoTaskRepository.Received().UpdateTask(Arg.Is<TodoTask>(t => t.Id == taskId));
            await _unitOfWorkMock.Received().SaveChangesAsync();
        }

        private TodoTask GetTaskEntity(Guid taskId, Guid userId)
        {
            return new TodoTask
            {
                Id = taskId,
                UserId = userId,
                Title = "Title",
                Description = "Description",
                DueDate = new DateTime(2022, 1, 1),
                Priority = DAL.Entities.Enums.TaskPriorityType.Low,
                Status = DAL.Entities.Enums.TaskStatusType.Pending,
            };
        }

        [Fact]
        public async Task ReadAsync_WhenTaskDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockTodoTaskRepository
                .GetTaskAsync(taskId)
                .Returns(Task.FromResult<TodoTask?>(null));

            // Act
            var result = await _taskService.ReadAsync(taskId, userId);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Type == ErrorType.NotFound);
        }

        private object GetExpectedTaskFromModel(Guid taskId, TodoTaskCreateModel model)
        {
            return new TodoTaskModel
            {
                Id = taskId,
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Priority = model.Priority,
                Status = model.Status,
            };
        }

        private static TodoTaskCreateModel GetTaskCreateModel()
        {
            return new TodoTaskCreateModel
            {
                Title = "Title",
                Description = "Description",
                DueDate = new DateTime(2022, 1, 1),
                Priority = BLL.Models.Enums.TaskPriorityType.Low,
                Status = BLL.Models.Enums.TaskStatusType.Pending,
            };
        }
    }
}