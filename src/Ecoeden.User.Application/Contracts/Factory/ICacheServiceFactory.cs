using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Application.Contracts.Factory
{
    public interface ICacheServiceFactory
    {
        ICacheService GetService(CahceServiceTypes type);
    }
}
