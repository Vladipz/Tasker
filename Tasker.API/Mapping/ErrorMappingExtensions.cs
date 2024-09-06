using ErrorOr;

namespace Tasker.API.Mapping
{
    public static class ErrorMappingExtensions
    {
        public static IResult ToResponse(this Error error)
        {
            return error.Type switch
            {
                ErrorType.Validation => Results.BadRequest(error.Description),
                ErrorType.Unauthorized => Results.Unauthorized(),
                ErrorType.Forbidden => Results.Forbid(),
                ErrorType.NotFound => Results.NotFound(),
                ErrorType.Conflict => Results.Conflict(error.Description),
                ErrorType.Failure or ErrorType.Unexpected or _ => Results.Problem(),
            };
        }

        public static IResult ToResponse(this List<Error> errors)
        {
            return errors.All(e => e.Type == ErrorType.Validation)
                ? errors.ToValidationResponse()
                : errors[0].ToResponse();
        }

        public static IResult ToValidationResponse(this List<Error> errors)
        {
            var dictionary = errors.GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
            return Results.ValidationProblem(dictionary);
        }
    }
}