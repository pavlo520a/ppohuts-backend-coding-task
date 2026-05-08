using Claims.Domain.Enums;

namespace Claims.Data.Documents;

public class CoverDocument : BaseDocument
{
    public required DateOnly StartDate { get; set; }

    public required DateOnly EndDate { get; set; }

    public required CoverType Type { get; set; }

    public required decimal Premium { get; set; }
}
