using grpc.Domain.Interfaces;
using grpc.Domain.Models;
using grpc.Repository;
using grpcAPI.Controllers;
using grpcAPI.Services;
using grpcAPI.Utilities;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.Decorate<IStoreService, StoreService>();

builder.Services.AddScoped<IRepository<ItemModel>, CosmosDB_itemRepository<ItemModel>>();
builder.Services.AddGrpc(c => c.Interceptors.Add<GrpcExceptionInterceptorHandler>());

builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddSingleton<CosmosDBClient>();
builder.Services.AddHostedService<ChangeFeedProcessorMonitor>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});

var app = builder.Build();

app.MapGrpcService<StoreController>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.Run();
