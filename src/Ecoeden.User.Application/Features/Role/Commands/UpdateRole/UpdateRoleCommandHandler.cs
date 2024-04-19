using Ecoeden.User.Application.Contracts.Data;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Ecoeden.User.Application.Features.Role.Commands.UpdateRole
{
    public sealed class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<bool>>
    {
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbTranscation _dbTransaction;

        public UpdateRoleCommandHandler(ILogger logger, 
            UserManager<ApplicationUser> userManager, 
            IDbTranscation dbTransaction)
        {
            _logger = logger;
            _userManager = userManager;
            _dbTransaction = dbTransaction;
        }

        public async Task<Result<bool>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.Here().MethodEnterd();

            if(IsUpdatingOwnRole(request.CurrentUser, request.Command.UserId))
            {
                _logger.Here().Error("Updating own role is not allowd");
                return Result<bool>.Failure(ErrorCodes.OperationFailed);
            }
            
            IEnumerable<string> roles = request.Command.Roles;
            ApplicationUser user = await _userManager.FindByIdAsync(request.Command.UserId);

            if(user is null)
            {
                _logger.Here().Error("No user was found for role assignments");
                return Result<bool>.Failure(ErrorCodes.NotFound, "User not found");
            }

            _dbTransaction.BeginTransaction();

            foreach (var role in roles)
            {
                if(await IsInRole(user, role))
                {
                    _logger.Here().Information("User has already assigned the role {roleName}", role);
                    continue;
                }
                await _userManager.AddToRoleAsync(user, role);
            }

            _dbTransaction.CommitTransaction();

            _logger.Here().Information("User assigned to role {roles}", string.Join(',', roles));
            _logger.Here().MethodExited();
            return Result<bool>.Success(true);
        }

        private async Task<bool> IsInRole(ApplicationUser user, string role) => await _userManager.IsInRoleAsync(user, role);

        private static bool IsUpdatingOwnRole(UserDto user, string userId) => userId == user.Id;

    }
}
