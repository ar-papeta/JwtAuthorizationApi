using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; }
}
