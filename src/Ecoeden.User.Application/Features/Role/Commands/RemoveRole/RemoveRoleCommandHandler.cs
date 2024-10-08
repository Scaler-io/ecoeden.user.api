﻿using Contracts.Events;
using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Contracts.EventBus;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Microsoft.Extensions.Options;
using User.Domain.ConfigurationOptions.App;
using MediatR;
using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Factory;

namespace Ecoeden.User.Application.Features.Role.Commands.RemoveRole;

public sealed class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, Result<bool>>
{
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPublishServiceFactory _publishServiceFactory;
    private readonly ICacheService _cacheService;
    private readonly AppOption _appOption;

    public RemoveRoleCommandHandler(ILogger logger,
        IUserRepository userRepository,
        IPublishServiceFactory publishServiceFactory,
        ICacheServiceFactory cacheServiceFactory,
        IOptions<AppOption> appOption)
    {
        _logger = logger;
        _userRepository = userRepository;
        _publishServiceFactory = publishServiceFactory;
        _cacheService = cacheServiceFactory.GetService(CahceServiceTypes.InMemory);
        _appOption = appOption.Value;
    }

    public async Task<Result<bool>> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.Here().MethodEnterd();

        if (IsUpdatingOwnRole(request.CurrentUser, request.Command.UserId))
        {
            _logger.Here().Error("Updating own role is not allowd");
            return Result<bool>.Failure(ErrorCodes.OperationFailed);
        }

        var user = await _userRepository.GetUserById(request.Command.UserId);

        if (!request.CurrentUser.IsAdmin() || user.CreatedBy != request.CurrentUser.Id)
        {
            _logger.Here().Error("{ErroCode} - user is not authorized");
            return Result<bool>.Failure(ErrorCodes.Unauthorized);
        }

        if (user is null)
        {
            _logger.Here().Error("{ErrorCode} - no user entity found with {id}", ErrorCodes.NotFound, request.Command.UserId);
            return Result<bool>.Failure(ErrorCodes.NotFound);
        }

        if (user.IsDefaultAdmin)
        {
            _logger.Here().Error("{ErrorCode} - no action can be performed on deafult admin", ErrorCodes.NotAllowed, request.Command.UserId);
            return Result<bool>.Failure(ErrorCodes.NotAllowed);
        }

        if (user.GetUserRolesCount() == 1)
        {
            _logger.Here().Error("{ErrorCode} - Cannot remove role. user must have at least one role", ErrorCodes.OperationFailed);
            return Result<bool>.Failure(ErrorCodes.OperationFailed);
        }

        if (await IsTragetUserAdmin(user) && !request.CurrentUser.IsAdmin())
        {
            _logger.Here().Error("{ErroCodes} - Non admin user cannot remove roles from another admin user", ErrorCodes.NotAllowed);
            return Result<bool>.Failure(ErrorCodes.NotAllowed);
        }

        var roles = request.Command.Roles;
        int roleRemoveCounter = 0;
        foreach (var role in roles)
        {
            if (user.GetUserRolesCount() == 1) break;
            await _userRepository.RemoveFromRoleAsync(user, role);
            roleRemoveCounter++;
        }

        user.SetUpdatedBy(request.CurrentUser.Id);
        user.SetUpdationTime();

        await _userRepository.UpdateUser(user);

        _cacheService.Remove(_appOption.UserListCacheKey);

        // publish user update event
        await PublishUserUpdateEvent(request, user);

        _logger.Here().Information("{count} - roles removed successfully", roleRemoveCounter);
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }

    private async Task PublishUserUpdateEvent(RemoveRoleCommand request, ApplicationUser user)
    {
        var userUpdateEventService = _publishServiceFactory.CreatePublishService<ApplicationUser, UserUpdated>();
        var updatedUser = await _userRepository.GetUserById(user.Id);
        await userUpdateEventService.PublishAsync(updatedUser, request.CorrelationId);
    }

    private static bool IsUpdatingOwnRole(UserDto user, string userId) => user.Id == userId;

    private async Task<bool> IsTragetUserAdmin(ApplicationUser user) => await _userRepository.IsInRole(user, Roles.Admin.ToString());
}
