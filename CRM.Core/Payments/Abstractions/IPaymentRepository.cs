using CRM.Core.Payments.Domain;

namespace CRM.Core.Payments.Abstractions
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetPaymentByIdAsync(
            Guid objPaymentId,
            Boolean blnAsTracking = false,
            CancellationToken objToken = default);

        Task<List<Payment>> GetPaymentsForInvoiceAsync(
            Guid objInvoiceId,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        Task AddPaymentAsync(
            Payment objPayment,
            CancellationToken objToken = default);

        Task UpdatePaymentAsync(
            Payment objPayment,
            CancellationToken objToken = default);
    }
}