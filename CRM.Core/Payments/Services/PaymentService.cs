using CRM.Core.Entities.Domain;
using CRM.Core.Invoices.Abstractions;
using CRM.Core.Invoices.Domain;
using CRM.Core.Invoices.Services;
using CRM.Core.Payments.Abstractions;
using CRM.Core.Payments.Domain;

namespace CRM.Core.Payments.Services
{
    public sealed class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceService _invoiceService;

        public PaymentService(
            IPaymentRepository objPaymentRepository,
            IInvoiceService objInvoiceService)
        {
            _paymentRepository =
                objPaymentRepository;

            _invoiceService =
                objInvoiceService;
        }

        ///<inheritdoc/>
        public Task<Payment?> GetPaymentByIdAsync(
            Guid objPaymentId,
            CancellationToken objToken = default)
        {
            if (objPaymentId == Guid.Empty)
            {
                return Task.FromResult<Payment?>(
                    null);
            }

            return _paymentRepository.GetPaymentByIdAsync(
                objPaymentId,
                false,
                objToken);
        }

        ///<inheritdoc/>
        public Task<List<Payment>> GetPaymentsForInvoiceAsync(
            Guid objInvoiceId,
            CancellationToken objToken = default)
        {
            return _paymentRepository.GetPaymentsForInvoiceAsync(
                objInvoiceId,
                false,
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Payment> AddPaymentAsync(
            Payment objPayment,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            if (objPayment == null)
            {
                throw new ArgumentNullException(
                    nameof(objPayment));
            }

            if (objPayment.InvoiceId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Invoice id is required.",
                    nameof(objPayment));
            }

            if (objPayment.Amount <= 0m)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(objPayment),
                    "Payment amount must be greater than zero.");
            }

            Invoice? objInvoice =
                await _invoiceService.GetInvoiceByIdAsync(
                    objPayment.InvoiceId,
                    objToken);

            if (objInvoice == null ||
                objInvoice.Entity.DeletedUtc.HasValue)
            {
                throw new InvalidOperationException(
                    "The selected invoice could not be found.");
            }

            if (objInvoice.Status ==
                InvoiceStatus.Draft)
            {
                throw new InvalidOperationException(
                    "Payments cannot be recorded against a draft invoice.");
            }

            if (objInvoice.Status ==
                InvoiceStatus.Void)
            {
                throw new InvalidOperationException(
                    "Payments cannot be recorded against a void invoice.");
            }

            List<Payment> colExistingPayments =
                await _paymentRepository
                    .GetPaymentsForInvoiceAsync(
                        objPayment.InvoiceId,
                        false,
                        objToken);

            Decimal dcmAlreadyPaid =
                colExistingPayments.Sum(
                    objExistingPayment =>
                        objExistingPayment.Amount);

            Decimal dcmOutstanding =
                Math.Max(
                    0m,
                    objInvoice.Total -
                    dcmAlreadyPaid);

            if (objPayment.Amount >
                dcmOutstanding)
            {
                throw new InvalidOperationException(
                    $"The payment cannot exceed the outstanding balance of {dcmOutstanding:C}.");
            }

            Guid objPaymentId =
                objPayment.Id == Guid.Empty
                    ? Guid.NewGuid()
                    : objPayment.Id;

            DateTime dteNow =
                DateTime.UtcNow;

            objPayment.Id =
                objPaymentId;

            objPayment.Amount =
                Decimal.Round(
                    objPayment.Amount,
                    2,
                    MidpointRounding.AwayFromZero);

            if (objPayment.PaymentDateUtc ==
                default)
            {
                objPayment.PaymentDateUtc =
                    dteNow;
            }

            objPayment.Reference =
                CleanString(
                    objPayment.Reference);

            objPayment.Notes =
                CleanString(
                    objPayment.Notes);

            objPayment.Entity =
                new CrmEntity
                {
                    Id =
                        objPaymentId,

                    EntityTypeId =
                        (Int32)PredefinedEntityType.Payment,

                    DisplayName =
                        $"Payment {objPayment.Amount:C}",

                    OwnerUserId =
                        objUserId,

                    CreatedUtc =
                        dteNow,

                    CreatedByUserId =
                        objUserId
                };

            await _paymentRepository.AddPaymentAsync(
                objPayment,
                objToken);

            await UpdateInvoiceStatusAsync(
                objInvoice.Id,
                objUserId,
                objToken);

            Payment? objSavedPayment =
                await _paymentRepository.GetPaymentByIdAsync(
                    objPaymentId,
                    false,
                    objToken);

            return objSavedPayment ??
                objPayment;
        }

        ///<inheritdoc/>
        public async Task<Boolean> DeletePaymentAsync(
            Guid objPaymentId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Payment? objPayment =
                await _paymentRepository.GetPaymentByIdAsync(
                    objPaymentId,
                    true,
                    objToken);

            if (objPayment == null ||
                objPayment.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            Guid objInvoiceId =
                objPayment.InvoiceId;

            DateTime dteNow =
                DateTime.UtcNow;

            objPayment.Entity.DeletedUtc =
                dteNow;

            objPayment.Entity.DeletedByUserId =
                objUserId;

            objPayment.Entity.UpdatedUtc =
                dteNow;

            objPayment.Entity.UpdatedByUserId =
                objUserId;

            await _paymentRepository.UpdatePaymentAsync(
                objPayment,
                objToken);

            await UpdateInvoiceStatusAsync(
                objInvoiceId,
                objUserId,
                objToken);

            return true;
        }

        private async Task UpdateInvoiceStatusAsync(
            Guid objInvoiceId,
            Guid? objUserId,
            CancellationToken objToken)
        {
            Invoice? objInvoice =
                await _invoiceService.GetInvoiceByIdAsync(
                    objInvoiceId,
                    objToken);

            if (objInvoice == null ||
                objInvoice.Status ==
                InvoiceStatus.Draft ||
                objInvoice.Status ==
                InvoiceStatus.Void)
            {
                return;
            }

            List<Payment> colPayments =
                await _paymentRepository
                    .GetPaymentsForInvoiceAsync(
                        objInvoiceId,
                        false,
                        objToken);

            Decimal dcmPaid =
                colPayments.Sum(
                    objPayment =>
                        objPayment.Amount);

            InvoiceStatus enmRequiredStatus;

            if (dcmPaid <= 0m)
            {
                enmRequiredStatus =
                    InvoiceStatus.Issued;
            }
            else if (dcmPaid >=
                     objInvoice.Total)
            {
                enmRequiredStatus =
                    InvoiceStatus.Paid;
            }
            else
            {
                enmRequiredStatus =
                    InvoiceStatus.PartPaid;
            }

            if (objInvoice.Status ==
                enmRequiredStatus)
            {
                return;
            }

            /*
             * At this point normal UpdateInvoiceAsync is Draft-only,
             * so don't use it to mutate financial status.
             *
             * We'll add a small dedicated status-update repository
             * method below.
             */
            await _invoiceService.RefreshPaymentStatusAsync(
                objInvoiceId,
                dcmPaid,
                objUserId,
                objToken);
        }

        private static String? CleanString(
            String? strValue)
        {
            return String.IsNullOrWhiteSpace(
                strValue)
                ? null
                : strValue.Trim();
        }
    }
}