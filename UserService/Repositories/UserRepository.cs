using MongoDB.Driver;
using UserService.Models;

namespace UserService.Repositories
{
    public class UserRepository
    {

        private readonly IMongoCollection<UserProfile> _collection;

        public UserRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<UserProfile>("Users");
        }

        public async Task CreateAsync(UserProfile user)
        {
           
            try
            {
                await _collection.InsertOneAsync(user);
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                throw;
            }
        }

        public async Task<List<UserProfile>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }


        public async Task<UserProfile?> GetByAuthUserIdAsync(string authUserId)
        {
            return await _collection.Find(u => u.AuthUserId == authUserId).FirstOrDefaultAsync();
        }

        public async Task<UserProfile?> GetByIdAsync(string id)
        {
            return await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(string id, UserProfile updatedUser)
        {
            await _collection.ReplaceOneAsync(u => u.Id == id, updatedUser);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(u => u.Id == id);
        }
    }
}
