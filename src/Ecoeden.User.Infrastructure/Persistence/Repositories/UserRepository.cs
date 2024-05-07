using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Ecoeden.User.Infrastructure.Persistence.Repositories;

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

    public async Task<bool> AddToClaimsAsync(string userName)
    {
        var user = await _userManager.Users
            .Include("UserRoles.Role.RolePermissions.Permission")
            .FirstOrDefaultAsync(x => x.UserName == userName);

        var roles = user.GetUserRoleMappings();
        var permissions = user.GetUserPermissionMappings();

        return (await _userManager.AddClaimsAsync(user, new Claim[]
        {
            new Claim(JwtClaimTypes.Name, user.UserName),
            new Claim(JwtClaimTypes.GivenName, user.FirstName),
            new Claim(JwtClaimTypes.FamilyName, user.Lastname),
            new Claim(JwtClaimTypes.Email, user.Email),
            new Claim(JwtClaimTypes.Role, JsonConvert.SerializeObject(roles)),
            new Claim("Permissions", JsonConvert.SerializeObject(permissions))
        })).Succeeded;
    }

    public async Task<string> GetEmailConfirmationToken(ApplicationUser user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user) ;
    }
}
