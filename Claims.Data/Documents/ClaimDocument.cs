using Claims.Domain.Enums;

namespace Claims.Data.Documents;

public class ClaimDocument : BaseDocument
{
    public required string CoverId { get; set; }

    public required DateTime Created { get; set; }

    public required string Name { get; set; }

    public required ClaimType Type { get; set; }

    public required decimal DamageCost { get; set; }
}
