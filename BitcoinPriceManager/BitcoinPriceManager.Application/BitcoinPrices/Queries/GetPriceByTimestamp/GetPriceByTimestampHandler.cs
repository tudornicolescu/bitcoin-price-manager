using BitcoinPriceManager.Application.Data;
using BitcoinPriceManager.SharedKernel.Extensions;

namespace BitcoinPriceManager.Application.BitcoinPrices.Queries.GetPriceByTimestamp;

internal sealed class GetPriceByTimestampHandler(IBitcoinPriceRepository repository)
    : IQueryHandler<GetPriceByTimestampQuery, GetPriceByTimestampResponse>
{
    public async Task<GetPriceByTimestampResponse> Handle(GetPriceByTimestampQuery query, CancellationToken cancellationToken)
    {
        // check the database for the existing price
        // if found, return it
        var bitcoinPrice = await repository.GetByTimestampAsync(query.Timestamp.NormalizeToHour(), cancellationToken);
        if (bitcoinPrice is not null)
        {
            return new GetPriceByTimestampResponse { Price = bitcoinPrice.Price };
        }

        // dispatch command to fetch the price from external APIs
        // return the aggregated price
        // TODO

        return new GetPriceByTimestampResponse();
    }
}
