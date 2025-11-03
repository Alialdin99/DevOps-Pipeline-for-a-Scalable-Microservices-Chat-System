using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChattingService.Models
{
       public class Message
       {
        
           [BsonId]
           [BsonRepresentation(BsonType.ObjectId)]
           public string Id { get; set; }

           [BsonElement("conversationId")]
           public string ConversationId { get; set; }

           [BsonElement("senderId")]
           public string SenderId { get; set; }

           [BsonElement("receiverId")]
           public string ReceiverId { get; set; }

           [BsonElement("content")]
           public string Content { get; set; }

           [BsonElement("sentAt")]
           public DateTime SentAt { get; set; } = DateTime.UtcNow;
        }
    
}
