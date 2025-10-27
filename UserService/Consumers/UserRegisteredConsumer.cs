using MassTransit;
using Shared.Contracts.Events;
using UserService.Models;
using UserService.Repositories;
using UserService.Services.Interface;

namespace UserService.Consumers
{
    public class UserRegisteredConsumer : IConsumer<UserRegistered>
    {
       

        private readonly IUserService _userService;
        private readonly ILogger<UserRegisteredConsumer> _logger;

        public UserRegisteredConsumer( IUserService userService ,ILogger<UserRegisteredConsumer> logger)
        {
            
            _userService = userService;
            _logger = logger;
        }


        public async Task Consume(ConsumeContext<UserRegistered> context)
        {
            var msg = context.Message;

            var profile = new UserProfile
            {
                AuthUserId = msg.UserId,
                Username = msg.Username,
                Email = msg.Email,
                CreatedAt = msg.CreatedAt
            };

            await _userService.CreateIfNotExistsAsync(profile);
            _logger.LogInformation(" user Registered from AuthService");
           
        }
    }
}
