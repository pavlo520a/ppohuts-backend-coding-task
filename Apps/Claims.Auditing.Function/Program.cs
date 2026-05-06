using Claims.Auditing.Function;
using Claims.Data.Auditing;
using Claims.Data.Auditing.Entities;
using Claims.Data.Auditing.Options;
using Microsoft.EntityFrameworkCore;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services
    .AddOptions<SqlServerOptions>()
    .BindConfiguration(SqlServerOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddDbContext<AuditContext>((serviceProvider, options) =>
{
    var sqlServerOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<SqlServerOptions>>().Value;
    options.UseSqlServer(sqlServerOptions.ConnectionString);
});

builder.Services.AddScoped<IAuditWriteRepository, AuditWriteRepository>();

builder.Build().Run();

public interface IAuditWriteRepository
{
    Task WriteAsync(AuditMessage auditMessage, CancellationToken cancellationToken);
}

public sealed class AuditWriteRepository(AuditContext context) : IAuditWriteRepository
{
    public async Task WriteAsync(AuditMessage auditMessage, CancellationToken cancellationToken)
    {
        if (auditMessage.EntityType.Equals("claim", StringComparison.OrdinalIgnoreCase))
        {
            var isDuplicate = await context.ClaimAudits.AnyAsync(
                x => x.ClaimId == auditMessage.EntityId
                    && x.HttpRequestType == auditMessage.HttpMethod
                    && x.Created == auditMessage.OccurredAtUtc,
                cancellationToken);

            if (!isDuplicate)
            {
                context.ClaimAudits.Add(new ClaimAudit
                {
                    ClaimId = auditMessage.EntityId,
                    HttpRequestType = auditMessage.HttpMethod,
                    Created = auditMessage.OccurredAtUtc
                });
            }
        }
        else if (auditMessage.EntityType.Equals("cover", StringComparison.OrdinalIgnoreCase))
        {
            var isDuplicate = await context.CoverAudits.AnyAsync(
                x => x.CoverId == auditMessage.EntityId
                    && x.HttpRequestType == auditMessage.HttpMethod
                    && x.Created == auditMessage.OccurredAtUtc,
                cancellationToken);

            if (!isDuplicate)
            {
                context.CoverAudits.Add(new CoverAudit
                {
                    CoverId = auditMessage.EntityId,
                    HttpRequestType = auditMessage.HttpMethod,
                    Created = auditMessage.OccurredAtUtc
                });
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
