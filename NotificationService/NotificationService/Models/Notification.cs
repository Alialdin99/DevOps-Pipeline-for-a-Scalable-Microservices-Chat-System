using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("receiverId")]
        public string ReceiverId { get; set; }

        [BsonElement("senderId")]
        public string SenderId { get; set; }

        [BsonElement("fromUser")]
        public string FromUser { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [BsonElement("read")]
        public bool Read { get; set; } = false;
    }
}

