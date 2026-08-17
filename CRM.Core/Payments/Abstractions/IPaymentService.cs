using CRM.Core.Payments.Domain;

namespace CRM.Core.Payments.Abstractions
{
    public interface IPaymentService
    {
        Task<Payment?> GetPaymentByIdAsync(
            Guid objPaymentId,
            CancellationToken objToken = default);

        Task<List<Payment>> GetPaymentsForInvoiceAsync(
            Guid objInvoiceId,
            CancellationToken objToken = default);

        Task<Payment> AddPaymentAsync(
            Payment objPayment,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        Task<Boolean> DeletePaymentAsync(
            Guid objPaymentId,
            Guid? objUserId = null,
            CancellationToken objToken = default);
    }
}