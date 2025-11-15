using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Events;
using NotificationService.Hubs;
using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService.Consumers
{
    public class MessageSentConsumer : IConsumer<MessageSentEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly NotificationRepository _notificationRepository;
        private readonly ILogger<MessageSentConsumer> _logger;

        public MessageSentConsumer(
            IHubContext<NotificationHub> hubContext,
            NotificationRepository notificationRepository,
            ILogger<MessageSentConsumer> logger)
        {
            _hubContext = hubContext;
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MessageSentEvent> context)
        {
            var msg = context.Message;
            
            try
            {
                // Save notification to database
                var notification = new Notification
                {
                    ReceiverId = msg.ReceiverId,
                    SenderId = msg.SenderId,
                    FromUser = msg.SenderId, // Will be resolved to username on frontend
                    Message = msg.Content,
                    Timestamp = msg.SentAt,
                    Read = false
                };

                await _notificationRepository.CreateAsync(notification);
                _logger.LogInformation("Notification saved for receiver: {ReceiverId}", msg.ReceiverId);

                // Send real-time notification via SignalR
                await _hubContext.Clients.User(msg.ReceiverId)
                    .SendAsync("ReceiveMessageNotification", new
                    {
                        Id = notification.Id,
                        SenderId = msg.SenderId,
                        Content = msg.Content,
                        SentAt = msg.SentAt
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MessageSentEvent for receiver: {ReceiverId}", msg.ReceiverId);
                throw;
            }
        }
    }
}

