using Tasker.BLL.Models.User;
using Tasker.DAL.Entities;

namespace Tasker.BLL.Mappings
{
    public static class UserMappingExtentions
    {
        public static TaskerUser ToEntity(this UserCreateModel model)
        {
            return new TaskerUser
            {
                UserName = model.UserName,
                Email = model.Email,
            };
        }
    }
}