namespace CRM.Core.Invoices.Abstractions
{
    public interface IInvoiceDocumentService
    {
        Task<Byte[]> GenerateInvoiceAsync(
            Guid objInvoiceId,
            CancellationToken objToken = default);
    }
}