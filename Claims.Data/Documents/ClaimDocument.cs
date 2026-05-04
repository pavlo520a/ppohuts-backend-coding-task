using Claims.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Data.Documents;

public class ClaimDocument
{
    [BsonId]
    public required string Id { get; set; }

    [BsonElement("coverId")]
    public required string CoverId { get; set; }

    [BsonElement("created")]
    public DateTime Created { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("claimType")]
    public ClaimType Type { get; set; }

    [BsonElement("damageCost")]
    public decimal DamageCost { get; set; }
}
