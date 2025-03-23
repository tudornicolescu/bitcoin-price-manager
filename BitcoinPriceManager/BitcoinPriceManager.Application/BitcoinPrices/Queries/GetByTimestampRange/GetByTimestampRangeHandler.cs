using BitcoinPriceManager.Application.Data;

namespace BitcoinPriceManager.Application.BitcoinPrices.Queries.GetByTimestampRange;

internal sealed class GetByTimestampRangeHandler(IBitcoinPriceRepository repository)
    : IQueryHandler<GetByTimestampRangeQuery, IEnumerable<GetByTimestampRangeResponse>>
{
    public async Task<IEnumerable<GetByTimestampRangeResponse>> Handle(GetByTimestampRangeQuery query, CancellationToken cancellationToken)
    {
        var bitcoinPrices = await repository.GetByTimestampRangeAsync(query.StartDate, query.EndDate, cancellationToken);

        return bitcoinPrices.Select(e => new GetByTimestampRangeResponse
        {
            Timestamp = e.Timestamp,
            Price = e.Price
        });
    }
}
