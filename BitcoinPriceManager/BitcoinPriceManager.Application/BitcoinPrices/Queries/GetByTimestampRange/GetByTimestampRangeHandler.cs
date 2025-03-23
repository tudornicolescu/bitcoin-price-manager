namespace BitcoinPriceManager.Application.BitcoinPrices.Queries.GetByTimestampRange;

internal sealed class GetByTimestampRangeHandler(IBitcoinPriceRepository repository, ILogger<GetByTimestampRangeHandler> logger)
    : IQueryHandler<GetByTimestampRangeQuery, IEnumerable<GetByTimestampRangeResponse>>
{
    public async Task<IEnumerable<GetByTimestampRangeResponse>> Handle(GetByTimestampRangeQuery query, CancellationToken cancellationToken)
    {
        var bitcoinPrices = await repository.GetByTimestampRangeAsync(query.StartDate, query.EndDate, cancellationToken);

        logger.LogInformation("Prices for timeframe {StartDate} - {EndDate} fetched from the database.", query.StartDate, query.EndDate);

        return bitcoinPrices.Select(e => new GetByTimestampRangeResponse(e.Timestamp, e.Price));
    }
}