namespace Claims.Data.Auditing.Entities;

public class ClaimAudit
{
    public int Id { get; set; }

    public required string ClaimId { get; set; }

    public required DateTime Created { get; set; }

    public required string HttpRequestType { get; set; }
}
