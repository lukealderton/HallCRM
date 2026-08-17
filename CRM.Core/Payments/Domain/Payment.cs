using CRM.Core.Entities.Domain;
using CRM.Core.Invoices.Domain;

namespace CRM.Core.Payments.Domain
{
    public sealed class Payment : CrmEntityRecord
    {
        public Guid InvoiceId { get; set; }

        public Invoice Invoice { get; set; } =
            null!;

        public Decimal Amount { get; set; }

        public DateTime PaymentDateUtc { get; set; }

        public PaymentMethod Method { get; set; } =
            PaymentMethod.BankTransfer;

        public String? Reference { get; set; }

        public String? Notes { get; set; }
    }
}