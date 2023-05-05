using grpc.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace grpcAPI.Services
{
    public class CacheService : ICacheService
    {
        public IDistributedCache _cache;
        public CacheService(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        public async Task SetCacheAsync(string cacheItem, string cacheKey)
        {
            await _cache.SetStringAsync(cacheKey, cacheItem);
        }
        public async Task SetCacheAsync<T>(T cacheItem, string cacheKey)
        {
            var itemSerialized = JsonSerializer.Serialize(cacheItem);
            await _cache.SetStringAsync(cacheKey, itemSerialized);
        }
        public async Task<string> GetCacheAsync(string cacheKey)
        {
            var cacheItem = await _cache.GetStringAsync(cacheKey);
            return cacheItem;
        }
        public async Task<T> GetCacheAsync<T>(string cacheKey)
        {
            var cacheItem = await _cache.GetStringAsync(cacheKey);
            var response = default(T);
            if (cacheItem != null)
            {
                response = JsonSerializer.Deserialize<T>(cacheItem);
            }
            return response;
        }
        public async Task DeleteCacheAsync(string cacheKey)
        {
            await _cache.RemoveAsync(cacheKey);
        }
    }
}
