using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Options;

public sealed class ClaimValidationOptions
{
    [Range(typeof(decimal), "0", "1000000000")]
    public decimal MaxDamageCost { get; init; } = 100000m;
}
