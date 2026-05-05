using Claims.Data.Auditing.Abstractions.Repositories;
using Claims.Data.Auditing.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Data.Auditing;

public static class DependencyInjection
{
    public static IServiceCollection AddAuditing(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "Connection string 'SqlServer' is not configured. Set ConnectionStrings:SqlServer in appsettings, environment variables, or user secrets.");

        services.AddDbContext<AuditContext>(options =>
            options.UseSqlServer(sqlConnectionString));

        services.AddScoped<IClaimAuditTrailRepository, ClaimAuditTrailRepository>();
        services.AddScoped<ICoverAuditTrailRepository, CoverAuditTrailRepository>();

        return services;
    }
}
