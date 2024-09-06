using Tasker.BLL.Interfaces;

namespace Tasker.BLL.Models.User
{
    public class TaskerUserModel : IDatedEntity
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public string PasswordHash { get; internal set; }
    }
}