using Ecoeden.User.Domain.Entities;

namespace Ecoeden.User.Application.Contracts.Data.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetUserById(string id);
        Task<IEnumerable<ApplicationUser>> GetAllUsers();
        Task<bool> CreateUserAsync(ApplicationUser user, string password);
        Task AddToRoleAsync(ApplicationUser user, string role);
        Task<bool> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles);
        Task RemoveFromRoleAsync(ApplicationUser user, string role);
        Task<bool> IsInRole(ApplicationUser user, string role);
        Task<bool> UserNameExistsAsync(string name);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UpdateUser(ApplicationUser user);
    }
}
