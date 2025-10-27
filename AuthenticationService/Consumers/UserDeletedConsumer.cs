using AuthenticationService.Repositories;
using MassTransit;
using Shared.Contracts.Events;

namespace AuthenticationService.Consumers
{
   
        public class UserDeletedConsumer : IConsumer<UserDeleted>
        {
            private readonly UserRepository _repository;

            public UserDeletedConsumer(UserRepository repository)
            {
                _repository = repository;
            }

            public async Task Consume(ConsumeContext<UserDeleted> context)
            {
                var message = context.Message;

                var user = await _repository.GetByIdAsync(message.UserId);
                if (user != null)
                {
                    await _repository.DeleteAsync(user.Id);
                    Console.WriteLine($" AuthService deleted user {user.Email}");
                }
                else
                {
                    Console.WriteLine($" User with ID {message.UserId} not found in AuthService.");
                }
            }
        }
}
