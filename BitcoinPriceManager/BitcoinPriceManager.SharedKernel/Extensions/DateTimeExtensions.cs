namespace BitcoinPriceManager.SharedKernel.Extensions;

public static class DateTimeExtensions
{
    public static DateTime NormalizeToHour(this DateTime inputDateTime)
    {
        return new DateTime(inputDateTime.Year, inputDateTime.Month, inputDateTime.Day, inputDateTime.Hour, 0, 0, DateTimeKind.Utc);
    }
}
