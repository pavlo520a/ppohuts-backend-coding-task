using Claims.Application.Commands.Claims;
using Claims.Application.Options;
using Claims.Data.Abstractions.Repositories;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Claims.Application.Validation.Claims;

public sealed class CreateClaimCommandValidator : AbstractValidator<CreateClaimCommand>
{
    public CreateClaimCommandValidator(
        ICoverRepository coverRepository,
        IOptions<ValidationRulesOptions> options)
    {
        var rules = options.Value;

        RuleFor(x => x.CoverId)
            .NotEmpty();

        RuleFor(x => x.DamageCost)
            .LessThanOrEqualTo(rules.Claims.MaxDamageCost)
            .WithMessage($"DamageCost cannot exceed {rules.Claims.MaxDamageCost}.");

        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                var cover = await coverRepository.GetByIdAsync(command.CoverId, cancellationToken);
                if (cover is null)
                {
                    return false;
                }

                var createdDate = command.Created.Date;
                return createdDate >= cover.StartDate.Date && createdDate <= cover.EndDate.Date;
            })
            .WithMessage("Created date must be within the period of the related cover and the cover must exist.");
    }
}
