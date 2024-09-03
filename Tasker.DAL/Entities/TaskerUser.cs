using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

namespace Tasker.DAL.Entities
{
    public class TaskerUser : IdentityUser<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override Guid Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        public IEnumerable<TodoTask> Tasks { get; set; } = default!;
    }
}