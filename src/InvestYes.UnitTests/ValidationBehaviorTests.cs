using FluentAssertions;
using FluentValidation;
using InvestYes.Application.Behaviors;
using InvestYes.Application.DTOs;
using InvestYes.Application.Features.Assets.Commands.CreateAsset;
using InvestYes.Domain.Enums;

namespace InvestYes.UnitTests
{
    public class ValidationBehaviorTests
    {
        [Fact]
        public async Task Should_Throw_When_Invalid()
        {
            var validators =
                new List<IValidator<CreateAssetCommand>>
                {
                new CreateAssetCommandValidator()
                };

            var behavior =
                new ValidationBehavior<
                    CreateAssetCommand,
                    AssetDto>(validators);

            var command =
                new CreateAssetCommand("","", AssetType.FII.ToString());

            await FluentActions
                .Invoking(() =>
                    behavior.Handle(
                        command,
                        default!,
                        CancellationToken.None))
                .Should()
                .ThrowAsync<ValidationException>();
        }
    }
}
