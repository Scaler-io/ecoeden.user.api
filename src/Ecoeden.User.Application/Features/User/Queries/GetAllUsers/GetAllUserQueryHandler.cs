using AutoMapper;
using Ecoeden.User.Application.Contracts.Cache;
using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Contracts.Factory;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Ecoeden.User.Domain.Models.Responses.Users;
using Microsoft.Extensions.Options;
using User.Domain.ConfigurationOptions.App;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Queries.GetAllUsers;

public sealed class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, Result<IReadOnlyList<UserResponse>>>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly IUserRepository _userRepository;
    private readonly AppOption _appOption;

    public GetAllUserQueryHandler(ILogger logger,
        IMapper mapper,
        ICacheServiceFactory cacheServiceFactory,
        IUserRepository userRepository,
        IOptions<AppOption> appOption)
    {
        _logger = logger;
        _mapper = mapper;
        _cacheService = cacheServiceFactory.GetService(CahceServiceTypes.InMemory);
        _userRepository = userRepository;
        _appOption = appOption.Value;
    }

    public async Task<Result<IReadOnlyList<UserResponse>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
    {
        _logger.Here().MethodEnterd();

        if (_cacheService.Contains(_appOption.UserListCacheKey)) // If a cahce hit
        {
            _logger.Here().Information("cache hit - {@key}", _appOption.UserListCacheKey);
            return Result<IReadOnlyList<UserResponse>>.Success(_cacheService.Get<IReadOnlyList<UserResponse>>(_appOption.UserListCacheKey));
        }

        // ofcourse, we cant ignoe a terrible cache miss 
        _logger.Here().Information("cache miss - {@key}", _appOption.UserListCacheKey);
        var result = await _userRepository.GetAllUsers();
        if (!result.Any())
        {
            _logger.Here().Error("{@errorcode} - Unable to fetch user list", ErrorCodes.OperationFailed);
            return Result<IReadOnlyList<UserResponse>>.Failure(ErrorCodes.OperationFailed);
        }

        var userResponse = _mapper.Map<IReadOnlyList<UserResponse>>(result);
        _cacheService.Set(_appOption.UserListCacheKey, userResponse, null);

        _logger.Here().Information("user list fetch successfull");
        _logger.Here().MethodExited();
        return Result<IReadOnlyList<UserResponse>>.Success(userResponse);
    }
}
