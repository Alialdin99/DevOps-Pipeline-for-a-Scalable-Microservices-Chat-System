using MassTransit;
using Shared.Contracts.Events;
using UserService.Models;
using UserService.Repositories;
using UserService.Services.Interface;

namespace UserService.Services.ConcreteServices
{
    public class UserService : IUserService
    {
        private readonly UserRepository _repo;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UserService> _logger;

        public UserService(UserRepository repo , IPublishEndpoint publishEndpoint, ILogger<UserService> logger)
        {
            _repo = repo;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<List<UserProfile>> GetAllAsync()
        {
            try
            {
                return await _repo.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving all users");
                throw;
            }
        }

        public async Task<UserProfile?> GetByIdAsync(string id)
        {
            try
            {
                return await _repo.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving user by Id: {UserId}", id);
                throw;
            }
        }

        public async Task<UserProfile?> GetByAuthUserIdAsync(string authUserId)
        {
            try
            {
                return await _repo.GetByAuthUserIdAsync(authUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving user by AuthUserId: {AuthUserId}", authUserId);
                throw;
            }
        }

        public async Task CreateIfNotExistsAsync(UserProfile profile)
        {
            try
            {
                // Check if user already exists by AuthUserId
                var existing = await _repo.GetByAuthUserIdAsync(profile.AuthUserId);
                if (existing != null)
                {
                    _logger.LogInformation("User profile already exists for AuthUserId: {AuthUserId}, Email: {Email}", 
                        profile.AuthUserId, profile.Email);
                    return;
                }

                // User doesn't exist, create it
                await _repo.CreateAsync(profile);
                _logger.LogInformation("User profile created for {Email}", profile.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating user profile for {Email}", profile.Email);
                throw;
            }
        }
        public async Task UpdateAsync(string id, UserProfile updated)
        {
            try
            {
                await _repo.UpdateAsync(id, updated);
                _logger.LogInformation("User profile updated for {Email}", updated.Email);

                await _publishEndpoint.Publish(new UserUpdated
                {
                    UserId = updated.AuthUserId,
                    Username = updated.Username,
                    Email = updated.Email
                });

                _logger.LogInformation("Published UserUpdated event for {Email}", updated.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating user profile for {Email}", updated.Email);
                throw;
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                var user = await _repo.GetByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("Attempted to delete non-existing user with Id: {UserId}", id);
                    return;
                }

                await _repo.DeleteAsync(id);
                _logger.LogInformation("User profile deleted for {Email}", user.Email);

                await _publishEndpoint.Publish(new UserDeleted
                {
                    UserId = user.AuthUserId,
                    DeletedAt = DateTime.UtcNow
                });

                _logger.LogInformation("Published UserDeleted event for {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting user with Id: {UserId}", id);
                throw;
            }
        }
        public async Task<List<UserProfile>> SearchUsersByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Username must not be empty.");

            return await _repo.GetByUserNameAsync(userName);
        }


    }
}

