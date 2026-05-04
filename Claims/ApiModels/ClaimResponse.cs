using Claims.Domain.Enums;

namespace Claims.ApiModels;

public sealed class ClaimResponse
{
    public string Id { get; set; } = string.Empty;

    public string CoverId { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public string Name { get; set; } = string.Empty;

    public ClaimType Type { get; set; }

    public decimal DamageCost { get; set; }
}
