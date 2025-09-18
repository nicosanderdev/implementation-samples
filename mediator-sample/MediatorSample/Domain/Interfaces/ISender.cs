namespace MediatorSample.Domain.Interfaces;

public interface ISender
{
    /// <summary>
    /// Sends a request to be handled by a single handler.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the response from the handler.</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}