using Ecoeden.User.Application.Contracts.Data;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecoeden.User.Application.Features.Role.Commands.RemoveRole
{
    public sealed class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, Result<bool>>
    {
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbTranscation _dbTransaction;

        public RemoveRoleCommandHandler(ILogger logger, 
            UserManager<ApplicationUser> userManager, 
            IDbTranscation dbTransaction)
        {
            _logger = logger;
            _userManager = userManager;
            _dbTransaction = dbTransaction;
        }

        public async Task<Result<bool>> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.Here().MethodEnterd();

            if(IsUpdatingOwnRole(request.CurrentUser, request.Command.UserId))
            {
                _logger.Here().Error("Updating own role is not allowd");
                return Result<bool>.Failure(ErrorCodes.OperationFailed);
            }

            var user = await _userManager.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(x => x.Id == request.Command.UserId);

            if(user is null)
            {
                _logger.Here().Error("{ErrorCode} - no user entity found with {id}", ErrorCodes.NotFound, request.Command.UserId);
                return Result<bool>.Failure(ErrorCodes.NotFound);
            }

            if(user.GetUserRolesCount() == 1)
            {
                _logger.Here().Error("{ErrorCode} - Cannot remove role. user must have at least one role", ErrorCodes.OperationFailed);
                return Result<bool>.Failure(ErrorCodes.OperationFailed);
            }

            if(await IsTragetUserAdmin(user) && !request.CurrentUser.IsAdmin())
            {
                _logger.Here().Error("{ErroCodes} - Non admin user cannot remove roles from another admin user", ErrorCodes.NotAllowed);
                return Result<bool>.Failure(ErrorCodes.NotAllowed);
            }

            _dbTransaction.BeginTransaction();

            var roles = request.Command.Roles;
            int roleRemoveCounter = 0;
            foreach(var role in roles)
            {
                if (user.GetUserRolesCount() == 1) break;
                await _userManager.RemoveFromRoleAsync(user, role);
                roleRemoveCounter++;
            }

            _dbTransaction.CommitTransaction();

            _logger.Here().Information("{count} - roles removed successfully", roleRemoveCounter);
            _logger.Here().MethodExited();
            return Result<bool>.Success(true);
        }

        private static bool IsUpdatingOwnRole(UserDto user, string userId) => user.Id == userId;

        private async Task<bool> IsTragetUserAdmin(ApplicationUser user) => await _userManager.IsInRoleAsync(user, Roles.Admin.ToString());
    }
}
