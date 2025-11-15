using MongoDB.Driver;
using NotificationService.Models;

namespace NotificationService.Repositories
{
    public class NotificationRepository
    {
        private readonly IMongoCollection<Notification> _notifications;

        public NotificationRepository(IMongoDatabase database)
        {
            _notifications = database.GetCollection<Notification>("Notifications");
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            await _notifications.InsertOneAsync(notification);
            return notification;
        }

        public async Task<List<Notification>> GetByReceiverIdAsync(string receiverId)
        {
            return await _notifications
                .Find(n => n.ReceiverId == receiverId)
                .SortByDescending(n => n.Timestamp)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(string id)
        {
            return await _notifications.Find(n => n.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _notifications.DeleteOneAsync(n => n.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> MarkAsReadAsync(string id)
        {
            var update = Builders<Notification>.Update.Set(n => n.Read, true);
            var result = await _notifications.UpdateOneAsync(n => n.Id == id, update);
            return result.ModifiedCount > 0;
        }
    }
}

