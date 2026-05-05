using System.ComponentModel.DataAnnotations;

namespace Claims.Data.Auditing.Options;

public sealed class SqlServerOptions
{
    public const string SectionName = "SqlServer";

    [Required]
    public required string ConnectionString { get; init; }
}
