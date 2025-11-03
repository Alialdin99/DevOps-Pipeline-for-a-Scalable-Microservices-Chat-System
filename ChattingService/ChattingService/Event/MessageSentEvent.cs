namespace ChattingService.Event
{
    public class MessageSentEvent
    {
        
        public string MessageId { get; init; }
        public string ConversationId { get; init; }
        public string SenderId { get; init; }
        public string ReceiverId { get; init; }
        public string Content { get; init; }
        public DateTime SentAt { get; init; }
        
    }
}
