using MediatR;

namespace BitcoinPriceManager.SharedKernel.CQRS;

public interface IQuery : IQuery<Unit>
{ }

public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
{ }
