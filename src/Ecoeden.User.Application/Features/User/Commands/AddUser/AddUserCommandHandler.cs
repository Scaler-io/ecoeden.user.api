using AutoMapper;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Ecoeden.User.Application.Features.User.Commands.AddUser
{
    public sealed class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<bool>>
    {

        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddUserCommandHandler(ILogger logger,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Result<bool>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            _logger.Here().MethodEnterd();

            var createUserEntity = _mapper.Map<ApplicationUser>(request.CreateUser);
            var roles = request.CreateUser.Roles;

            var createResult = await _userManager.CreateAsync(createUserEntity, request.CreateUser.Password);
            if (!createResult.Succeeded)
            {
                _logger.Here().Error("Failed to create new user {@username}", request.CreateUser.UserName);
                return Result<bool>.Failure(ErrorCodes.OperationFailed);
            }

            var roleAssignmentResult = await _userManager.AddToRolesAsync(createUserEntity, roles);
            if (!roleAssignmentResult.Succeeded)
            {
                _logger.Here().Error("Failed to assign roles to {@username}", request.CreateUser.UserName);
                return Result<bool>.Failure(ErrorCodes.OperationFailed);
            }

            _logger.Here().Information("user {@username} created", request.CreateUser.UserName);
            _logger.Here().MethodExited();
            return Result<bool>.Success(true);
        }
    }
}
