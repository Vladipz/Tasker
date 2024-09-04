using ErrorOr;

using Tasker.BLL.Models.User;

namespace Tasker.BLL.Interfaces
{
    public interface IUserService
    {
        Task<ErrorOr<Created>> RegisterAsync(UserCreateModel model);
        // Task<ErrorOr<>> LoginAsync(UserLoginModel model);
    }
}