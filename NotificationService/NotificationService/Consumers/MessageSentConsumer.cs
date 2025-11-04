using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Events;
using NotificationService.Hubs;

namespace NotificationService.Consumers
{
    public class MessageSentConsumer : IConsumer<MessageSentEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public MessageSentConsumer(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<MessageSentEvent> context)
        {
            var msg = context.Message;
            await _hubContext.Clients.User(msg.ReceiverId)
                .SendAsync("ReceiveMessageNotification", new
                {
                    SenderId = msg.SenderId,
                    Content = msg.Content,
                    SentAt = msg.SentAt
                });
        }
    }
}

