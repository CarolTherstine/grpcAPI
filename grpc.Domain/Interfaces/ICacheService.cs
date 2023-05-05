using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grpc.Domain.Interfaces
{
    public interface ICacheService
    {
        public Task SetCacheAsync(string cacheItem, string cacheKey);
        public Task SetCacheAsync<T>(T cacheItem, string cacheKey);
        public Task<string> GetCacheAsync(string cacheKey);
        public Task<T> GetCacheAsync<T>(string cacheKey);
        public Task DeleteCacheAsync(string cacheKey);
    }
}
