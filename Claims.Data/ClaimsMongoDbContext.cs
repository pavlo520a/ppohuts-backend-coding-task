using Claims.Data.Documents;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using MongoDB.EntityFrameworkCore.Metadata.Conventions;

namespace Claims.Data;

public class ClaimsMongoDbContext(DbContextOptions<ClaimsMongoDbContext> options) : DbContext(options)
{
    public DbSet<ClaimDocument> Claims => Set<ClaimDocument>();

    public DbSet<CoverDocument> Covers => Set<CoverDocument>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new CamelCaseElementNameConvention());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<ClaimDocument>()
            .ToCollection("claims");

        modelBuilder
            .Entity<CoverDocument>()
            .ToCollection("covers");
    }
}
