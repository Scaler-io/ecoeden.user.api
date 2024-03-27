using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Factory;
using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Infrastructure.Factory
{
    public sealed class CacheServiceFactory : ICacheServiceFactory
    {
        private readonly IEnumerable<ICacheService> _cacheServices;

        public CacheServiceFactory(IEnumerable<ICacheService> cacheServices)
        {
            _cacheServices = cacheServices;
        }

        public ICacheService GetService(CahceServiceTypes type)
        {
            var service = _cacheServices.Where(s => s.ServiceType == type).FirstOrDefault();
            return service is null ? throw new ArgumentNullException("No cache service implementation was found of type") : service;
        }
    }
}
