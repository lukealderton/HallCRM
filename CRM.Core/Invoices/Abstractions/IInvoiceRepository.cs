using CRM.Core.Invoices.Domain;

namespace CRM.Core.Invoices.Abstractions
{
    public interface IInvoiceRepository
    {
        /// <summary>
        /// Gets an invoice by its unique identifier.
        /// </summary>
        Task<Invoice?> GetInvoiceByIdAsync(
            Guid objInvoiceId,
            Boolean blnAsTracking = false,
            CancellationToken objToken = default);

        /// <summary>
        /// Gets invoices matching the supplied criteria.
        /// </summary>
        Task<List<Invoice>> GetInvoicesAsync(
            String? strSearch = null,
            InvoiceStatus? enmStatus = null,
            Guid? objJobId = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        /// <summary>
        /// Adds a new invoice.
        /// </summary>
        Task AddInvoiceAsync(
            Invoice objInvoice,
            CancellationToken objToken = default);

        /// <summary>
        /// Updates an existing invoice.
        /// </summary>
        Task UpdateInvoiceAsync(
            Invoice objInvoice,
            CancellationToken objToken = default);

        /// <summary>
        /// Determines whether an invoice number already exists.
        /// </summary>
        Task<Boolean> InvoiceNumberExistsAsync(
            String strInvoiceNumber,
            Guid? objExcludeInvoiceId = null,
            CancellationToken objToken = default);
    }
}