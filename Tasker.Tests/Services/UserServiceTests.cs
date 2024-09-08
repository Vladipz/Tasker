using ErrorOr;

using FluentAssertions;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Tasker.BLL.Interfaces;
using Tasker.BLL.Mappings;
using Tasker.BLL.Models.User;
using Tasker.BLL.Services;
using Tasker.DAL.Entities;
namespace Tasker.BLL.Tests.Services
{
    public class UserServiceTests
    {
        private readonly UserManager<TaskerUser> _userManagerMock;
        private readonly IValidator<UserCreateModel> _validatorMock;
        private readonly ILogger<UserService> _loggerMock;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _userManagerMock = Substitute.For<UserManager<TaskerUser>>(
                Substitute.For<IUserStore<TaskerUser>>(), null, null, null, null, null, null, null, null);
            _validatorMock = Substitute.For<IValidator<UserCreateModel>>();
            _loggerMock = Substitute.For<ILogger<UserService>>();
            _userService = new UserService(_userManagerMock, _validatorMock, _loggerMock);
        }

        [Fact]
        public async Task CheckPasswordAsync_WhenPasswordIsIncorrect_ReturnsUnauthorizedError()
        {
            // Arrange
            var userModel = new TaskerUserModel { Id = Guid.NewGuid(), UserName = "testuser" };
            var password = "wrongpassword";
            var taskerUser = userModel.ToEntity();

            _userManagerMock.CheckPasswordAsync(taskerUser, password).Returns(false);

            // Act
            var result = await _userService.CheckPasswordAsync(userModel, password);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Unauthorized);
        }

        [Fact]
        public async Task ReadAsync_WhenUserExists_ReturnsUserModel()
        {
            // Arrange
            var userName = "testuser";
            var taskerUser = new TaskerUser { Id = Guid.NewGuid(), UserName = userName };

            _userManagerMock.FindByNameAsync(userName).Returns(taskerUser);

            // Act
            var result = await _userService.ReadAsync(userName);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.UserName.Should().Be(userName);
        }

        [Fact]
        public async Task ReadAsync_WhenUserDoesNotExist_ReturnsNotFoundError()
        {
            // Arrange
            var userName = "nonexistentuser";

            _userManagerMock.FindByNameAsync(userName).Returns((TaskerUser)null);

            // Act
            var result = await _userService.ReadAsync(userName);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task RegisterAsync_WhenModelIsValid_CreatesUserAndReturnsCreated()
        {
            // Arrange
            var model = new UserCreateModel
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "Pa$$w0rd!",
            };

            _validatorMock.ValidateAsync(model).Returns(new ValidationResult());
            _userManagerMock.FindByEmailAsync(model.Email).Returns((TaskerUser)null);
            _userManagerMock.CreateAsync(Arg.Any<TaskerUser>(), model.Password).Returns(IdentityResult.Success);

            // Act
            var result = await _userService.RegisterAsync(model);

            // Assert
            result.IsError.Should().BeFalse();
            await _userManagerMock.Received(1).CreateAsync(Arg.Any<TaskerUser>(), model.Password);
        }

        [Fact]
        public async Task RegisterAsync_WhenModelIsInvalid_ReturnsValidationErrors()
        {
            // Arrange
            var model = new UserCreateModel { UserName = "newuser" };
            var validationResult = new ValidationResult(new[] { new ValidationFailure("Email", "Email is required") });

            _validatorMock.ValidateAsync(model).Returns(validationResult);

            // Act
            var result = await _userService.RegisterAsync(model);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public async Task RegisterAsync_WhenUserAlreadyExists_ReturnsConflictError()
        {
            // Arrange
            var model = new UserCreateModel
            {
                UserName = "existinguser",
                Email = "existinguser@example.com",
                Password = "Password123"
            };

            var existingUser = new TaskerUser { Email = model.Email };

            _validatorMock.ValidateAsync(model).Returns(new ValidationResult());
            _userManagerMock.FindByEmailAsync(model.Email).Returns(existingUser);

            // Act
            var result = await _userService.RegisterAsync(model);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Conflict);
        }
    }
}