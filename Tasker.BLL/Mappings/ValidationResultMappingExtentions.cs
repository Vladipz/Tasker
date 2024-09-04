using ErrorOr;

using FluentValidation.Results;

namespace Tasker.BLL.Mappings
{
    public static class ValidationResultMappingExtentions
    {
        public static ErrorOr<TResult> ToValidationError<TResult>(this ValidationResult validationResult)
        {
            return validationResult.Errors.ConvertAll(vf => Error.Validation(vf.PropertyName, vf.ErrorMessage));
        }
    }
}