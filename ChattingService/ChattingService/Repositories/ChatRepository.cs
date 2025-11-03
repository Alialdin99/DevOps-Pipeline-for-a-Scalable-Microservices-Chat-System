using ChattingService.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChattingService.Repositories
{
   
    public class ChatRepository
    {
        private readonly IMongoCollection<Message> _messages;
        private readonly IMongoCollection<Conversation> _conversations;

        public ChatRepository(IMongoDatabase db)
        {
            _messages = db.GetCollection<Message>("Messages");
        }
        public async Task<Conversation> GetConversationAsync(string userA, string userB)
        {
            var filter = Builders<Conversation>.Filter.And(
                Builders<Conversation>.Filter.All(c => c.Participants, new[] { userA, userB }),
                Builders<Conversation>.Filter.Size(c => c.Participants, 2)
            );
            return await _conversations.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<List<Conversation>> GetConversationsOfUserAsync(string userId)
        {
            var filter = Builders<Conversation>.Filter.AnyEq(c => c.Participants, userId);
            return await _conversations.Find(filter).ToListAsync();

        }
        public async Task<Conversation> CreateConversationAsync(List<string> participants)
        {
            var conversation = new Conversation { Participants = participants };
            await _conversations.InsertOneAsync(conversation);
            return conversation;
        }
        public async Task<Message> CreateMessageAsync(Message message)
        {
            await _messages.InsertOneAsync(message);
            return message;
        }


        public async Task<IEnumerable<Message>> GetConversationMessagesAsync(string conversationId, int limit = 100)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.ConversationId, conversationId);
            return await _messages.Find(filter).SortBy(m => m.SentAt).Limit(limit).ToListAsync();
        }
    }
    

}
