namespace CRM.Core.Mailing.Domain
{
    public sealed class ExceptionMailDetail
    {
        public String? RequestUrl { get; set; }
        public String? Host { get; set; }
        public String? Path { get; set; }
        public Int32? StatusCode { get; set; }

        public String? RemoteIp { get; set; }
        public String? UserAgent { get; set; }

        public DateTime DtmUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional extra detail supplied by caller (e.g. "MemberId=..., ListingId=...").
        /// </summary>
        public String? ExtraMessage { get; set; }
    }
}
