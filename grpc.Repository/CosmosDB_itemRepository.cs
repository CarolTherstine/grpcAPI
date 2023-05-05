using grpc.Domain.Interfaces;
using grpc.Domain.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grpc.Repository
{
    public class CosmosDB_itemRepository<T> : IRepository<T> where T : ItemModel
    {
        private readonly CosmosDBClient cosmosDb;
        private readonly Container container;
        public CosmosDB_itemRepository(CosmosDBClient CosmosDb)
        {
            cosmosDb = CosmosDb ?? throw new ArgumentNullException(nameof(CosmosDb));
            container = cosmosDb.GetContainer();
        }

        public async Task<ItemResponse<T>> CreateItemAsync(T item)
        {
            if (String.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
            }
            var response = await container.CreateItemAsync<T>(item, new PartitionKey(item.SectionKey));
            return response;
        }
        public async Task<ItemResponse<T>> UpdateItemAsync(string id, T item)
        {
            var response = await container.UpsertItemAsync<T>(item, new PartitionKey(item.SectionKey));
            return response;
        }

        public async Task<ItemResponse<T>> DeleteItemAsync(string id, string sectionKey)
        {
            var response = await container.DeleteItemAsync<T>(id, new PartitionKey(sectionKey));
            return response;
        }

        public async Task<ItemResponse<T>> GetItemAsync(string id, string sectionKey)
        {
            ItemResponse<T> response = await container.ReadItemAsync<T>(id, new PartitionKey(sectionKey));
            return response;
        }

        public async Task<IEnumerable<T>> GetItemsAsync(string queryString)
        {
            var query = container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }
    }
}
