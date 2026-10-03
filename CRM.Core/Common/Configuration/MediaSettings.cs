namespace CRM.Core.Common.Configuration
{
    public class MediaSettings
    {
        public String Root { get; set; } = "App_Data/Media";

        public Int64 MaxFileSizeBytes { get; set; } = 100 * 1024 * 1024;

        public Int32 MaxFilesPerUpload { get; set; } = 10;
    }
}
