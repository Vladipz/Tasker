using Tasker.API.Contracts.Requests;
using Tasker.API.Mapping;
using Tasker.BLL.Interfaces;

namespace Tasker.API.Endpoints
{
    public static class AuthenticationEndpoint
    {
        public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/auth");

            group.MapPost("/register", async (RegistrationRequest request, IUserService userService) =>
            {
                var result = await userService.RegisterAsync(request.ToCreateModel());
                return result.Match(
                    user => Results.Created(),
                    error => error.ToResponse());
            });
        }
    }
}