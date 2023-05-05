using Grpc.Core;
using Grpc.Core.Interceptors;
using grpc.Domain.Others;
using grpcAPI.Protos;

namespace grpcAPI.Utilities
{
    public class GrpcExceptionInterceptorHandler : Interceptor
    {
        private readonly ILogger<GrpcExceptionInterceptorHandler> logger;

        public GrpcExceptionInterceptorHandler(ILogger<GrpcExceptionInterceptorHandler> Logger)
        {
            logger = Logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await base.UnaryServerHandler(request, context, continuation);
            }
            catch (DomainException de)
            {
                var status = new StatusEmptyResponse
                {
                    Code = de.Code,
                    Message = de.Message
                };
                return MapResponse<TRequest, TResponse>(status);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);

                var status = new StatusEmptyResponse
                {
                    Code = "500",
                    Message = "Server error"
                };

                return MapResponse<TRequest, TResponse>(status);
            }
        }

        private TResponse MapResponse<TRequest, TResponse>(StatusEmptyResponse status)
        {
            var concreteResponse = Activator.CreateInstance<TResponse>();

            concreteResponse?.GetType().GetProperty(nameof(status.Code))?.SetValue(concreteResponse, status.Code);

            concreteResponse?.GetType().GetProperty(nameof(status.Message))?.SetValue(concreteResponse, status.Message);

            return concreteResponse;
        }
    }
}
