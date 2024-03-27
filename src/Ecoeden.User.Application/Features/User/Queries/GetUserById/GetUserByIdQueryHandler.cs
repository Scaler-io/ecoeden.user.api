using AutoMapper;
using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Factory;
using Ecoeden.User.Application.Contracts.Persistence.Users;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Ecoeden.User.Domain.Models.Responses.Users;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Queries.GetUserById
{
    public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserResponse>>
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(ILogger logger,
            IMapper mapper,
            ICacheServiceFactory cacheServiceFactory,
            IUserRepository userRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _cacheService = cacheServiceFactory.GetService(CahceServiceTypes.InMemory);
            _userRepository = userRepository;
        }

        public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.Here().MethodEnterd();

            string cacheKey = $"user-{request.Id}";
            if (_cacheService.Contains(cacheKey)) // If a cahce hit
            {
                _logger.Here().Information("cache hit - {@key}", cacheKey);
                _logger.Here().MethodExited();
                return Result<UserResponse>.Success(_cacheService.Get<UserResponse>(cacheKey));
            }

            // ofcourse, we cant ignoe a terrible cache miss 
            _logger.Here().Information("cache miss - {@key}", cacheKey);
            var result = await _userRepository.GetUserById(request.Id);
            if (!result.IsSuccess)
            {
                _logger.Here().Error("{@errorcode} - No user was found with {@id}", ErrorCodes.NotFound, request.Id);
                return Result<UserResponse>.Failure(ErrorCodes.NotFound);
            }

            var userResponse = _mapper.Map<UserResponse>(result.Value);
            _cacheService.Set(cacheKey, userResponse, null);

            _logger.Here().Information("user found with id {id}", request.Id);
            _logger.Here().MethodExited();
            return Result<UserResponse>.Success(userResponse);
        }
    }
}
