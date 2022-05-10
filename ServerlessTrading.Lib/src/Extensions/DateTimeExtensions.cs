using System.Globalization;

namespace ServerlessTrading.Lib.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToIso8601(this DateTime dateTime)
        {
            return dateTime.ToString("s", CultureInfo.InvariantCulture);
        }
    }
}
