using Claims.Domain.Enums;

namespace Claims.Domain.Models;

public class Claim
{
    public required string Id { get; set; }

    public required string CoverId { get; set; }

    public required DateTime Created { get; set; }

    public required string Name { get; set; }

    public required ClaimType Type { get; set; }

    public required decimal DamageCost { get; set; }
}
