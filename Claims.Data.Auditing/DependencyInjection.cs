using Claims.Auditing;
using Claims.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Data.Auditing;

public static class DependencyInjection
{
    public static IServiceCollection AddClaimsAuditing(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "Connection string 'SqlServer' is not configured. Set ConnectionStrings:SqlServer in appsettings, environment variables, or user secrets.");

        services.AddDbContext<AuditContext>(options =>
            options.UseSqlServer(sqlConnectionString));

        services.AddScoped<IAuditTrailRepository, SqlAuditTrailRepository>();

        return services;
    }
}
