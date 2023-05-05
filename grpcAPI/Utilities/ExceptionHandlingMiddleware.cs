using grpc.Domain.Others;
using grpcAPI.Protos;
using System.Net;

namespace grpcAPI.Utilities
{
    public class ExceptionHandlingMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _next = next ?? throw new ArgumentException(nameof(next));
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (DomainException de)
            {
                _logger.LogError(de, de.Message);
                var status = new StatusEmptyResponse
                {
                    Code = de.Code,
                    Message = de.Message
                };

                await RewriteBodyAsync(httpContext.Response, status);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                var status = new StatusEmptyResponse
                {
                    Code = "500",
                    Message = "Server error"
                };

                await RewriteBodyAsync(httpContext.Response, status);
            }
        }
        private async Task RewriteBodyAsync(HttpResponse httpResponse, StatusEmptyResponse status)
        {
            httpResponse.ContentType = "application/json; charset=utf-8";
            httpResponse.StatusCode = (int)HttpStatusCode.OK;

            await httpResponse.WriteAsJsonAsync(status);
        }
    }
}
