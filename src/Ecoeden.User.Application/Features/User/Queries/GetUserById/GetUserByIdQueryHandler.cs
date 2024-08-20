using AutoMapper;
using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Ecoeden.User.Domain.Models.Responses.Users;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserResponse>>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(ILogger logger,
        IMapper mapper,
        IUserRepository userRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.Here().MethodEnterd();

        var result = await _userRepository.GetUserById(request.Id);

        if (result is null)
        {
            _logger.Here().Error("{@errorcode} - No user was found with {@id}", ErrorCodes.NotFound, request.Id);
            return Result<UserResponse>.Failure(ErrorCodes.NotFound);
        }

        var userResponse = _mapper.Map<UserResponse>(result);

        _logger.Here().Information("user found with id {id}", request.Id);
        _logger.Here().MethodExited();
        return Result<UserResponse>.Success(userResponse);
    }
}
