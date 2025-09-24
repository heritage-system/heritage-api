using Cultural_Heritage_System.Dtos.Response.Contributor;
using StackExchange.Redis;
using System.Text.Json;

namespace Cultural_Heritage_System.Services.Impl
{
    public class UserCacheService : IUserCacheService
    {
        private readonly IDatabase _db;
        private const string Key = "latest_users";

        public UserCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<List<DropdownUserResponse>> GetCachedUsersAsync()
        {
            var data = await _db.StringGetAsync(Key);
            if (data.IsNullOrEmpty) return new List<DropdownUserResponse>();

            return JsonSerializer.Deserialize<List<DropdownUserResponse>>(data!)!;
        }

        public async Task SetCachedUsersAsync(List<DropdownUserResponse> users)
        {
            var json = JsonSerializer.Serialize(users);
            await _db.StringSetAsync(Key, json, TimeSpan.FromMinutes(10)); 
        }
    }
}
