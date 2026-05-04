using Claims.Domain;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Data.Persistence;

public class CoverDocument
{
    [BsonId]
    public string Id { get; set; } = string.Empty;

    [BsonElement("startDate")]
    public DateTime StartDate { get; set; }

    [BsonElement("endDate")]
    public DateTime EndDate { get; set; }

    [BsonElement("claimType")]
    public CoverType Type { get; set; }

    [BsonElement("premium")]
    public decimal Premium { get; set; }
}
