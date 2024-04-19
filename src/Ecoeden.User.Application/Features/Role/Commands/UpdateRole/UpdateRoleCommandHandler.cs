using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using MediatR;

namespace Ecoeden.User.Application.Features.Role.Commands.UpdateRole
{
    public sealed class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<bool>>
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;

        public UpdateRoleCommandHandler(ILogger logger,
            IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
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
            ApplicationUser user = await _userRepository.GetUserById(request.Command.UserId);

            if(user is null)
            {
                _logger.Here().Error("No user was found for role assignments");
                return Result<bool>.Failure(ErrorCodes.NotFound, "User not found");
            }

            foreach (var role in roles)
            {
                if(await _userRepository.IsInRole(user, role))
                {
                    _logger.Here().Information("User has already assigned the role {roleName}", role);
                    continue;
                }
                await _userRepository.AddToRoleAsync(user, role);
            }

            user.SetUpdatedBy(request.CurrentUser.Id);
            user.setUpdationTime();

            await _userRepository.UpdateUser(user);

            _logger.Here().Information("User assigned to role {roles}", string.Join(',', roles));
            _logger.Here().MethodExited();
            return Result<bool>.Success(true);
        }

        private static bool IsUpdatingOwnRole(UserDto user, string userId) => userId == user.Id;  
    }
}
