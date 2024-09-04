using FluentValidation;

using Tasker.BLL.Models;

namespace Tasker.BLL.Validators.ToDoTask
{
    public class CreateToDoTaskValidator : AbstractValidator<TodoTaskCreateModel>
    {
        public CreateToDoTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            // DueDate should be in the future(my opinion)
            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow);

            // TODO: Add a message to the DueDate rule
            RuleFor(x => x.Status)
                .IsInEnum();

            RuleFor(x => x.Priority)
                .IsInEnum();
        }
    }
}
