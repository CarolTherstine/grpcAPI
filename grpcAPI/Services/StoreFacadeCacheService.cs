using grpc.Domain.Interfaces;
using grpc.Domain.Models;
using grpc.Domain.Others;
using System.Text.Json;

namespace grpcAPI.Services
{
    public class StoreFacadeCacheService : IStoreService
    {
        //Here sectionKey is the equivalent of cacheKey, as we cache items list per section
        private IStoreService service;
        private ICacheService cache;
        public StoreFacadeCacheService(IStoreService Service, ICacheService Cache)
        {
            service = Service ?? throw new ArgumentNullException(nameof(Service));
            cache = Cache ?? throw new ArgumentNullException(nameof(Cache));
        }
        public async Task<ItemModel> CreateAsync(ItemModel item)
        {
            var response = await service.CreateAsync(item);
            return response;
        }
        public async Task<ItemModel> UpdateAsync(ItemModel item)
        {
            var response = await service.UpdateAsync(item);
            return response;
        }
        public async Task<ItemModel> DeleteAsync(string id, string sectionKey)
        {

            var response = await service.DeleteAsync(id, sectionKey);
            if (!string.IsNullOrWhiteSpace(response.SectionKey))
            {
                var json = await cache.GetCacheAsync(response.SectionKey);
                var itensList = JsonSerializer.Deserialize<List<ItemModel>>(json);
                if (itensList != null)
                {
                    itensList = itensList.Where(c => c.Id != id).ToList();
                    await cache.DeleteCacheAsync(response.SectionKey);
                    await cache.SetCacheAsync(itensList, response.SectionKey);
                }
                else
                {
                    throw new DomainException("206", "Sucessful remove. WARNING: Failed recaching");
                }
            }
            return response;

        }

        public async Task<ItemModel> GetAsync(string id, string sectionKey)
        {
            var cacheItens = await cache.GetCacheAsync<List<ItemModel>>(sectionKey);
            var response = cacheItens.Where(c => c.Id == id && c.SectionKey == sectionKey).FirstOrDefault();
            if (response == null)
            {
                response = await service.GetAsync(id,sectionKey);
            }
            return response;
        }
        public async Task<IEnumerable<ItemModel>> GetAsync(string cacheKey)
        {
            var returnitems = new List<ItemModel>();
            var trygetcache = await cache.GetCacheAsync<List<ItemModel>>(cacheKey);
            if (trygetcache != null)
                returnitems = trygetcache;
            else
            {
                var response = await service.GetAsync(cacheKey);
                returnitems.AddRange(response);
                await cache.SetCacheAsync(returnitems, cacheKey);
            }
            return returnitems;
        }
    }
}
