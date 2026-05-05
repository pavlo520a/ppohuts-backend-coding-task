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
            .CustomAsync(async (command, context, cancellationToken) =>
            {
                var cover = await coverRepository.GetByIdAsync(command.CoverId, cancellationToken);
                
                if (cover is null)
                {
                    context.AddFailure(nameof(command.CoverId), "Related cover does not exist.");
                    return;
                }

                var createdDate = command.Created.Date;

                if (createdDate < cover.StartDate.Date || createdDate > cover.EndDate.Date)
                {
                    context.AddFailure(nameof(command.Created), "Created date must be within the period of the related cover.");
                }
            });
    }
}
