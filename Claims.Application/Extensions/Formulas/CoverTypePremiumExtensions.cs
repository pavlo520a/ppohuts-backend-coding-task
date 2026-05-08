using Claims.Application.Options.Formulas;
using Claims.Domain.Enums;

namespace Claims.Application.Extensions.Formulas;

public static class CoverTypePremiumExtensions
{
    public static decimal GetTypeMultiplier(this PremiumTypeMultipliersOptions multipliers, CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => multipliers.Yacht,
            CoverType.PassengerShip => multipliers.PassengerShip,
            CoverType.Tanker => multipliers.Tanker,
            _ => multipliers.Other
        };
    }
}
