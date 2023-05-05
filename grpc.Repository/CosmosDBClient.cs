using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grpc.Repository
{
    public class CosmosDBClient
    {
        private readonly CosmosClient cosmosClient;
        private readonly Container container;
        private readonly Container leaseContainer;
        public CosmosDBClient(IConfiguration config)
        {
            cosmosClient = new CosmosClient(config["CosmosDB:ConnectionString"]);
            container = cosmosClient.GetContainer(config["CosmosDB:DatabaseId"], config["CosmosDB:ContainerId"]);
            leaseContainer = cosmosClient.GetContainer(config["CosmosDB:DatabaseId"], config["CosmosDB:LeaseContainerId"]);
        }
        public Container GetContainer()
        {
            return container;
        }
        public Container GetLeaseContainer()
        {
            return leaseContainer;
        }
    }
}
