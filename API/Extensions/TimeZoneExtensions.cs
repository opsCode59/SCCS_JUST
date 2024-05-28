namespace API.Extensions
    {
    public class TimeZoneExtensions
        {
        public DateTime ConvertToKSA( DateTime utcTime )
            {
            // Get the time zone info for KSA (Arab Standard Time)
            TimeZoneInfo ksaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");

            // Convert the UTC time to KSA time
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, ksaTimeZone);
            }
        }
    }
