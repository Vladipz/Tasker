using FluentValidation;

using Tasker.BLL.Models;

namespace Tasker.BLL.Validators.Shared
{
    public class PaginationValidator : AbstractValidator<PaginationParameters>
    {
        public PaginationValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .GreaterThan(0);
        }
    }
}