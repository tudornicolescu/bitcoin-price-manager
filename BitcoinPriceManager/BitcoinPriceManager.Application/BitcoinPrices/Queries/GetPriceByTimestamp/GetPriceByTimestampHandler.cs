using BitcoinPriceManager.Application.BitcoinPrices.Commands.FetchPriceByTimestamp;
using MediatR;

namespace BitcoinPriceManager.Application.BitcoinPrices.Queries.GetPriceByTimestamp;

internal sealed class GetPriceByTimestampHandler(IBitcoinPriceRepository repository,
    IMediator mediator,
    ILogger<GetPriceByTimestampHandler> logger)
    : IQueryHandler<GetPriceByTimestampQuery, GetPriceByTimestampResponse>
{
    public async Task<GetPriceByTimestampResponse> Handle(GetPriceByTimestampQuery query, CancellationToken cancellationToken)
    {
        // check the database for the existing price
        // if found, return it
        var bitcoinPrice = await repository.GetByTimestampAsync(query.Timestamp.NormalizeToHour(), cancellationToken);
        if (bitcoinPrice is not null)
        {
            logger.LogInformation("Price for timestamp {Timestamp} retrieved from the database.", query.Timestamp);

            return new GetPriceByTimestampResponse(bitcoinPrice.Price);
        }

        logger.LogInformation("Price for timestamp {Timestamp} will be fetched from external services...", query.Timestamp);

        // dispatch command to fetch the price from external APIs
        // return the aggregated price
        var fetchPriceByTimestampResponse = await mediator.Send(new FetchPriceByTimestampCommand(query.Timestamp), cancellationToken);

        return new GetPriceByTimestampResponse(fetchPriceByTimestampResponse.Price);
    }
}