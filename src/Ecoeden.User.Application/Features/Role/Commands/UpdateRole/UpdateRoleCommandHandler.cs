﻿using Contracts.Events;
using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Contracts.EventBus;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Microsoft.Extensions.Options;
using User.Domain.ConfigurationOptions.App;
using Ecoeden.User.Application.Contracts.Factory;
using MediatR;

namespace Ecoeden.User.Application.Features.Role.Commands.UpdateRole;

public sealed class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<bool>>
{
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPublishServiceFactory _publishServiceFactory;
    private readonly ICacheService _cacheService;
    private readonly AppOption _appOption;

    public UpdateRoleCommandHandler(ILogger logger,
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

    public async Task<Result<bool>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.Here().MethodEnterd();

        if (IsUpdatingOwnRole(request.CurrentUser, request.Command.UserId))
        {
            _logger.Here().Error("Updating own role is not allowd");
            return Result<bool>.Failure(ErrorCodes.OperationFailed);
        }

        List<string> roles = request.Command.Roles.ToList();
        ApplicationUser user = await _userRepository.GetUserById(request.Command.UserId);

        if (user is null)
        {
            _logger.Here().Error("No user was found for role assignments");
            return Result<bool>.Failure(ErrorCodes.NotFound, "User not found");
        }

        if (user.IsDefaultAdmin)
        {
            _logger.Here().Error("{ErrorCode} - no action can be performed on deafult admin", ErrorCodes.NotAllowed, request.Command.UserId);
            return Result<bool>.Failure(ErrorCodes.NotAllowed);
        }

        foreach (var role in roles)
        {
            if (await _userRepository.IsInRole(user, role))
            {
                _logger.Here().Information("User has already assigned the role {roleName}", role);
                continue;
            }
            await _userRepository.AddToRoleAsync(user, role);
        }

        user.SetUpdatedBy(request.CurrentUser.Id);
        user.SetUpdationTime();

        await _userRepository.UpdateUser(user);

        _cacheService.Remove(_appOption.UserListCacheKey);
        await PublishUserUpdateEvent(request, user);

        _logger.Here().Information("User assigned to role {roles}", string.Join(',', roles));
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }

    private async Task PublishUserUpdateEvent(UpdateRoleCommand request, ApplicationUser user)
    {
        // publish user updte event
        var userUpdateEventService = _publishServiceFactory.CreatePublishService<ApplicationUser, UserUpdated>();
        var updatedUser = await _userRepository.GetUserById(user.Id);
        await userUpdateEventService.PublishAsync(updatedUser, request.CorrelationId);
    }

    private static bool IsUpdatingOwnRole(UserDto user, string userId) => userId == user.Id;
}
