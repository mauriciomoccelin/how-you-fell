using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HowYouFell.Api.Models;

public abstract class Entity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; protected set; }

    protected Entity()
    {
        Id = ObjectId.GenerateNewId().ToString();
    }
}
