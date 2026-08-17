using CRM.Core.Companies.Domain;
using CRM.Core.Contacts.Domain;
using CRM.Core.Entities.Domain;
using CRM.Core.Jobs.Domain;

namespace CRM.Core.Invoices.Domain
{
    public sealed class Invoice : CrmEntityRecord
    {
        public Guid JobId { get; set; }

        public Job Job { get; set; } =
            null!;

        public Guid? CompanyId { get; set; }

        public Company? Company { get; set; }

        public Guid? ContactId { get; set; }

        public Contact? Contact { get; set; }

        public String InvoiceNumber { get; set; } =
            String.Empty;

        public InvoiceStatus Status { get; set; } =
            InvoiceStatus.Draft;

        public DateTime? IssueDateUtc { get; set; }

        public DateTime? DueDateUtc { get; set; }

        public String? CustomerName { get; set; }

        public String? AddressLine1 { get; set; }

        public String? AddressLine2 { get; set; }

        public String? Town { get; set; }

        public String? County { get; set; }

        public String? Postcode { get; set; }

        public String? Notes { get; set; }

        public ICollection<InvoiceLine> Lines { get; set; } =
            new List<InvoiceLine>();

        public Decimal Subtotal =>
            Lines.Sum(
                objLine =>
                    objLine.LineTotal);

        public Decimal Total =>
            Subtotal;
    }
}