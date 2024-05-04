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
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Ecoeden.User.Application.Features.User.Commands.AddUser;

public sealed class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<bool>>
{

    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly ICacheService _cacheService;
    private readonly IPublishService<ApplicationUser, UserInvitationSent> _publishService;

    private const string CACHE_KEY = "userlist";

    public AddUserCommandHandler(ILogger logger,
        IMapper mapper,
        IUserRepository userRepository,
        ICacheServiceFactory cacheServiceFactory,
        IPublishService<ApplicationUser, UserInvitationSent> publishService,
        IProfileRepository profileRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _userRepository = userRepository;
        _cacheService = cacheServiceFactory.GetService(CahceServiceTypes.InMemory);
        _publishService = publishService;
        _profileRepository = profileRepository;
    }

    public async Task<Result<bool>> Handle(AddUserCommand request, CancellationToken cancellationToken)
      {
        _logger.Here().MethodEnterd();

        if(await _userRepository.UserNameExistsAsync(request.CreateUser.UserName))
        {
            _logger.Here().Error("Username {username} already used");
            return Result<bool>.Failure(ErrorCodes.BadRequest, "Username already used");
        }

        if (await _userRepository.EmailExistsAsync(request.CreateUser.Email))
        {
            _logger.Here().Error("Email {email} already used");
            return Result<bool>.Failure(ErrorCodes.BadRequest, "Email already used");
        }

        var createUserEntity = _mapper.Map<ApplicationUser>(request.CreateUser);
        createUserEntity.SetCreatedBy(request.RequestInformation.CurrentUser.Id);

        if (!await _userRepository.CreateUserAsync(createUserEntity, request.CreateUser.Password))
        {
            _logger.Here().Error("Failed to create new user {@username}", request.CreateUser.UserName);
            return Result<bool>.Failure(ErrorCodes.OperationFailed);
        }

        if (!await _userRepository.AddToRolesAsync(createUserEntity, request.CreateUser.Roles))
        {
            _logger.Here().Error("Failed to assign roles to {@username}", request.CreateUser.UserName);
            return Result<bool>.Failure(ErrorCodes.OperationFailed);
        }

        if(!await _userRepository.AddToClimsAsync(request.CreateUser.UserName))
        {
            _logger.Here().Error("Failed to assign claims to {@username}", request.CreateUser.UserName);
            return Result<bool>.Failure(ErrorCodes.OperationFailed);
        }

        _cacheService.Remove(CACHE_KEY); // invalidate user list cache
        _logger.Here().Information("user {@username} created", request.CreateUser.UserName);

        // generates email confirmation token
        var token = await _profileRepository.GetEmailConfirmationToken(createUserEntity);

        // publishes user invitation event
        await _publishService.PublishAsync(createUserEntity,
            request.RequestInformation.CorrelationId, 
            new { Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token))});

        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }
}
