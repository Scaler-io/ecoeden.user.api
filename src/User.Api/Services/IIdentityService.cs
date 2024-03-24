using Ecoeden.User.Domain.Models.Core;

namespace User.Api.Services
{
    public interface IIdentityService
    {
        UserDto PrepareUser();
    }
}
