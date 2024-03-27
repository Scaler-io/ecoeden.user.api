using Ecoeden.User.Application.Contracts.Persistence.Users;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecoeden.User.Infrastructure.Repositories.Users
{
    public sealed class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        private readonly UserDbContext _context;
        public UserRepository(UserDbContext context) : 
            base(context)
        {
            _context = context;
        }

        public async Task<Result<ApplicationUser>> GetUserById(string id)
        {
            var result = await _context.Users
                .Include("UserRoles.Role.RolePermissions.Permission")
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return Result<ApplicationUser>.Success(result);
        }
    }
}
