using CRM.Core.Invoices.Abstractions;
using CRM.Core.Invoices.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CRM.Infrastructure.Invoices.Repositories
{
    public sealed class InvoiceRepository : IInvoiceRepository
    {
        private readonly IDbContextFactory<CRMDbContext> _dbContextFactory;

        public InvoiceRepository(
            IDbContextFactory<CRMDbContext> objDbContextFactory)
        {
            _dbContextFactory =
                objDbContextFactory;
        }

        ///<inheritdoc/>
        public async Task<Invoice?> GetInvoiceByIdAsync(
            Guid objInvoiceId,
            Boolean blnAsTracking = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objContext =
                await _dbContextFactory.CreateDbContextAsync(
                    objToken);

            IQueryable<Invoice> objQuery =
                objContext
                    .Set<Invoice>()
                    .Include(objInvoice =>
                        objInvoice.Entity)
                    .Include(objInvoice =>
                        objInvoice.Job)
                        .ThenInclude(objJob =>
                            objJob.Entity)
                    .Include(objInvoice =>
                        objInvoice.Company)
                    .Include(objInvoice =>
                        objInvoice.Contact)
                            .ThenInclude(objContact =>
                                objContact!.Entity)
                    .Include(objInvoice =>
                        objInvoice.Lines)
                        .ThenInclude(objLine =>
                            objLine.Service);

            if (!blnAsTracking)
            {
                objQuery =
                    objQuery.AsNoTracking();
            }

            return await objQuery
                .FirstOrDefaultAsync(
                    objInvoice =>
                        objInvoice.Id ==
                        objInvoiceId,
                    objToken);
        }

        ///<inheritdoc/>
        public async Task<List<Invoice>> GetInvoicesAsync(
            String? strSearch = null,
            InvoiceStatus? enmStatus = null,
            Guid? objJobId = null,
            Guid? objCompanyId = null,
            Boolean blnOutstandingOnly = false,
            Boolean blnOverdueOnly = false,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objContext =
                await _dbContextFactory.CreateDbContextAsync(
                    objToken);

            IQueryable<Invoice> objQuery =
                objContext
                    .Set<Invoice>()
                    .AsNoTracking()
                    .Include(objInvoice =>
                        objInvoice.Entity)
                    .Include(objInvoice =>
                        objInvoice.Job)
                    .Include(objInvoice =>
                        objInvoice.Company)
                    .Include(objInvoice =>
                        objInvoice.Contact)
                        .ThenInclude(objContact =>
                            objContact!.Entity)
                    .Include(objInvoice =>
                        objInvoice.Lines)
                    .Include(objInvoice =>
                        objInvoice.Payments)
                        .ThenInclude(objPayment =>
                            objPayment.Entity);

            if (!blnIncludeDeleted)
            {
                objQuery =
                    objQuery.Where(
                        objInvoice =>
                            !objInvoice.Entity.DeletedUtc.HasValue);
            }

            if (!blnIncludeArchived)
            {
                objQuery =
                    objQuery.Where(
                        objInvoice =>
                            !objInvoice.Entity.ArchivedUtc.HasValue);
            }

            if (enmStatus.HasValue)
            {
                objQuery =
                    objQuery.Where(
                        objInvoice =>
                            objInvoice.Status ==
                            enmStatus.Value);
            }

            if (objJobId.HasValue)
            {
                objQuery =
                    objQuery.Where(
                        objInvoice =>
                            objInvoice.JobId ==
                            objJobId.Value);
            }

            if (objCompanyId.HasValue)
            {
                objQuery =
                    objQuery.Where(
                        objInvoice =>
                            objInvoice.CompanyId ==
                            objCompanyId.Value);
            }

            if (!String.IsNullOrWhiteSpace(
                strSearch))
            {
                String strSearchValue =
                    strSearch.Trim();

                objQuery =
                    objQuery.Where(
                        objInvoice =>
                            objInvoice.InvoiceNumber.Contains(
                                strSearchValue) ||

                            (objInvoice.CustomerName != null &&
                             objInvoice.CustomerName.Contains(
                                 strSearchValue)) ||

                            objInvoice.Job.Name.Contains(
                                strSearchValue) ||

                            (objInvoice.Company != null &&
                             objInvoice.Company.Name.Contains(
                                 strSearchValue)) ||

                            (objInvoice.Postcode != null &&
                             objInvoice.Postcode.Contains(
                                 strSearchValue)));
            }

            if (blnOutstandingOnly ||
                blnOverdueOnly)
            {
                objQuery =
                    objQuery.Where(
                        objInvoice =>
                            objInvoice.Status !=
                            InvoiceStatus.Draft &&

                            objInvoice.Status !=
                            InvoiceStatus.Void &&

                            (
                                objInvoice.Lines
                                    .Sum(objLine =>
                                        (Decimal?)(
                                            objLine.Quantity *
                                            objLine.UnitPrice))
                                ?? 0m
                            )
                            >
                            (
                                objInvoice.Payments
                                    .Where(objPayment =>
                                        !objPayment.Entity.DeletedUtc.HasValue)
                                    .Sum(objPayment =>
                                        (Decimal?)objPayment.Amount)
                                ?? 0m
                            ));
            }

            if (blnOverdueOnly)
            {
                DateTime dteTodayUtc =
                    DateTime.UtcNow.Date;

                objQuery =
                    objQuery.Where(
                        objInvoice =>
                            objInvoice.DueDateUtc.HasValue &&
                            objInvoice.DueDateUtc.Value <
                            dteTodayUtc);
            }

            return await objQuery
                .OrderByDescending(objInvoice =>
                    objInvoice.IssueDateUtc ??
                    objInvoice.Entity.CreatedUtc)
                .ThenByDescending(objInvoice =>
                    objInvoice.Entity.CreatedUtc)
                .ToListAsync(
                    objToken);
        }

        ///<inheritdoc/>
        public async Task AddInvoiceAsync(
            Invoice objInvoice,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objContext =
                await _dbContextFactory.CreateDbContextAsync(
                    objToken);

            /*
             * Relationships are referenced by their foreign-key IDs.
             * Don't let detached Job / Company / Contact / Service
             * graphs get inserted or modified when the invoice is added.
             */
            objInvoice.Job =
                null!;

            objInvoice.Company =
                null;

            objInvoice.Contact =
                null;

            foreach (InvoiceLine objLine
                in objInvoice.Lines)
            {
                objLine.Invoice =
                    objInvoice;

                objLine.Service =
                    null;
            }

            objContext
                .Set<Invoice>()
                .Add(
                    objInvoice);

            await objContext.SaveChangesAsync(
                objToken);
        }

        ///<inheritdoc/>
        public async Task UpdateInvoiceAsync(
            Invoice objInvoice,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objContext =
                await _dbContextFactory.CreateDbContextAsync(
                    objToken);

            /*
             * Invoice lines are part of the Invoice aggregate.
             * Replace the existing lines with the supplied snapshot.
             */
            List<InvoiceLine> colExistingLines =
                await objContext
                    .Set<InvoiceLine>()
                    .Where(
                        objLine =>
                            objLine.InvoiceId ==
                            objInvoice.Id)
                    .ToListAsync(
                        objToken);

            if (colExistingLines.Count > 0)
            {
                objContext
                    .Set<InvoiceLine>()
                    .RemoveRange(
                        colExistingLines);
            }

            Invoice? objExistingInvoice =
                await objContext
                    .Set<Invoice>()
                    .Include(objExisting =>
                        objExisting.Entity)
                    .FirstOrDefaultAsync(
                        objExisting =>
                            objExisting.Id ==
                            objInvoice.Id,
                        objToken);

            if (objExistingInvoice == null)
            {
                return;
            }

            objExistingInvoice.JobId =
                objInvoice.JobId;

            objExistingInvoice.CompanyId =
                objInvoice.CompanyId;

            objExistingInvoice.ContactId =
                objInvoice.ContactId;

            objExistingInvoice.InvoiceNumber =
                objInvoice.InvoiceNumber;

            objExistingInvoice.Status =
                objInvoice.Status;

            objExistingInvoice.IssueDateUtc =
                objInvoice.IssueDateUtc;

            objExistingInvoice.DueDateUtc =
                objInvoice.DueDateUtc;

            objExistingInvoice.CustomerName =
                objInvoice.CustomerName;

            objExistingInvoice.AddressLine1 =
                objInvoice.AddressLine1;

            objExistingInvoice.AddressLine2 =
                objInvoice.AddressLine2;

            objExistingInvoice.Town =
                objInvoice.Town;

            objExistingInvoice.County =
                objInvoice.County;

            objExistingInvoice.Postcode =
                objInvoice.Postcode;

            objExistingInvoice.Notes =
                objInvoice.Notes;

            objExistingInvoice.Entity.DisplayName =
                objInvoice.Entity.DisplayName;

            objExistingInvoice.Entity.UpdatedUtc =
                objInvoice.Entity.UpdatedUtc;

            objExistingInvoice.Entity.UpdatedByUserId =
                objInvoice.Entity.UpdatedByUserId;

            objExistingInvoice.Entity.ArchivedUtc =
                objInvoice.Entity.ArchivedUtc;

            objExistingInvoice.Entity.ArchivedByUserId =
                objInvoice.Entity.ArchivedByUserId;

            objExistingInvoice.Entity.DeletedUtc =
                objInvoice.Entity.DeletedUtc;

            objExistingInvoice.Entity.DeletedByUserId =
                objInvoice.Entity.DeletedByUserId;

            foreach (InvoiceLine objLine
                in objInvoice.Lines)
            {
                objContext
                    .Set<InvoiceLine>()
                    .Add(
                        new InvoiceLine
                        {
                            Id =
                                objLine.Id == Guid.Empty
                                    ? Guid.NewGuid()
                                    : objLine.Id,

                            InvoiceId =
                                objInvoice.Id,

                            ServiceId =
                                objLine.ServiceId,

                            SortOrder =
                                objLine.SortOrder,

                            Description =
                                objLine.Description,

                            Quantity =
                                objLine.Quantity,

                            UnitPrice =
                                objLine.UnitPrice
                        });
            }

            await objContext.SaveChangesAsync(
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Boolean> InvoiceNumberExistsAsync(
            String strInvoiceNumber,
            Guid? objExcludeInvoiceId = null,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objContext =
                await _dbContextFactory.CreateDbContextAsync(
                    objToken);

            IQueryable<Invoice> objQuery =
                objContext
                    .Set<Invoice>()
                    .AsNoTracking()
                    .Where(
                        objInvoice =>
                            objInvoice.InvoiceNumber ==
                            strInvoiceNumber);

            if (objExcludeInvoiceId.HasValue)
            {
                objQuery =
                    objQuery.Where(
                        objInvoice =>
                            objInvoice.Id !=
                            objExcludeInvoiceId.Value);
            }

            return await objQuery.AnyAsync(
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Invoice?> IssueInvoiceAsync(
            Guid objInvoiceId,
            DateTime dteIssueDateUtc,
            DateTime dteDueDateUtc,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objContext =
                await _dbContextFactory.CreateDbContextAsync(
                    objToken);

            await using var objTransaction =
                await objContext.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    objToken);

            Invoice? objInvoice =
                await objContext
                    .Set<Invoice>()
                    .Include(objInvoice =>
                        objInvoice.Entity)
                    .Include(objInvoice =>
                        objInvoice.Lines)
                    .FirstOrDefaultAsync(
                        objInvoice =>
                            objInvoice.Id ==
                            objInvoiceId,
                        objToken);

            if (objInvoice == null ||
                objInvoice.Entity.DeletedUtc.HasValue)
            {
                await objTransaction.RollbackAsync(
                    objToken);

                return null;
            }

            if (objInvoice.Status !=
                InvoiceStatus.Draft)
            {
                await objTransaction.RollbackAsync(
                    objToken);

                return null;
            }

            /*
             * Locking the transaction at Serializable isolation means
             * invoice-number allocation and the update happen together.
             *
             * Only customer-facing invoice numbers beginning INV-
             * participate in this sequence. Draft references are ignored.
             */
            List<String> colInvoiceNumbers =
                await objContext
                    .Set<Invoice>()
                    .Where(objExisting =>
                        objExisting.InvoiceNumber.StartsWith(
                            "INV-"))
                    .Select(objExisting =>
                        objExisting.InvoiceNumber)
                    .ToListAsync(
                        objToken);

            Int32 intHighestNumber =
                0;

            foreach (String strExistingNumber
                in colInvoiceNumbers)
            {
                String strNumberPart =
                    strExistingNumber[
                        "INV-".Length..];

                if (Int32.TryParse(
                        strNumberPart,
                        out Int32 intNumber) &&
                    intNumber >
                    intHighestNumber)
                {
                    intHighestNumber =
                        intNumber;
                }
            }

            Int32 intNextNumber =
                intHighestNumber + 1;

            String strInvoiceNumber =
                $"INV-{intNextNumber:000000}";

            DateTime dteNow =
                DateTime.UtcNow;

            objInvoice.InvoiceNumber =
                strInvoiceNumber;

            objInvoice.Status =
                InvoiceStatus.Issued;

            objInvoice.IssueDateUtc =
                dteIssueDateUtc;

            objInvoice.DueDateUtc =
                dteDueDateUtc;

            objInvoice.Entity.DisplayName =
                strInvoiceNumber;

            objInvoice.Entity.UpdatedUtc =
                dteNow;

            objInvoice.Entity.UpdatedByUserId =
                objUserId;

            await objContext.SaveChangesAsync(
                objToken);

            await objTransaction.CommitAsync(
                objToken);

            return await objContext
                .Set<Invoice>()
                .AsNoTracking()
                .Include(objInvoice =>
                    objInvoice.Entity)
                .Include(objInvoice =>
                    objInvoice.Job)
                .Include(objInvoice =>
                    objInvoice.Company)
                .Include(objInvoice =>
                    objInvoice.Contact)
                    .ThenInclude(objContact =>
                        objContact!.Entity)
                .Include(objInvoice =>
                    objInvoice.Lines)
                    .ThenInclude(objLine =>
                        objLine.Service)
                .FirstOrDefaultAsync(
                    objInvoice =>
                        objInvoice.Id ==
                        objInvoiceId,
                    objToken);
        }

        ///<inheritdoc/>
        public async Task<Boolean> SetInvoiceStatusAsync(
            Guid objInvoiceId,
            InvoiceStatus enmStatus,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objContext =
                await _dbContextFactory.CreateDbContextAsync(
                    objToken);

            Invoice? objInvoice =
                await objContext
                    .Set<Invoice>()
                    .Include(objInvoice =>
                        objInvoice.Entity)
                    .FirstOrDefaultAsync(
                        objInvoice =>
                            objInvoice.Id ==
                            objInvoiceId,
                        objToken);

            if (objInvoice == null ||
                objInvoice.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            objInvoice.Status =
                enmStatus;

            objInvoice.Entity.UpdatedUtc =
                DateTime.UtcNow;

            objInvoice.Entity.UpdatedByUserId =
                objUserId;

            await objContext.SaveChangesAsync(
                objToken);

            return true;
        }
    }
}