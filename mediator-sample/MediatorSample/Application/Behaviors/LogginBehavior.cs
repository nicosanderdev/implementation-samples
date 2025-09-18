using System.Diagnostics;
using MediatorSample.Domain.Interfaces;

namespace MediatorSample.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = typeof(TRequest).Name;
        Console.WriteLine($"[START] Handling {requestName}");

        var stopwatch = Stopwatch.StartNew();
        
        var response = await next();

        stopwatch.Stop();
        Console.WriteLine($"[END] Handled {requestName} in {stopwatch.ElapsedMilliseconds}ms");

        return response;
    }
}