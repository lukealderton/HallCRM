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

        /// <summary>
        /// Issues an invoice by setting its status to "Issued" and updating the issue and due dates.
        /// </summary>
        /// <param name="objInvoiceId"></param>
        /// <param name="dteIssueDateUtc"></param>
        /// <param name="dteDueDateUtc"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Invoice?> IssueInvoiceAsync(
            Guid objInvoiceId,
            DateTime dteIssueDateUtc,
            DateTime dteDueDateUtc,
            Guid? objUserId = null,
            CancellationToken objToken = default);
    }
}