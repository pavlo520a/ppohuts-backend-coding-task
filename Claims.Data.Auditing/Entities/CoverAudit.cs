namespace Claims.Data.Auditing.Entities;

public class CoverAudit
{
    public int Id { get; set; }

    public required string CoverId { get; set; }

    public required DateTime Created { get; set; }

    public required string HttpRequestType { get; set; }
}
