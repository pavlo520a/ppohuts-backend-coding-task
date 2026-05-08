using Claims.Data.Auditing.Entities;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data.Auditing;

public class AuditContext(DbContextOptions<AuditContext> options) : DbContext(options)
{
    public DbSet<ClaimAudit> ClaimAudits => Set<ClaimAudit>();

    public DbSet<CoverAudit> CoverAudits => Set<CoverAudit>();
}
