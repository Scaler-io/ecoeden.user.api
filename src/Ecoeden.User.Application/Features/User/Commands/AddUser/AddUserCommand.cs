using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Requests;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Commands.AddUser
{
    public sealed class AddUserCommand : IRequest<Result<bool>>
    {
        public CreateUserRequest CreateUser { get; private set; }

        public AddUserCommand(CreateUserRequest createUser)
        {
            CreateUser = createUser;
        }
    }
}
