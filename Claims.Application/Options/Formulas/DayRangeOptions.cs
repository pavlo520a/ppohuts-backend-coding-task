using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Options.Formulas;

public sealed class DayRangeOptions
{
    [Range(1, int.MaxValue)]
    public int? From { get; init; }

    [Range(1, int.MaxValue)]
    public int? To { get; init; }
}
