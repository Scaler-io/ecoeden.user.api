using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecoeden.User.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            return await _userManager.Users
                .Include("UserRoles.Role.RolePermissions.Permission")
                .ToListAsync();
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _userManager.Users
                .Include("UserRoles.Role.RolePermissions.Permission")
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddToRoleAsync(ApplicationUser user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        {
           return (await _userManager.AddToRolesAsync(user, roles)).Succeeded;
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string role)
        {
            await _userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task<bool> IsInRole(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> CreateUserAsync(ApplicationUser user, string password)
        {
            return (await _userManager.CreateAsync(user, password)).Succeeded;
        }

        public async Task<bool> UserNameExistsAsync(string name)
        {
            return await _userManager.FindByNameAsync(name) is not null;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        public async Task<bool> UpdateUser(ApplicationUser user)
        {
            return (await _userManager.UpdateAsync(user)).Succeeded;
        }
    }
}
