using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Core.Invoices.Domain
{
    public sealed class InvoiceLine
    {
        public Guid Id { get; set; }

        public Guid InvoiceId { get; set; }

        public Invoice Invoice { get; set; } =
            null!;

        public Guid? ServiceId { get; set; }

        public CRM.Core.Services.Domain.Service? Service { get; set; }

        public Int32 SortOrder { get; set; }

        public String Description { get; set; } =
            String.Empty;

        public Decimal Quantity { get; set; } =
            1m;

        public Decimal UnitPrice { get; set; }

        [NotMapped]
        public Decimal LineTotal =>
            Quantity *
            UnitPrice;
    }
}