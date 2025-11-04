using ChattingService.Event;
using ChattingService.Models;
using ChattingService.Repositories;
using MassTransit;



namespace ChattingService.Services
{
    public class ChatService
    {
        private readonly ChatRepository _repo;
        private readonly IPublishEndpoint _publishEndpoint;

        public ChatService(ChatRepository repo, IPublishEndpoint publishEndpoint, IConfiguration config)
        {
            _repo = repo;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Message> SendMessage( string senderId, string receiverId, string content)
        {
            Conversation conversation = await _repo.GetConversationAsync(senderId, receiverId);
            
            if (conversation == null)
                conversation = await _repo.CreateConversationAsync(new[] {senderId, receiverId}.ToList());

            var message = new Message
            {
                Content = content,
                SenderId = senderId,
                ReceiverId = receiverId,
                ConversationId = conversation.Id
            };

            await _repo.CreateMessageAsync(message);

            await _publishEndpoint.Publish(new MessageSentEvent
            {
                MessageId = message.Id,
                SenderId = senderId,
                ReceiverId = receiverId,
                ConversationId = conversation.Id,
                Content = content,
                SentAt = message.SentAt
            });

            return message;
        }

        public async Task<List<Conversation>> GetConversationsOfUserAsync(string userId)
        {
            List<Conversation> conversations = await _repo.GetConversationsOfUserAsync(userId);
            return conversations;
        }

        public Task<IEnumerable<Message>> GetConversationMessagesAsync(string conversationId)
        {
            return _repo.GetConversationMessagesAsync(conversationId);
        }
    }
    
}
