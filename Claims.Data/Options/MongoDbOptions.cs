using System.ComponentModel.DataAnnotations;

namespace Claims.Data.Options;

public sealed class MongoDbOptions
{
    public const string SectionName = "MongoDb";

    [Required]
    public required string ConnectionString { get; init; }

    [Required]
    public required string DatabaseName { get; init; }
}
