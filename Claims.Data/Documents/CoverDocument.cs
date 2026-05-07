using Claims.Domain.Enums;

namespace Claims.Data.Documents;

public class CoverDocument : BaseDocument
{
    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public CoverType Type { get; set; }

    public decimal Premium { get; set; }
}
