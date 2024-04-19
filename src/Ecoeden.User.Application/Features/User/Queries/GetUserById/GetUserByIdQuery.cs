using Ecoeden.User.Application.Behaviors;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Responses.Users;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Queries.GetUserById
{
    public sealed class GetUserByIdQuery : IRequest<Result<UserResponse>>, ISkipPiplineBehavior
    {
        public string Id { get; set; }

        public GetUserByIdQuery(string id)
        {
            Id = id;
        }
    }
}
