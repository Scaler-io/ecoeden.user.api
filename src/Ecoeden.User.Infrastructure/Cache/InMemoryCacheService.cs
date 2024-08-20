using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Domain.Models.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using User.Domain.ConfigurationOptions.App;

namespace Ecoeden.User.Infrastructure.Cache
{
    public sealed class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly AppOption _appOption;

        public InMemoryCacheService(IMemoryCache memoryCache, IOptions<AppOption> options)
        {
            _memoryCache = memoryCache;
            _appOption = options.Value;
        }

        public CahceServiceTypes ServiceType => CahceServiceTypes.InMemory;

        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }
        public void Set<T>(string key, T value, int? expirationTime)
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(45),
                AbsoluteExpirationRelativeToNow = expirationTime.HasValue
                    ? TimeSpan.FromSeconds(expirationTime.Value)
                    : TimeSpan.FromSeconds(_appOption.CacheExpiration)
            }); ;
        }

        public bool Contains(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
