﻿using AutoMapper;
using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Contracts.Factory;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Commands.AddUser
{
    public sealed class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<bool>>
    {

        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;

        private const string CACHE_KEY = "userlist";

        public AddUserCommandHandler(ILogger logger,
            IMapper mapper,
            IUserRepository userRepository,
            ICacheServiceFactory cacheServiceFactory)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _cacheService = cacheServiceFactory.GetService(CahceServiceTypes.InMemory);
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

            var roles = request.CreateUser.Roles;

            var isUserCreated = await _userRepository.CreateUserAsync(createUserEntity, request.CreateUser.Password);
            if (!isUserCreated)
            {
                _logger.Here().Error("Failed to create new user {@username}", request.CreateUser.UserName);
                return Result<bool>.Failure(ErrorCodes.OperationFailed);
            }

            var isRoleAssigned = await _userRepository.AddToRolesAsync(createUserEntity, roles);
            if (!isRoleAssigned)
            {
                _logger.Here().Error("Failed to assign roles to {@username}", request.CreateUser.UserName);
                return Result<bool>.Failure(ErrorCodes.OperationFailed);
            }

            _cacheService.Remove(CACHE_KEY); // invalidate user list cache

            _logger.Here().Information("user {@username} created", request.CreateUser.UserName);
            _logger.Here().MethodExited();
          
            return Result<bool>.Success(true);
        }
    }
}
