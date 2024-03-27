using AutoMapper;
using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Factory;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Ecoeden.User.Domain.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecoeden.User.Application.Features.User.Queries.GetAllUsers
{
    public sealed class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, Result<IReadOnlyList<UserResponse>>>
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUserQueryHandler(ILogger logger,
            IMapper mapper,
            ICacheServiceFactory cacheServiceFactory,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _cacheService = cacheServiceFactory.GetService(CahceServiceTypes.InMemory);
            _userManager = userManager;
        }

        public async Task<Result<IReadOnlyList<UserResponse>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            _logger.Here().MethodEnterd();
            
            string cacheKey = "userlist";
            if (_cacheService.Contains(cacheKey)) // If a cahce hit
            {
                _logger.Here().Information("cache hit - {@key}", cacheKey);
                return Result<IReadOnlyList<UserResponse>>.Success(_cacheService.Get<IReadOnlyList<UserResponse>>(cacheKey));
            }

            // ofcourse, we cant ignoe a terrible cache miss 
            _logger.Here().Information("cache miss - {@key}", cacheKey);
            var result = await _userManager.Users.Include("UserRoles.Role.RolePermissions.Permission").ToListAsync();
            if (!result.Any())
            {
                _logger.Here().Error("{@errorcode} - Unable to fetch user list", ErrorCodes.OperationFailed);
                return Result<IReadOnlyList<UserResponse>>.Failure(ErrorCodes.OperationFailed);
            } 

            var userResponse = _mapper.Map<IReadOnlyList<UserResponse>>(result);
            _cacheService.Set(cacheKey, userResponse, null);

            _logger.Here().Information("user list fetch successfull");
            _logger.Here().MethodExited();
            return Result<IReadOnlyList<UserResponse>>.Success(userResponse);
        }
    }
}
