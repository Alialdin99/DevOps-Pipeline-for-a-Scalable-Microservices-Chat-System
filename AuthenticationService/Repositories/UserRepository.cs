using AuthenticationService.Models;
using MongoDB.Driver;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
namespace AuthenticationService.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;
        public UserRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task UpdateAsync(string id, User updatedUser)
        {
            await _users.ReplaceOneAsync(u => u.Id == id, updatedUser);
        }

        public async Task DeleteAsync(string id)
        {
            await _users.DeleteOneAsync(u => u.Id == id);
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }
    }
}
