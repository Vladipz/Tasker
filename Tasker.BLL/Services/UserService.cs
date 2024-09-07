using ErrorOr;

using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Tasker.BLL.Interfaces;
using Tasker.BLL.Mappings;
using Tasker.BLL.Models.User;
using Tasker.DAL.Entities;

namespace Tasker.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<TaskerUser> _userManager;
        private readonly IValidator<UserCreateModel> _validator;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<TaskerUser> userManager, IValidator<UserCreateModel> validator, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _validator = validator;
            _logger = logger;
        }

        public async Task<ErrorOr<TaskerUserModel>> CheckPasswordAsync(TaskerUserModel userModel, string password)
        {
            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(userModel.ToEntity(), password);

            if (!isPasswordCorrect)
            {
                return Error.Unauthorized();
            }

            return userModel;
        }

        public async Task<ErrorOr<TaskerUserModel>> ReadAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Error.NotFound();
            }

            return user.ToModel();
        }

        public async Task<ErrorOr<Created>> RegisterAsync(UserCreateModel model)
        {
            var validationresult = await _validator.ValidateAsync(model);
            if (!validationresult.IsValid)
            {
                return validationresult.ToValidationError<Created>();
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return Error.Conflict();
            }

            var user = model.ToEntity();

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return result.ToValidationError<Created>();
            }

            _logger.LogInformation("User {user.UserName} created a new account", user.UserName);
            return Result.Created;
        }
    }
}