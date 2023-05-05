using grpc.Domain.Models;
using Microsoft.Azure.Cosmos;

namespace grpc.Domain.Interfaces
{
    public interface IRepository<T> where T : ItemModel
    {
        Task<IEnumerable<T>> GetItemsAsync(string query);
        Task<ItemResponse<T>> GetItemAsync(string id, string sectionKey);
        Task<ItemResponse<T>> CreateItemAsync(T item);
        Task<ItemResponse<T>> UpdateItemAsync(string id, T item);
        Task<ItemResponse<T>> DeleteItemAsync(string id, string sectionkey);
    }
}
