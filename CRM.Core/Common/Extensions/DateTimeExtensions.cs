namespace CRM.Core.Common.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns a readable string of the provided date based on how long ago it happened e.g. 1 minute ago, 1 year ago, etc.
        /// </summary>
        /// <param name="objDateTime"></param>
        /// <returns></returns>
        public static String GetRelativeTime(this DateTime objDateTime)
        {
            TimeSpan tsDelta = DateTime.UtcNow - objDateTime.ToUniversalTime();

            if (tsDelta.TotalMinutes < 1)
            {
                return tsDelta.Seconds < 2 ? "Just now" : tsDelta.Seconds + " seconds ago";
            }

            if (tsDelta.TotalMinutes < 2)
            {
                return "1 minute ago";
            }

            if (tsDelta.TotalMinutes < 60)
            {
                return tsDelta.Minutes + " minutes ago";
            }

            if (tsDelta.TotalMinutes < 90)
            {
                return "1 hour ago";
            }

            if (tsDelta.TotalHours < 24)
            {
                return tsDelta.Hours + " hours ago";
            }

            if (tsDelta.TotalHours < 48)
            {
                return "Yesterday";
            }

            if (tsDelta.TotalDays < 30)
            {
                return tsDelta.Days + " days ago";
            }

            if (tsDelta.TotalDays < 365)
            {
                Int32 intMonths = Convert.ToInt32(Math.Floor((Double)tsDelta.TotalDays / 30));
                return intMonths <= 1 ? "1 month ago" : intMonths + " months ago";
            }
            else
            {
                Int32 intYears = Convert.ToInt32(Math.Floor((Double)tsDelta.TotalDays / 365));
                return intYears <= 1 ? "1 year ago" : intYears + " years ago";
            }
        }

        /// <summary>
        /// Returns a readable string of the provided date based on how long ago it happened e.g. 1 minute ago, 1 year ago, etc.
        /// </summary>
        /// <param name="objDateTime"></param>
        /// <returns></returns>
        public static String GetRelativeTime(this DateTimeOffset objDateTime)
        {
            TimeSpan tsDelta = DateTime.UtcNow - objDateTime.ToUniversalTime();

            if (tsDelta.TotalMinutes < 1)
            {
                return tsDelta.Seconds < 2 ? "Just now" : tsDelta.Seconds + " seconds ago";
            }

            if (tsDelta.TotalMinutes < 2)
            {
                return "1 minute ago";
            }

            if (tsDelta.TotalMinutes < 60)
            {
                return tsDelta.Minutes + " minutes ago";
            }

            if (tsDelta.TotalMinutes < 90)
            {
                return "1 hour ago";
            }

            if (tsDelta.TotalHours < 24)
            {
                return tsDelta.Hours + " hours ago";
            }

            if (tsDelta.TotalHours < 48)
            {
                return "Yesterday";
            }

            if (tsDelta.TotalDays < 30)
            {
                return tsDelta.Days + " days ago";
            }

            if (tsDelta.TotalDays < 365)
            {
                Int32 intMonths = Convert.ToInt32(Math.Floor((Double)tsDelta.TotalDays / 30));
                return intMonths <= 1 ? "1 month ago" : intMonths + " months ago";
            }
            else
            {
                Int32 intYears = Convert.ToInt32(Math.Floor((Double)tsDelta.TotalDays / 365));
                return intYears <= 1 ? "1 year ago" : intYears + " years ago";
            }
        }
    }
}
