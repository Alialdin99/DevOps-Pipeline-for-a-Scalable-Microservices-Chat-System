
using MassTransit;
using NotificationService.Events;
using NotificationService.Services;


namespace NotificationService.Consumers
{

    public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
    {
        private readonly EmailService _emailService;

        public UserRegisteredConsumer(EmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var user = context.Message;
            var subject = "Welcome to ChatApp!";
            var body = $"<h3>Hi {user.Username},</h3><p>Welcome! Your account has been created successfully.</p>";

            await _emailService.SendEmailAsync(user.Email, subject, body);
        }
    }

}
