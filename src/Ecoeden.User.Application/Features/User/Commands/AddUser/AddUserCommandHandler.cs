using AutoMapper;
using Ecoeden.User.Application.Contracts.Data;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Ecoeden.User.Application.Features.User.Commands.AddUser
{
    public sealed class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<bool>>
    {

        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IDbTranscation _dbTransaction;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddUserCommandHandler(ILogger logger,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IDbTranscation dbTransaction)
        {
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
            _dbTransaction = dbTransaction;
        }

        public async Task<Result<bool>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            _logger.Here().MethodEnterd();

            if(await UserNameExists(request.CreateUser.UserName))
            {
                _logger.Here().Error("Username {username} already used");
                return Result<bool>.Failure(ErrorCodes.BadRequest, "Username already used");
            }

            if (await EmailExists(request.CreateUser.Email))
            {
                _logger.Here().Error("Email {email} already used");
                return Result<bool>.Failure(ErrorCodes.BadRequest, "Email already used");
            }

            var createUserEntity = _mapper.Map<ApplicationUser>(request.CreateUser);
            createUserEntity.SetCreatedBy(request.RequestInformation.CurrentUser.Id);

            var roles = request.CreateUser.Roles;

            _dbTransaction.BeginTransaction();

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

            _dbTransaction.CommitTransaction();

            _logger.Here().Information("user {@username} created", request.CreateUser.UserName);
            _logger.Here().MethodExited();
          
            return Result<bool>.Success(true);
        }

        private async Task<bool> UserNameExists(string username) => await _userManager.FindByNameAsync(username) is not null;

        private async Task<bool> EmailExists(string email) => await _userManager.FindByEmailAsync(email) is not null;
    }
}
