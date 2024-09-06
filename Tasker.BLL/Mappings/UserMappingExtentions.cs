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

        public static TaskerUser ToEntity(this TaskerUserModel model)
        {
            return new TaskerUser
            {
                Id = model.Id,
                UserName = model.UserName,
                Email = model.Email,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
                PasswordHash = model.PasswordHash,
            };
        }

        public static TaskerUserModel ToModel(this TaskerUser entity)
        {
            return new TaskerUserModel
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Email = entity.Email,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                PasswordHash = entity.PasswordHash,
            };
        }
    }
}