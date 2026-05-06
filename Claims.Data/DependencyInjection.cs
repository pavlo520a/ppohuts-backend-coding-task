using Claims.Data.Abstractions.Outbox;
using Claims.Data.Abstractions.Repositories;
using Claims.Data.Options;
using Claims.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Claims.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
        {
            var mongoOptions = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return new MongoClient(mongoOptions.ConnectionString);
        });

        services.AddDbContext<ClaimsMongoDbContext>((serviceProvider, options) =>
        {
            var mongoOptions = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            var client = serviceProvider.GetRequiredService<MongoClient>();
            var database = client.GetDatabase(mongoOptions.DatabaseName);
            options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
        });

        services.AddScoped<IClaimRepository, ClaimRepository>();
        services.AddScoped<ICoverRepository, CoverRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();

        services
            .AddOptions<MongoDbOptions>()
            .BindConfiguration(MongoDbOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
