using Claims.Data.Auditing.Abstractions.Repositories;
using Claims.Data.Auditing.Options;
using Claims.Data.Auditing.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Claims.Data.Auditing;

public static class DependencyInjection
{
    public static IServiceCollection AddAuditing(this IServiceCollection services)
    {
        services.AddDbContext<AuditContext>((serviceProvider, options) =>
        {
            var sqlServerOptions = serviceProvider.GetRequiredService<IOptions<SqlServerOptions>>().Value;
            options.UseSqlServer(sqlServerOptions.ConnectionString);
        });

        services.AddScoped<IClaimAuditTrailRepository, ClaimAuditTrailRepository>();
        services.AddScoped<ICoverAuditTrailRepository, CoverAuditTrailRepository>();

        return services;
    }
}
