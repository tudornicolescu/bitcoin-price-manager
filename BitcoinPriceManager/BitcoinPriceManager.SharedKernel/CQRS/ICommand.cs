using MediatR;

namespace BitcoinPriceManager.SharedKernel.CQRS;

public interface ICommand : ICommand<Unit>
{ }

public interface ICommand<out TResponse> : IRequest<TResponse>
{ }
