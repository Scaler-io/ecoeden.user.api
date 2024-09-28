using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Requests;
using Ecoeden.User.Domain.Models.Responses.Users;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Commands.AddUser;

public sealed class AddUserCommand : IRequest<Result<CreateUserResponse>>
{
    public CreateUserRequest CreateUser { get; private set; }
    public RequestInformation RequestInformation { get; }

    public AddUserCommand(CreateUserRequest createUser, RequestInformation requestInformation)
    {
        CreateUser = createUser;
        RequestInformation = requestInformation ;
    }
}
