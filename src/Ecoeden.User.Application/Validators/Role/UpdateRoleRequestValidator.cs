using Ecoeden.User.Domain.Models.Constants;
using Ecoeden.User.Domain.Models.Requests;
using FluentValidation;

namespace Ecoeden.User.Application.Validators.Role
{
    public sealed class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
    {
        public UpdateRoleRequestValidator()
        {
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithErrorCode(ErrorMessages.UserIdRequired.Code)
                .WithMessage(ErrorMessages.UserIdRequired.Message);

            RuleFor(x => x.Roles)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithErrorCode(ErrorMessages.RoleRequired.Code)
                .WithMessage(ErrorMessages.RoleRequired.Message);
        }
    }
}
