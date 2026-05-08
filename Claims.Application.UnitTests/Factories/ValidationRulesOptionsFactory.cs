using Claims.Application.Options;
using Microsoft.Extensions.Options;

namespace Claims.Application.UnitTests.Factories;

internal static class ValidationRulesOptionsFactory
{
    public static IOptions<ValidationRulesOptions> CreateRulesOptions()
    {
        var maxDamageCost = 100000m;
        var maxInsurancePeriodYears = 1;

        return Microsoft.Extensions.Options.Options.Create(new ValidationRulesOptions
        {
            Claims = new ClaimValidationOptions
            {
                MaxDamageCost = maxDamageCost
            },
            Covers = new CoverValidationOptions
            {
                MaxInsurancePeriodYears = maxInsurancePeriodYears
            }
        });
    }
}
