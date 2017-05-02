using System;

namespace FactorioSupervisor.Helpers
{
    public static class TimeHelpers
    {
        public static string GetTimeSpanDuration(string input)
        {
            var dateTimeNow = DateTime.Now;
            var dateTimeLastUpdate = DateTime.Parse(input);

            var timeSpan = dateTimeNow - dateTimeLastUpdate;

            if (timeSpan.Days == 1)
                return $"1 day ago";

            if (timeSpan.Days > 1)
                return $"{timeSpan.Days} days ago";

            if (timeSpan.Days == 0 && timeSpan.Hours <= 24)
                return $"{timeSpan.Hours} hours ago";

            if (timeSpan.Minutes < 60 && timeSpan.Hours == 0 && timeSpan.Days == 0)
                return $"{timeSpan} minutes ago";

            if (timeSpan.Days >= 365)
                return $"1 year ago";

            return $"{timeSpan.Days}d {timeSpan.Hours}h {timeSpan.Minutes}m ago";
        }
    }
}
