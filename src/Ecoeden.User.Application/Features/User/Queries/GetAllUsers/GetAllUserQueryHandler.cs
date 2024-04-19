using AutoMapper;
using Ecoeden.User.Application.Behaviors;
using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Contracts.Factory;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Ecoeden.User.Domain.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Ecoeden.User.Application.Features.User.Queries.GetAllUsers
{
    public sealed class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, Result<IReadOnlyList<UserResponse>>>
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        private const string CACHE_KEY = "userlist";

        public GetAllUserQueryHandler(ILogger logger,
            IMapper mapper,
            ICacheServiceFactory cacheServiceFactory,
            IUserRepository userRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _cacheService = cacheServiceFactory.GetService(CahceServiceTypes.InMemory);
            _userRepository = userRepository;
        }

        public async Task<Result<IReadOnlyList<UserResponse>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            _logger.Here().MethodEnterd();

            if (_cacheService.Contains(CACHE_KEY)) // If a cahce hit
            {
                _logger.Here().Information("cache hit - {@key}", CACHE_KEY);
                return Result<IReadOnlyList<UserResponse>>.Success(_cacheService.Get<IReadOnlyList<UserResponse>>(CACHE_KEY));
            }

            // ofcourse, we cant ignoe a terrible cache miss 
            _logger.Here().Information("cache miss - {@key}", CACHE_KEY);
            var result = await _userRepository.GetAllUsers();
            if (!result.Any())
            {
                _logger.Here().Error("{@errorcode} - Unable to fetch user list", ErrorCodes.OperationFailed);
                return Result<IReadOnlyList<UserResponse>>.Failure(ErrorCodes.OperationFailed);
            } 

            var userResponse = _mapper.Map<IReadOnlyList<UserResponse>>(result);
            _cacheService.Set(CACHE_KEY, userResponse, null);

            _logger.Here().Information("user list fetch successfull");
            _logger.Here().MethodExited();
            return Result<IReadOnlyList<UserResponse>>.Success(userResponse);
        }
    }
}
