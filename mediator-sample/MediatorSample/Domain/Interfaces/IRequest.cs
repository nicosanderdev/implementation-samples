using MediatorSample.Domain.Struct;

namespace MediatorSample.Domain.Interfaces;

/// <summary>
/// Represents a request that returns a value.
/// </summary>
/// <typeparam name="TResponse">The type of the response from the handler.</typeparam>
public interface IRequest<out TResponse> { }

/// <summary>
/// Represents a request that does not return a value.
/// It inherits from IRequest<Unit> to unify the handling logic.
/// </summary>
public interface IRequest : IRequest<Unit> { }