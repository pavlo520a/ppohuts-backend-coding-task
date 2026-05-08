using Claims.Domain.Enums;

namespace Claims.Web.ApiModels.Covers;

public sealed class CreateCoverRequest
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public CoverType Type { get; set; }
}
