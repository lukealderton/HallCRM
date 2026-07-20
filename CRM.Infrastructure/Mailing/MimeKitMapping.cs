using CRM.Core.Mailing.Domain;
using MimeKit;

namespace CRM.Infrastructure.Mailing
{
    internal static class MimeKitMapping
    {
        public static MailboxAddress ToMailboxAddress(this EmailAddress objAddress)
        {
            if (String.IsNullOrWhiteSpace(objAddress.Name))
            {
                return MailboxAddress.Parse(objAddress.Address);
            }

            return new MailboxAddress(objAddress.Name, objAddress.Address);
        }

        public static IEnumerable<MailboxAddress> ToMailboxAddresses(this IEnumerable<EmailAddress> colAddresses)
        {
            foreach (EmailAddress objAddress in colAddresses)
            {
                yield return objAddress.ToMailboxAddress();
            }
        }
    }
}
