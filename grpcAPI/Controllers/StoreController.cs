using grpc.Domain.Interfaces;
using grpc.Domain.Models;
using grpc.Domain.Others;
using Grpc.Core;
using grpcAPI.Protos;
using grpcAPI.Utilities;
using System.Reflection.Metadata;

namespace grpcAPI.Controllers
{
    public class StoreController : StoreProto.StoreProtoBase
    {
        readonly IStoreService service;
        readonly ILogger<StoreController> logger;
        public StoreController(IStoreService Service, ILogger<StoreController> Logger)
        {
            service = Service ?? throw new ArgumentNullException(nameof(Service));
            logger = Logger ?? throw new ArgumentNullException(nameof(Logger));
        }
        public override async Task<Item> GetItem(Request request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
                throw new DomainException("400", "Get item received an empty id");

            var response = await service.GetAsync(request.Id, request.SectionKey);

            return response.ItemModel_To_grpcItem();
        }
        public override async Task<ItemList> GetItemList(RequestList request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.SectionKey))
                throw new DomainException("400", "Get itens received an empty section key");

            var response = await service.GetAsync(request.SectionKey);

            var returnResponse = response.ItemList_To_grpcItemlist();
            return returnResponse;
        }
        public override async Task<StatusEmptyResponse> Create(Item request, ServerCallContext context)
        {
            logger.LogInformation($"Create item {request.Id} for section {request.SectionKey}");

            if (request == null)
                throw new DomainException("400", "Create received an empty or null object");

            var response = await service.CreateAsync(request.grpcItem_To_ItemModel());

            return new StatusEmptyResponse { Code = "200", Message = "Create successful" };
        }
        public override async Task<StatusEmptyResponse> Update(Item request, ServerCallContext context)
        {
            logger.LogInformation($"Update item {request.Id} for section {request.SectionKey}");

            if (request == null || string.IsNullOrWhiteSpace(request.Id))
                throw new DomainException("400", "Update received an empty or null object");

            await service.UpdateAsync(request.grpcItem_To_ItemModel());

            return new StatusEmptyResponse { Code = "200", Message = "Update sucessful" };
        }
        public override async Task<StatusEmptyResponse> Delete (Request request, ServerCallContext context)
        {
            logger.LogInformation($"Delete item {request.Id} for section {request.SectionKey}");

            if (string.IsNullOrWhiteSpace(request.SectionKey) || string.IsNullOrWhiteSpace(request.Id))
                throw new DomainException("400", "Delete item received an empty parameter");

            var response = await service.DeleteAsync(request.Id, request.SectionKey);

            return new StatusEmptyResponse { Code = "200", Message = "Delete sucessful" };
        }
    }
}
