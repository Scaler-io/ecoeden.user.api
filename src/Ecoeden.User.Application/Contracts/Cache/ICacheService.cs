using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Application.Contracts.Cache
{
    public interface ICacheService
    {
        CahceServiceTypes ServiceType { get; }
        T Get<T>(string key);
        void Set<T>(string key, T value, int? expirationTime);
        void Remove(string key);
        bool Contains(string key);
    }
}
