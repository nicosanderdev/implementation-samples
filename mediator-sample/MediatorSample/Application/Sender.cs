using MediatorSample.Domain.Interfaces;

namespace MediatorSample.Application;

public class Sender : ISender
{
    private readonly IServiceProvider _serviceProvider;

    public Sender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        
        var handler = _serviceProvider.GetRequiredService(handlerType);
        
        var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var pipelines = _serviceProvider.GetServices(pipelineType)
            .Cast<object>()
            .Reverse()
            .ToList();

        Task<TResponse> HandlerDelegate() => ((dynamic)handler).Handle((dynamic)request, cancellationToken);

        var chainedDelegate = pipelines.Aggregate(
            (RequestHandlerDelegate<TResponse>)HandlerDelegate,
            (next, pipeline) => () => ((dynamic)pipeline).Handle((dynamic)request, cancellationToken, next)
        );
        
        return chainedDelegate();
    }
}