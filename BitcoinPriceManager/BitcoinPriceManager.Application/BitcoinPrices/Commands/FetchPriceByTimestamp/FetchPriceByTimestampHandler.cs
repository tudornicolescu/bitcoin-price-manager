namespace BitcoinPriceManager.Application.BitcoinPrices.Commands.FetchPriceByTimestamp;

internal sealed class FetchPriceByTimestampHandler(IFetchBitcoinPriceService fetchBitcoinPrice,
    IBitcoinPriceRepository repository,
    ILogger<FetchPriceByTimestampHandler> logger)
    : ICommandHandler<FetchPriceByTimestampCommand, FetchPriceByTimestampResponse>
{
    public async Task<FetchPriceByTimestampResponse> Handle(FetchPriceByTimestampCommand command, CancellationToken cancellationToken)
    {
        var aggregatedPrice = await fetchBitcoinPrice.GetByTimestampAsync(command.Timestamp, cancellationToken);
        if (aggregatedPrice == 0)
        {
            logger.LogError("No prices were fetched from external APIs.");
            throw new InvalidOperationException("Price could not be fetched from external APIs.");
        }

        var bitcoinPrice = new BitcoinPrice(command.Timestamp, aggregatedPrice);

        await repository.AddAsync(bitcoinPrice, cancellationToken);

        return new FetchPriceByTimestampResponse(bitcoinPrice.Id, bitcoinPrice.Timestamp, bitcoinPrice.Price);
    }
}