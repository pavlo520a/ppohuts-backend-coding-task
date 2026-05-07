using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Data.Documents;

public abstract class BaseDocument
{
    [BsonId]
    public required string Id { get; set; }
}
