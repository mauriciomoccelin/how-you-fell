using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HowYouFell.Api.Models;

public class PersonFelling : Entity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string? TeamId { get; private set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ThreadId { get; private set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? TenantId { get; private set; }
    public string Description { get; private set; } = null!;
    public PersonFellingType Type { get; private set; }

    public class Factory
    {
        public static PersonFelling Create(
            string teamId,
            string threadId,
            string tenantId,
            string description,
            PersonFellingType type
        )
        {
            return new PersonFelling
            {
                Type = type,
                TeamId = teamId,
                ThreadId = threadId,
                TenantId = tenantId,
                Description = description
            };
        }
    }
}
