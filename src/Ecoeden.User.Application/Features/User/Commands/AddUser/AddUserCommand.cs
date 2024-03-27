using Ecoeden.User.Domain.Models.Requests;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Commands.AddUser
{
    public sealed class AddUserCommand : IRequest<Unit>
    {
        public CreateUserRequest Request { get; private set; }
        public AddUserCommand(CreateUserRequest request)
        {
            Request = request;
        }
    }
}
