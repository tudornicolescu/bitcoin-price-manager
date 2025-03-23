namespace BitcoinPriceManager.Application.BitcoinPrices.Commands.FetchPriceByTimestamp;

public sealed record FetchPriceByTimestampCommand(DateTime Timestamp)
    : ICommand<FetchPriceByTimestampResponse>;