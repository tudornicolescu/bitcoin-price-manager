namespace BitcoinPriceManager.SharedKernel.Extensions;

public static class DateTimeExtensions
{
    public static DateTime NormalizeToHour(this DateTime inputDateTime)
    {
        return new DateTime(inputDateTime.Year, inputDateTime.Month, inputDateTime.Day, inputDateTime.Hour, 0, 0, DateTimeKind.Utc);
    }

    public static long ToUnixTimeMilliseconds(this DateTime dateTime)
    {
        var dateTimeOffset = new DateTimeOffset(dateTime.NormalizeToHour().ToUniversalTime());

        return dateTimeOffset.ToUnixTimeMilliseconds();
    }

    public static long ToUnixTimeSeconds(this DateTime dateTime)
    {
        var dateTimeOffset = new DateTimeOffset(dateTime.NormalizeToHour().ToUniversalTime());

        return dateTimeOffset.ToUnixTimeSeconds();
    }
}
