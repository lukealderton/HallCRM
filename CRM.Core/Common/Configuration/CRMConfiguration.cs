namespace CRM.Core.Common.Configuration
{
    public class CRMConfiguration
    {
        public String ApplicationName { get; set; } = "101 CRM 2026";
        public String ApplicationNameShort { get; set; } = "CRM";

        public String DataConnectionString { get; set; } = "";
        public String ManagementConnectionString { get; set; } = "";

        public MailSettings Mail { get; set; } = new MailSettings();

        public GoogleGeocodingSettings GoogleGeocoding { get; set; } = new GoogleGeocodingSettings();

        public ScheduledJobSettings ScheduledJobs { get; set; } = new ScheduledJobSettings();

        public MediaSettings MediaSettings { get; set; } = new MediaSettings();
    }
}
