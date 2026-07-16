using FluentValidation;

namespace InvestYes.Application.Features.Assets.Commands.UpdateAsset;

public sealed class UpdateAssetCommandValidator : AbstractValidator<UpdateAssetCommand>
{
    public UpdateAssetCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Ticker)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Type)
            .IsInEnum();
    }
}