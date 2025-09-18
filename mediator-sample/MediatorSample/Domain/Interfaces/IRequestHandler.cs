﻿using MediatorSample.Domain.Struct;

namespace MediatorSample.Domain.Interfaces;

/// <summary>
/// Defines a handler for a request.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
/// <typeparam name="TResponse">The type of response from the handler.</typeparam>
public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a handler for a request with no return value.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit> where TRequest : IRequest<Unit>
{
}