namespace Claims.Data.Auditing.Abstractions.Repositories;

public interface ICoverAuditTrailRepository
{
    Task WriteAsync(string coverId, string httpMethod, CancellationToken cancellationToken);
}
