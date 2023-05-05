using grpc.Domain.Interfaces;
using grpc.Domain.Models;
using grpc.Domain.Others;
using System.Net;

namespace grpcAPI.Services
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<ItemModel> repository;
        public StoreService(IRepository<ItemModel> Repository)
        {
            repository = Repository ?? throw new ArgumentNullException(nameof(Repository));
        }
        public async Task<ItemModel> GetAsync(string id, string partitionKey)
        {
            var response = await repository.GetItemAsync(id, partitionKey);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new DomainException(response.StatusCode.ToString(), "Internal error");
            return response.Resource;
        }
        public async Task<IEnumerable<ItemModel>> GetAsync(string sectionKey)
        {
            var response = await repository.GetItemsAsync($"SELECT * FROM c WHERE c.pk = '{sectionKey}'");
            if (response == null)
                throw new DomainException("400", "Not found");
            return response;
        }
        public async Task<ItemModel> CreateAsync(ItemModel item)
        {
            var response = await repository.CreateItemAsync(item);
            if (!response.StatusCode.Equals(HttpStatusCode.OK))
                throw new DomainException(response.StatusCode.ToString(), response.StatusCode.ToString());
            return response.Resource;
        }
        public async Task<ItemModel> DeleteAsync(string id, string sectionKey)
        {
            var response = await repository.DeleteItemAsync(id, sectionKey);
            if (!response.StatusCode.Equals(HttpStatusCode.OK))
            {
                throw new DomainException(response.StatusCode.ToString(), response.StatusCode.ToString());
            }
            return response;
        }
        public async Task<ItemModel> UpdateAsync(ItemModel item)
        {
            var response = await repository.UpdateItemAsync(item.Id, item);
            if (!response.StatusCode.Equals(HttpStatusCode.OK))
            {
                throw new DomainException(response.StatusCode.ToString(), response.StatusCode.ToString());
            }
            return response.Resource;
        }
    }
}
