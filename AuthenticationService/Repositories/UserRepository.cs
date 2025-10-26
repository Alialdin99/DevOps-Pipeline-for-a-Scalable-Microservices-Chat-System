using AuthenticationService.Models;
using MongoDB.Driver;
using System.Runtime.CompilerServices;
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
    }
}
