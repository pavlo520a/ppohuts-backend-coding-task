using Claims.Application.Commands.Covers;
using Claims.Application.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Claims.Application.Validation.Covers;

public sealed class CreateCoverCommandValidator : AbstractValidator<CreateCoverCommand>
{
    public CreateCoverCommandValidator(IOptions<ValidationRulesOptions> options)
    {
        var rules = options.Value;

        RuleFor(x => x.StartDate)
            .Must(date => date.ToUniversalTime().Date >= DateTime.UtcNow.Date)
            .WithMessage("StartDate cannot be in the past.");

        RuleFor(x => x)
            .Must(x => x.EndDate.Date >= x.StartDate.Date)
            .WithMessage("EndDate must be greater than or equal to StartDate.");

        RuleFor(x => x)
            .Must(x => x.EndDate.Date <= x.StartDate.Date.AddYears(rules.Covers.MaxInsurancePeriodYears).AddDays(-1))
            .WithMessage($"The total insurance period cannot exceed {rules.Covers.MaxInsurancePeriodYears} year(s).");
    }
}
