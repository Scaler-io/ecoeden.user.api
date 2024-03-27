using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;

namespace Ecoeden.User.Application.Contracts.Persistence.Users
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<Result<ApplicationUser>> GetUserById(string id);
    }
}
