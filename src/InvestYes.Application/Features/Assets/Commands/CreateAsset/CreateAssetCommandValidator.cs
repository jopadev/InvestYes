using FluentValidation;

namespace InvestYes.Application.Features.Assets.Commands.CreateAsset;

public sealed class CreateAssetCommandValidator : AbstractValidator<CreateAssetCommand>
{
    public CreateAssetCommandValidator()
    {
        RuleFor(x => x.Ticker)
            .NotEmpty()
            .MaximumLength(10)
            .Matches("^[A-Za-z0-9]+$")
            .WithMessage("Ticker inválido.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        //RuleFor(x => x.Type)
        //    .IsInEnum();

    }
}
