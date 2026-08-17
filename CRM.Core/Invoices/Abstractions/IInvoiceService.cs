using CRM.Core.Invoices.Domain;

namespace CRM.Core.Invoices.Abstractions
{
    public interface IInvoiceService
    {
        /// <summary>
        /// Gets an invoice by its unique identifier.
        /// </summary>
        Task<Invoice?> GetInvoiceByIdAsync(
            Guid objInvoiceId,
            CancellationToken objToken = default);

        /// <summary>
        /// Gets invoices matching the supplied criteria.
        /// </summary>
        Task<List<Invoice>> GetInvoicesAsync(
            String? strSearch = null,
            InvoiceStatus? enmStatus = null,
            Guid? objJobId = null,
            Guid? objCompanyId = null,
            Boolean blnOutstandingOnly = false,
            Boolean blnOverdueOnly = false,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        /// <summary>
        /// Creates a draft invoice from a job.
        /// </summary>
        Task<Invoice> CreateFromJobAsync(
            Guid objJobId,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        /// <summary>
        /// Adds a new invoice.
        /// </summary>
        Task<Invoice> AddInvoiceAsync(
            Invoice objInvoice,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        /// <summary>
        /// Updates an existing invoice.
        /// </summary>
        Task<Invoice?> UpdateInvoiceAsync(
            Invoice objInvoice,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        /// <summary>
        /// Archives an invoice.
        /// </summary>
        Task<Boolean> ArchiveInvoiceAsync(
            Guid objInvoiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        /// <summary>
        /// Restores an archived invoice.
        /// </summary>
        Task<Boolean> RestoreInvoiceAsync(
            Guid objInvoiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        /// <summary>
        /// Soft deletes an invoice.
        /// </summary>
        Task<Boolean> DeleteInvoiceAsync(
            Guid objInvoiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        /// <summary>
        /// Voids an invoice, setting its status to 'Voided' and preventing further payments or modifications.
        /// </summary>
        /// <param name="objInvoiceId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> VoidInvoiceAsync(
            Guid objInvoiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        /// <summary>
        /// Issues an invoice, setting its status to 'Issued' and optionally updating the issue and due dates.
        /// </summary>
        /// <param name="objInvoiceId"></param>
        /// <param name="dteIssueDateUtc"></param>
        /// <param name="dteDueDateUtc"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Invoice?> IssueInvoiceAsync(
            Guid objInvoiceId,
            DateTime? dteIssueDateUtc = null,
            DateTime? dteDueDateUtc = null,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        /// <summary>
        /// Refreshes the payment status of an invoice based on the amount paid, updating its status to 'Paid', 'Partially Paid', or 'Unpaid' as appropriate.
        /// </summary>
        /// <param name="objInvoiceId"></param>
        /// <param name="dcmAmountPaid"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> RefreshPaymentStatusAsync(
            Guid objInvoiceId,
            Decimal dcmAmountPaid,
            Guid? objUserId = null,
            CancellationToken objToken = default);
    }
}