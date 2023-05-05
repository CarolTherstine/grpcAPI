using grpc.Domain.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace grpc.Repository
{
    public class ChangeFeedProcessorMonitor : BackgroundService
    {
        private static CosmosDBClient client;
        private static IDistributedCache cache;
        public ChangeFeedProcessorMonitor(CosmosDBClient Client, IDistributedCache Cache)
        {
            client = Client ?? throw new ArgumentNullException(nameof(Client));
            cache = Cache ?? throw new ArgumentNullException(nameof(Cache));
        }
        private static async Task<ChangeFeedProcessor> StartChangeFeedProcessorAsync(CosmosDBClient cosmosClient)
        {
            Container leaseContainer = cosmosClient.GetLeaseContainer();
            ChangeFeedProcessor changeFeedProcessor = cosmosClient.GetContainer()
            .GetChangeFeedProcessorBuilder<ItemModel>(processorName: "changeFeedSample", onChangesDelegate: HandleChangesAsync)
                .WithInstanceName("grcpAPI")
                .WithLeaseContainer(leaseContainer)
                .Build();

            await changeFeedProcessor.StartAsync();
            return changeFeedProcessor;
        }
        static async Task HandleChangesAsync(ChangeFeedProcessorContext context, IReadOnlyCollection<ItemModel> changes, CancellationToken cancellationToken)
        {
            foreach (var item in changes)
            {
                var json = await cache.GetStringAsync(item.SectionKey);
                var itemList = new List<ItemModel>();
                if (json != null)
                {
                    itemList = JsonSerializer.Deserialize<List<ItemModel>>(json);
                    itemList = itemList.Where(c => c.Id != item.Id).ToList();
                }
                cache.Remove(item.SectionKey);
                if (itemList.Any())
                    await cache.SetStringAsync(item.SectionKey, JsonSerializer.Serialize(itemList));
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartChangeFeedProcessorAsync(client);
        }
    }
}