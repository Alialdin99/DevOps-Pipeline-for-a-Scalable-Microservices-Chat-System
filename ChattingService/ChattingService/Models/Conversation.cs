    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    namespace ChattingService.Models
    {
        public class Conversation
        {

            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }

            [BsonElement("participants")]
            public List<string> Participants { get; set; } = new();//strings for user ids

            [BsonElement("createdAt")]
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            [BsonElement("lastMessage")]
            public string? LastMessage { get; set; }
        }
    }
