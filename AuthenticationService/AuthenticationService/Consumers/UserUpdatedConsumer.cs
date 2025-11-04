using AuthenticationService.Repositories;
using MassTransit;
using Shared.Contracts.Events;

namespace AuthenticationService.Consumers
{
    public class UserUpdatedConsumer : IConsumer<UserUpdated>
    {
        private readonly UserRepository _repo;
        private readonly ILogger<UserUpdatedConsumer> _logger;

        public UserUpdatedConsumer(UserRepository repo, ILogger<UserUpdatedConsumer> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserUpdated> context)
        {
            var msg = context.Message;

            var existing = await _repo.GetByIdAsync(msg.UserId);
            if (existing == null)
            {
                _logger.LogWarning("User not found in AuthService for update: {UserId}", msg.UserId);
                return;
            }

            existing.Username = msg.Username;
            existing.Email = msg.Email;

            await _repo.UpdateAsync(existing.Id, existing);
            _logger.LogInformation("Updated user {UserId} in AuthService", msg.UserId);
        }
    }
}
