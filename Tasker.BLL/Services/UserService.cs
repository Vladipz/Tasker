using ErrorOr;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

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

        public UserService(UserManager<TaskerUser> userManager, IValidator<UserCreateModel> validator)
        {
            _userManager = userManager;
            _validator = validator;
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

            return Result.Created;
        }
    }
}