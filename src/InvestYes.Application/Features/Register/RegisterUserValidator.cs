using FluentValidation;
using InvestYes.Application.Features.Register.Commands;

namespace InvestYes.Application.Features.Register;

public sealed class RegisterUserValidator
    : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .MinimumLength(8)
            .Matches("[A-Z]")
            .WithMessage("A senha deve conter uma letra maiúscula.")
            .Matches("[a-z]")
            .WithMessage("A senha deve conter uma letra minúscula.")
            .Matches("[0-9]")
            .WithMessage("A senha deve conter um número.");
    }
}


