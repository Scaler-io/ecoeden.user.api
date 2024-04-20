using Ecoeden.User.Domain.Models.Core;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Commands.EnableUser
{
    public sealed class EnableUserCommand : IRequest<Result<bool>>
    {
        public EnableUserCommand(string userId, UserDto currentUser)
        {
            UserId = userId;
            CurrentUser = currentUser;
        }

        public string UserId { get; set; }
        public UserDto CurrentUser { get; set; }
    }
}
