using AutoMapper;
using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Factory;
using Ecoeden.User.Application.Contracts.Persistence.Users;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Ecoeden.User.Domain.Models.Responses.Users;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Queries.GetAllUsers
{
    public sealed class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, Result<IReadOnlyList<UserResponse>>>
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        public GetAllUserQueryHandler(ILogger logger, IUserRepository userRepository, 
            IMapper mapper, 
            ICacheServiceFactory cacheServiceFactory)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
            _cacheService = cacheServiceFactory.GetService(CahceServiceTypes.InMemory);
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
            var result = await _userRepository.GetAllAsync("UserRoles.Role.RolePermissions.Permission");
            if (!result.IsSuccess)
            {
                _logger.Here().Error("{@errorcode} - Unable to fetch user list", ErrorCodes.OperationFailed);
                return Result<IReadOnlyList<UserResponse>>.Failure(ErrorCodes.OperationFailed);
            } 

            var userResponse = _mapper.Map<IReadOnlyList<UserResponse>>(result.Value);
            _cacheService.Set(cacheKey, userResponse, null);

            _logger.Here().Information("user list fetch successfull");
            _logger.Here().MethodExited();
            return Result<IReadOnlyList<UserResponse>>.Success(userResponse);
        }
    }
}
