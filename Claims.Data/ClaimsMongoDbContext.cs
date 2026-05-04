using Claims.Data.Documents;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Data;

public class ClaimsMongoDbContext(DbContextOptions<ClaimsMongoDbContext> options) : DbContext(options)
{
    public DbSet<ClaimDocument> Claims => Set<ClaimDocument>();

    public DbSet<CoverDocument> Covers => Set<CoverDocument>();

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
