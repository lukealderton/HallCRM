using CRM.Core.Payments.Abstractions;
using CRM.Core.Payments.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Payments.Repositories
{
    public sealed class PaymentRepository : IPaymentRepository
    {
        private readonly IDbContextFactory<CRMDbContext> _dbContextFactory;

        public PaymentRepository(
            IDbContextFactory<CRMDbContext> objDbContextFactory)
        {
            _dbContextFactory =
                objDbContextFactory;
        }

        ///<inheritdoc/>
        public async Task<Payment?> GetPaymentByIdAsync(
            Guid objPaymentId,
            Boolean blnAsTracking = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objContext =
                await _dbContextFactory.CreateDbContextAsync(
                    objToken);

            IQueryable<Payment> objQuery =
                objContext
                    .Set<Payment>()
                    .Include(objPayment =>
                        objPayment.Entity)
                    .Include(objPayment =>
                        objPayment.Invoice)
                        .ThenInclude(objInvoice =>
                            objInvoice.Entity);

            if (!blnAsTracking)
            {
                objQuery =
                    objQuery.AsNoTracking();
            }

            return await objQuery
                .FirstOrDefaultAsync(
                    objPayment =>
                        objPayment.Id ==
                        objPaymentId,
                    objToken);
        }

        ///<inheritdoc/>
        public async Task<List<Payment>> GetPaymentsForInvoiceAsync(
            Guid objInvoiceId,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objContext =
                await _dbContextFactory.CreateDbContextAsync(
                    objToken);

            IQueryable<Payment> objQuery =
                objContext
                    .Set<Payment>()
                    .AsNoTracking()
                    .Include(objPayment =>
                        objPayment.Entity)
                    .Where(objPayment =>
                        objPayment.InvoiceId ==
                        objInvoiceId);

            if (!blnIncludeDeleted)
            {
                objQuery =
                    objQuery.Where(
                        objPayment =>
                            !objPayment.Entity.DeletedUtc.HasValue);
            }

            return await objQuery
                .OrderByDescending(objPayment =>
                    objPayment.PaymentDateUtc)
                .ThenByDescending(objPayment =>
                    objPayment.Entity.CreatedUtc)
                .ToListAsync(
                    objToken);
        }

        ///<inheritdoc/>
        public async Task AddPaymentAsync(
            Payment objPayment,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objContext =
                await _dbContextFactory.CreateDbContextAsync(
                    objToken);

            objPayment.Invoice =
                null!;

            objContext
                .Set<Payment>()
                .Add(
                    objPayment);

            await objContext.SaveChangesAsync(
                objToken);
        }

        ///<inheritdoc/>
        public async Task UpdatePaymentAsync(
            Payment objPayment,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objContext =
                await _dbContextFactory.CreateDbContextAsync(
                    objToken);

            Payment? objExistingPayment =
                await objContext
                    .Set<Payment>()
                    .Include(objPayment =>
                        objPayment.Entity)
                    .FirstOrDefaultAsync(
                        objExisting =>
                            objExisting.Id ==
                            objPayment.Id,
                        objToken);

            if (objExistingPayment == null)
            {
                return;
            }

            objExistingPayment.Amount =
                objPayment.Amount;

            objExistingPayment.PaymentDateUtc =
                objPayment.PaymentDateUtc;

            objExistingPayment.Method =
                objPayment.Method;

            objExistingPayment.Reference =
                objPayment.Reference;

            objExistingPayment.Notes =
                objPayment.Notes;

            objExistingPayment.Entity.DisplayName =
                objPayment.Entity.DisplayName;

            objExistingPayment.Entity.UpdatedUtc =
                objPayment.Entity.UpdatedUtc;

            objExistingPayment.Entity.UpdatedByUserId =
                objPayment.Entity.UpdatedByUserId;

            objExistingPayment.Entity.DeletedUtc =
                objPayment.Entity.DeletedUtc;

            objExistingPayment.Entity.DeletedByUserId =
                objPayment.Entity.DeletedByUserId;

            await objContext.SaveChangesAsync(
                objToken);
        }
    }
}