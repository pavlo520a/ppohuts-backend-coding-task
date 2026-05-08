using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Options.Formulas;

public sealed class PremiumTypeMultipliersOptions
{
    [Range(0.0, double.MaxValue)]
    public decimal Yacht { get; init; }

    [Range(0.0, double.MaxValue)]
    public decimal PassengerShip { get; init; }

    [Range(0.0, double.MaxValue)]
    public decimal Tanker { get; init; }

    [Range(0.0, double.MaxValue)]
    public decimal Other { get; init; }
}
