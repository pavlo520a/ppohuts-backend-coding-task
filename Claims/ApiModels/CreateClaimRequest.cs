using Claims.Domain;

namespace Claims.ApiModels;

public sealed class CreateClaimRequest
{
    public string CoverId { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public string Name { get; set; } = string.Empty;

    public ClaimType Type { get; set; }

    public decimal DamageCost { get; set; }
}
