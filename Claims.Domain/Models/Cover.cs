using Claims.Domain.Enums;

namespace Claims.Domain.Models;

public class Cover
{
    public required string Id { get; set; }

    public required DateTime StartDate { get; set; }

    public required DateTime EndDate { get; set; }

    public required CoverType Type { get; set; }

    public required decimal Premium { get; set; }
}
