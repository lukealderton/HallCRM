namespace CRM.Core.Mailing.Domain
{
    public sealed class EmailAddress
    {
        public EmailAddress(String strAddress, String? strName = null)
        {
            Address = strAddress?.Trim() ?? String.Empty;
            Name = strName?.Trim();
        }

        public String Address { get; }
        public String? Name { get; }
    }
}
