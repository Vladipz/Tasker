using ErrorOr;

using Tasker.BLL.Models.User;

namespace Tasker.BLL.Interfaces
{
    public interface ITokenService
    {
        ErrorOr<TokenModel> GenerateToken(TaskerUserModel userModel);
    }
}