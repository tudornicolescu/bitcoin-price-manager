using BitcoinPriceManager.Application.BitcoinPrices.Commands.FetchPriceByTimestamp;

namespace BitcoinPriceManager.Application.BitcoinPrices.Validators;

/// <summary>
/// Fluent Validation class for FetchPriceByTimestampCommand
/// </summary>
public class FecthPriceByTimestampCommandValidator : AbstractValidator<FetchPriceByTimestampCommand>
{
    public FecthPriceByTimestampCommandValidator()
    {
        // Timestamp must not the default value, null or in the future
        RuleFor(command => command.Timestamp)
            .NotNull().WithMessage("Timestamp must have a value.")
            .NotEqual(DateTime.MinValue).WithMessage("Timestamp cannot be empty.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Timestamp cannot be in the future.");
    }
}