namespace Claims.Data.Auditing.Abstractions.Repositories;

public interface ICoverAuditTrailRepository
{
    Task<bool> AnyAsync(string coverId, string httpMethod, CancellationToken cancellationToken);

    Task WriteAsync(string coverId, string httpMethod, CancellationToken cancellationToken);
}
