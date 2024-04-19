using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Requests;
using MediatR;

namespace Ecoeden.User.Application.Features.Role.Commands.UpdateRole
{
    public class UpdateRoleCommand : IRequest<Result<bool>>
    {
        public UpdateRoleRequest Command { get; set; }
        public UserDto CurrentUser { get; set; }

        public UpdateRoleCommand(UpdateRoleRequest request, 
            UserDto currentUser)
        {
            Command = request;
            CurrentUser = currentUser;
        }
    }
}
