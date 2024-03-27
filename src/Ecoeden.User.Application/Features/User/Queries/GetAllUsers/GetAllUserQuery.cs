using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Responses.Users;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Queries.GetAllUsers
{
    public sealed class GetAllUserQuery : IRequest<Result<IReadOnlyList<UserResponse>>>
    {
    }
}
