using Tasker.API.Contracts.Responses;
using Tasker.BLL.Models.User;

namespace Tasker.API.Mapping
{
    public static class TokenMappingExtentions
    {
        public static LoginResponse ToResponse(this TokenModel model)
        {
            return new LoginResponse
            {
                Token = model.Token,
                Expiration = model.Expiration,
            };
        }
    }
}