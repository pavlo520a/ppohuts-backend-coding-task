using Claims.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Data.Documents;

public class CoverDocument
{
    [BsonId]
    public required string Id { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public CoverType Type { get; set; }

    public decimal Premium { get; set; }
}
