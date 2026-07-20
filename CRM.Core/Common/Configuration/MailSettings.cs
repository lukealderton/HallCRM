namespace CRM.Core.Common.Configuration
{
    public sealed class MailSettings
    {
        public String FromName { get; set; } = "";
        public String FromAddress { get; set; } = "";
        
        public String WebmasterName { get; set; } = "";
        public String WebmasterAddress { get; set; } = "";

        public String? Username { get; set; }
        public String? Password { get; set; }
        public String? Host { get; set; }
        public Int32 Port { get; set; }
    }
}
