using UserService.Models;

namespace UserService.Services.Interface
{
    public interface IUserService
    {

        Task<List<UserProfile>> GetAllAsync();
        Task<UserProfile?> GetByIdAsync(string id);
        Task<UserProfile?> GetByAuthUserIdAsync(string authUserId);
        Task CreateIfNotExistsAsync(UserProfile profile);
        Task UpdateAsync(string id, UserProfile updated);
        Task DeleteAsync(string id);

        Task<List<UserProfile>> SearchUsersByNameAsync(string userName);
    }
}
