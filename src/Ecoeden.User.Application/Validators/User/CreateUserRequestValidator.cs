using Ecoeden.User.Domain.Models.Constants;
using Ecoeden.User.Domain.Models.Requests;
using FluentValidation;

namespace Ecoeden.User.Application.Validators.User;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.UserName)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotEmpty()
            .WithErrorCode(ErrorMessages.UserNameRequired.Code)
            .WithMessage(ErrorMessages.UserNameRequired.Message);

        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotEmpty()
            .WithErrorCode(ErrorMessages.FirstNameRequired.Code)
            .WithMessage(ErrorMessages.FirstNameRequired.Message);

        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotEmpty()
            .WithErrorCode(ErrorMessages.LastNameRequired.Code)
            .WithMessage(ErrorMessages.LastNameRequired.Message);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotEmpty()
            .WithErrorCode(ErrorMessages.EmailRequired.Code)
            .WithMessage(ErrorMessages.EmailRequired.Message)
            .EmailAddress()
            .WithErrorCode(ErrorMessages.ValidEmail.Code)
            .WithMessage(ErrorMessages.ValidEmail.Message); ;

        RuleFor(x => x.Roles)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotEmpty()
            .WithErrorCode(ErrorMessages.RoleRequired.Code)
            .WithMessage(ErrorMessages.RoleRequired.Message); ;

    }
}
