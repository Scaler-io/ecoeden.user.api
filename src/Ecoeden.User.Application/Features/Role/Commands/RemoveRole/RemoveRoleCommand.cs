using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Requests;
using MediatR;

namespace Ecoeden.User.Application.Features.Role.Commands.RemoveRole;

public sealed class RemoveRoleCommand : IRequest<Result<bool>>
{
    public RemoveRoleCommand(RemoveRoleRequest request, UserDto currentUser, string correlationId)
    {
        Command = request;
        CurrentUser = currentUser;
        CorrelationId = correlationId;
    }

    public RemoveRoleRequest Command { get; set; }
    public UserDto CurrentUser { get; set; }
    public string CorrelationId { get; set; }
}
