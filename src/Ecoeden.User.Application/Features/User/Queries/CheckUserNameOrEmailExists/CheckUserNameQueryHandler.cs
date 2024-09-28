using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Models.Core;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Queries.CheckUserNameOrEmailExists;
public class CheckUserNameOrEmailExistsQueryHandler : IRequestHandler<CheckUserNameOrEmailExistsQuery, Result<bool>>
{
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;

    public CheckUserNameOrEmailExistsQueryHandler(ILogger logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<Result<bool>> Handle(CheckUserNameOrEmailExistsQuery request, CancellationToken cancellationToken)
    {
        _logger.Here().MethodEnterd();
        _logger.Here().WithCorrelationId(request.CorrelationId).Information("Request - check username or email exist");
        bool result = false;

        switch (request.CheckOption)
        {
            case "username":
                result = await _userRepository.UserNameExistsAsync(request.CheckValue);
                break;
            case "email":
                result = await _userRepository.EmailExistsAsync(request.CheckValue);
                break;
            default:
                break;     
        }

        _logger.Here().MethodExited();
        return Result<bool>.Success(result);
    }
}
