namespace CRM.Core.Users.Domain
{
    public sealed class UserProfile
    {
        public Guid Id { get; set; }

        public string? Forename { get; set; }
        public string? Surname { get; set; }
        public string? Mobile { get; set; }

        // Address
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Town { get; set; }
        public string? County { get; set; }
        public string? Postcode { get; set; }
        public string? Country { get; set; }

        // Payment (provider customer identifier only, e.g. Stripe "cus_...")
        public string? StripeCustomerId  { get; set; }

        public DateTimeOffset? CreatedUtc { get; set; }
        public DateTimeOffset? UpdatedUtc { get; set; }
    }

}
