using AutoMapper;
using Contracts.Events;
using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Contracts.Factory;
using Ecoeden.User.Application.EventBus;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using MediatR;
using Ecoeden.User.Application.Contracts.EventBus;
using Microsoft.Extensions.Options;
using User.Domain.ConfigurationOptions.App;
using Ecoeden.User.Domain.Models.Responses.Users;

namespace Ecoeden.User.Application.Features.User.Commands.AddUser;

public sealed class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<CreateUserResponse>>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly IPublishServiceFactory _publishServiceFactory;
    private readonly AppOption _appOption;

    public AddUserCommandHandler(ILogger logger,
        IMapper mapper,
        IUserRepository userRepository,
        ICacheServiceFactory cacheServiceFactory,
        IPublishServiceFactory publishServiceFactory,
        IOptions<AppOption> appOption)
    {
        _logger = logger;
        _mapper = mapper;
        _userRepository = userRepository;
        _cacheService = cacheServiceFactory.GetService(CahceServiceTypes.InMemory);
        _publishServiceFactory = publishServiceFactory;
        _appOption = appOption.Value;
    }

    public async Task<Result<CreateUserResponse>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        _logger.Here().MethodEnterd();

        if (await _userRepository.UserNameExistsAsync(request.CreateUser.UserName))
        {
            _logger.Here().Error("Username {username} already used");
            return Result<CreateUserResponse>.Failure(ErrorCodes.BadRequest, "Username already used");
        }

        if (await _userRepository.EmailExistsAsync(request.CreateUser.Email))
        {
            _logger.Here().Error("Email {email} already used");
            return Result<CreateUserResponse>.Failure(ErrorCodes.BadRequest, "Email already used");
        }

        var createUserEntity = _mapper.Map<ApplicationUser>(request.CreateUser);
        createUserEntity.SetCreatedBy(request.RequestInformation.CurrentUser.Id);

        if (!await _userRepository.CreateUserAsync(createUserEntity, request.CreateUser.Password))
        {
            _logger.Here().Error("Failed to create new user {@username}", request.CreateUser.UserName);
            return Result<CreateUserResponse>.Failure(ErrorCodes.OperationFailed);
        }

        if (!await _userRepository.AddToRolesAsync(createUserEntity, request.CreateUser.Roles))
        {
            _logger.Here().Error("Failed to assign roles to {@username}", request.CreateUser.UserName);
            return Result<CreateUserResponse>.Failure(ErrorCodes.OperationFailed);
        }

        if (!await _userRepository.AddToClaimsAsync(request.CreateUser.UserName))
        {
            _logger.Here().Error("Failed to assign claims to {@username}", request.CreateUser.UserName);
            return Result<CreateUserResponse>.Failure(ErrorCodes.OperationFailed);
        }

        _cacheService.Remove(_appOption.UserListCacheKey); // invalidate user list cache
        _logger.Here().Information("user {@username} created", request.CreateUser.UserName);

        // generates email confirmation token
        var token = await _userRepository.GetEmailConfirmationToken(createUserEntity);

        // publishes user invitation event
        var userInvitePublishService = _publishServiceFactory.CreatePublishService<ApplicationUser, UserInvitationSent>();
        await userInvitePublishService.PublishAsync(createUserEntity,
            request.RequestInformation.CorrelationId,
            new { Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)) });

        // publishes user generic password sent
        var userPasswordPublishService = _publishServiceFactory.CreatePublishService<ApplicationUser, UserGenericPasswordSent>();
        await userPasswordPublishService.PublishAsync(createUserEntity, request.RequestInformation.CorrelationId);

        var userCreatedEventService = _publishServiceFactory.CreatePublishService<ApplicationUser, UserCreated>();
        await userCreatedEventService.PublishAsync(createUserEntity, request.RequestInformation.CorrelationId);

        _logger.Here().MethodExited();
        return Result<CreateUserResponse>.Success(new CreateUserResponse
        {
            Id = createUserEntity.Id,
        });
    }
}
