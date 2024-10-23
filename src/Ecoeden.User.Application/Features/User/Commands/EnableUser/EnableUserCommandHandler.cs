using Contracts.Events;
using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Contracts.EventBus;
using Ecoeden.User.Application.Contracts.Factory;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using User.Domain.ConfigurationOptions.App;

namespace Ecoeden.User.Application.Features.User.Commands.EnableUser;

public sealed class EnableUserCommandHandler : IRequestHandler<EnableUserCommand, Result<bool>>
{
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly IPublishServiceFactory _publishServiceFactory;
    private readonly AppOption _appOption;

    public EnableUserCommandHandler(ILogger logger,
        IUserRepository userRepository,
        ICacheServiceFactory cacheServiceFactory,
        IPublishServiceFactory publishServiceFactory,
        IOptions<AppOption> appOption)
    {
        _logger = logger;
        _userRepository = userRepository;
        _cacheService = cacheServiceFactory.GetService(CahceServiceTypes.InMemory);
        _publishServiceFactory = publishServiceFactory;
        _appOption = appOption.Value;
    }

    public async Task<Result<bool>> Handle(EnableUserCommand request, CancellationToken cancellationToken)
    {
        _logger.Here().MethodEnterd();

        var userEntity = await _userRepository.GetUserById(request.UserId);
        if (userEntity is null)
        {
            _logger.Here().Error("{ErrodCode} - No user was found with id", ErrorCodes.NotFound);
            return Result<bool>.Failure(ErrorCodes.NotFound);
        }

        if (userEntity.IsDefaultAdmin)
        {
            _logger.Here().Error("{ErrodCode} - No operation can be performed on default admin", ErrorCodes.Unauthorized);
            return Result<bool>.Failure(ErrorCodes.Unauthorized);
        }

        if (!request.CurrentUser.IsAdmin() && userEntity.CreatedBy != request.CurrentUser.Id)
        {
            _logger.Here().Error("{ErrodCode} - operation not allowed on this user", ErrorCodes.NotAllowed);
            return Result<bool>.Failure(ErrorCodes.NotAllowed);
        }

        userEntity.ToggleVisibility();
        userEntity.SetUpdatedBy(request.CurrentUser.Id);
        userEntity.SetUpdationTime();

        await _userRepository.UpdateUser(userEntity);

        _cacheService.Remove(_appOption.UserListCacheKey);
        _cacheService.Remove($"user-{userEntity.Id}");

        // publish user update event
        var userUpdatedEventService = _publishServiceFactory.CreatePublishService<ApplicationUser, UserUpdated>();
        await userUpdatedEventService.PublishAsync(userEntity, request.CorrelationId);

        _logger.Here().Information("User updated successfully");
        _logger.Here().MethodExited();

        return Result<bool>.Success(userEntity.IsActive);
    }
}
