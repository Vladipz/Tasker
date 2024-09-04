using Tasker.API.Contracts.Requests;
using Tasker.BLL.Models.User;

namespace Tasker.API.Mapping
{
    public static class UserMappingExtentions
    {
        public static UserCreateModel ToCreateModel(this RegistrationRequest request)
        {
            return new UserCreateModel
            {
                UserName = request.Username,
                Email = request.Email,
                Password = request.Password
            };
        }
    }
}