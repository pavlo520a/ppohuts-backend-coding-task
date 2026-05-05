using Claims.Data.Abstractions.Repositories;
using Claims.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Claims.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoConnectionString = configuration["MongoDb:ConnectionString"]
            ?? throw new InvalidOperationException(
                "MongoDb:ConnectionString is not configured. Set MongoDb:ConnectionString in appsettings, environment variables, or user secrets.");
        var mongoDatabaseName = configuration["MongoDb:DatabaseName"]
            ?? throw new InvalidOperationException(
                "MongoDb:DatabaseName is not configured.");

        services.AddDbContext<ClaimsMongoDbContext>(options =>
        {
            var client = new MongoClient(mongoConnectionString);
            var database = client.GetDatabase(mongoDatabaseName);
            options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
        });

        services.AddScoped<IClaimRepository, ClaimRepository>();
        services.AddScoped<ICoverRepository, CoverRepository>();

        return services;
    }
}
