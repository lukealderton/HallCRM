namespace CRM.Core.Common.Configuration
{
    public sealed class ScheduledJobSettings
    {
        /// <summary>
        /// Format: "HH:mm" (e.g., "02:00" for 2 AM)
        /// </summary>
        public string? DailyRunTime { get; set; }
    }
}
