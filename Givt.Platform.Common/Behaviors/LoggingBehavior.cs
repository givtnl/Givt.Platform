using MediatR;
using Newtonsoft.Json;
using Serilog.Sinks.Http.Logger;

namespace Givt.Donations.Infrastructure.Behaviors;

public record LoggingBehavior<TRequest, TResponse>(ILog Logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (Exception e)
        {
            Logger.Information($"Received error while handling request {typeof(TRequest).Name}" + JsonConvert.SerializeObject(new
            {
                Request = request,
                Exception = e.ToString()
            }, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new LoggingContractResolver()
            }));

            throw;
        }
    }
}
