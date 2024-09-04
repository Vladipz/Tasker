using ErrorOr;

using Microsoft.AspNetCore.Identity;

namespace Tasker.BLL.Mappings
{
    public static class IdentityResultMappingExtensions
    {
        public static ErrorOr<TResult> ToValidationError<TResult>(this IdentityResult result)
        {
            // HACK: Перетворюємо помилки в словник
            // Якщо операція неуспішна, перетворюємо помилки в словник
            var errorDictionary = result.Errors.ToDictionary(e => e.Code, e => e.Description);
            var errors = errorDictionary.Select(e => Error.Validation(e.Key, e.Value));
            return errors.ToList();
        }
    }
}