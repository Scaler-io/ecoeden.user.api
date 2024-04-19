using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Requests;
using MediatR;

namespace Ecoeden.User.Application.Features.Role.Commands.RemoveRole
{
    public sealed class RemoveRoleCommand : IRequest<Result<bool>>
    {
        public RemoveRoleCommand(RemoveRoleRequest request, UserDto currentUser)
        {
            Command = request;
            CurrentUser = currentUser;
        }

        public RemoveRoleRequest Command { get; set; }
        public UserDto CurrentUser { get; set; }
    }
}
