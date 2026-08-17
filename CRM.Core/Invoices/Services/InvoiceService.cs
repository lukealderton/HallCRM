using CRM.Core.Entities.Domain;
using CRM.Core.Invoices.Abstractions;
using CRM.Core.Invoices.Domain;
using CRM.Core.Jobs.Abstractions;
using CRM.Core.Jobs.Domain;

namespace CRM.Core.Invoices.Services
{
    public sealed class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IJobService _jobService;

        public InvoiceService(
            IInvoiceRepository objInvoiceRepository,
            IJobService objJobService)
        {
            _invoiceRepository =
                objInvoiceRepository;

            _jobService =
                objJobService;
        }

        ///<inheritdoc/>
        public Task<Invoice?> GetInvoiceByIdAsync(
            Guid objInvoiceId,
            CancellationToken objToken = default)
        {
            if (objInvoiceId == Guid.Empty)
            {
                return Task.FromResult<Invoice?>(
                    null);
            }

            return _invoiceRepository.GetInvoiceByIdAsync(
                objInvoiceId,
                false,
                objToken);
        }

        ///<inheritdoc/>
        public Task<List<Invoice>> GetInvoicesAsync(
            String? strSearch = null,
            InvoiceStatus? enmStatus = null,
            Guid? objJobId = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            return _invoiceRepository.GetInvoicesAsync(
                strSearch,
                enmStatus,
                objJobId,
                objCompanyId,
                blnIncludeArchived,
                blnIncludeDeleted,
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Invoice> CreateFromJobAsync(
            Guid objJobId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            if (objJobId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Job id is required.",
                    nameof(objJobId));
            }

            Job? objJob =
                await _jobService.GetJobByIdAsync(
                    objJobId,
                    objToken);

            if (objJob == null ||
                objJob.Entity.DeletedUtc.HasValue)
            {
                throw new InvalidOperationException(
                    "The selected job could not be found.");
            }

            if (objJob.ServiceLinks.Count == 0)
            {
                throw new InvalidOperationException(
                    "The job does not contain any service lines to invoice.");
            }

            List<JobServiceLink> colUnpricedLines =
                objJob.ServiceLinks
                    .Where(
                        objLink =>
                            !objLink.UnitPrice.HasValue)
                    .ToList();

            if (colUnpricedLines.Count > 0)
            {
                String strServices =
                    String.Join(
                        ", ",
                        colUnpricedLines
                            .Select(
                                objLink =>
                                    objLink.Service.Name)
                            .Distinct()
                            .OrderBy(
                                strName =>
                                    strName));

                throw new InvalidOperationException(
                    "All job service lines must have a price before " +
                    "an invoice can be created. Unpriced services: " +
                    strServices +
                    ".");
            }

            Guid objInvoiceId =
                Guid.NewGuid();

            DateTime dteNow =
                DateTime.UtcNow;

            String strInvoiceNumber =
                await GenerateDraftInvoiceNumberAsync(
                    objInvoiceId,
                    objToken);

            String strCustomerName =
                GetCustomerName(
                    objJob);

            Invoice objInvoice =
                new()
                {
                    Id =
                        objInvoiceId,

                    JobId =
                        objJob.Id,

                    CompanyId =
                        objJob.CompanyId,

                    ContactId =
                        objJob.ContactId,

                    InvoiceNumber =
                        strInvoiceNumber,

                    Status =
                        InvoiceStatus.Draft,

                    /*
                     * A Draft has not technically been issued yet.
                     * We'll populate IssueDateUtc when the user
                     * performs the Issue action.
                     */
                    IssueDateUtc =
                        null,

                    DueDateUtc =
                        null,

                    CustomerName =
                        CleanString(
                            strCustomerName),

                    /*
                     * For now use the Job/site snapshot as the
                     * invoice address.
                     *
                     * We can separate Billing Address from Site
                     * Address later if required.
                     */
                    AddressLine1 =
                        CleanString(
                            objJob.AddressLine1),

                    AddressLine2 =
                        CleanString(
                            objJob.AddressLine2),

                    Town =
                        CleanString(
                            objJob.Town),

                    County =
                        CleanString(
                            objJob.County),

                    Postcode =
                        CleanPostcode(
                            objJob.Postcode),

                    Lines =
                        CreateLinesFromJob(
                            objInvoiceId,
                            objJob),

                    Entity =
                        new CrmEntity
                        {
                            Id =
                                objInvoiceId,

                            EntityTypeId =
                                (Int32)PredefinedEntityType.Invoice,

                            DisplayName =
                                strInvoiceNumber,

                            OwnerUserId =
                                objUserId,

                            CreatedUtc =
                                dteNow,

                            CreatedByUserId =
                                objUserId
                        }
                };

            await _invoiceRepository.AddInvoiceAsync(
                objInvoice,
                objToken);

            Invoice? objSavedInvoice =
                await _invoiceRepository.GetInvoiceByIdAsync(
                    objInvoiceId,
                    false,
                    objToken);

            return objSavedInvoice ??
                objInvoice;
        }

        ///<inheritdoc/>
        public async Task<Invoice> AddInvoiceAsync(
            Invoice objInvoice,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            if (objInvoice == null)
            {
                throw new ArgumentNullException(
                    nameof(objInvoice));
            }

            if (objInvoice.JobId == Guid.Empty)
            {
                throw new ArgumentException(
                    "A job is required.",
                    nameof(objInvoice));
            }

            Guid objInvoiceId =
                objInvoice.Id == Guid.Empty
                    ? Guid.NewGuid()
                    : objInvoice.Id;

            DateTime dteNow =
                DateTime.UtcNow;

            objInvoice.Id =
                objInvoiceId;

            if (String.IsNullOrWhiteSpace(
                objInvoice.InvoiceNumber))
            {
                objInvoice.InvoiceNumber =
                    await GenerateDraftInvoiceNumberAsync(
                        objInvoiceId,
                        objToken);
            }
            else
            {
                objInvoice.InvoiceNumber =
                    objInvoice.InvoiceNumber
                        .Trim()
                        .ToUpperInvariant();

                Boolean blnNumberExists =
                    await _invoiceRepository
                        .InvoiceNumberExistsAsync(
                            objInvoice.InvoiceNumber,
                            null,
                            objToken);

                if (blnNumberExists)
                {
                    throw new InvalidOperationException(
                        $"Invoice number '{objInvoice.InvoiceNumber}' " +
                        "already exists.");
                }
            }

            CleanInvoice(
                objInvoice);

            objInvoice.Lines =
                CleanLines(
                    objInvoiceId,
                    objInvoice.Lines);

            objInvoice.Entity =
                new CrmEntity
                {
                    Id =
                        objInvoiceId,

                    EntityTypeId =
                        (Int32)PredefinedEntityType.Invoice,

                    DisplayName =
                        objInvoice.InvoiceNumber,

                    OwnerUserId =
                        objUserId,

                    CreatedUtc =
                        dteNow,

                    CreatedByUserId =
                        objUserId
                };

            await _invoiceRepository.AddInvoiceAsync(
                objInvoice,
                objToken);

            Invoice? objSavedInvoice =
                await _invoiceRepository.GetInvoiceByIdAsync(
                    objInvoiceId,
                    false,
                    objToken);

            return objSavedInvoice ??
                objInvoice;
        }

        ///<inheritdoc/>
        public async Task<Invoice?> UpdateInvoiceAsync(
            Invoice objInvoice,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            if (objInvoice == null)
            {
                throw new ArgumentNullException(
                    nameof(objInvoice));
            }

            if (objInvoice.Id == Guid.Empty)
            {
                throw new ArgumentException(
                    "Invoice id is required.",
                    nameof(objInvoice));
            }

            Invoice? objExistingInvoice =
                await _invoiceRepository.GetInvoiceByIdAsync(
                    objInvoice.Id,
                    true,
                    objToken);

            if (objExistingInvoice == null ||
                objExistingInvoice.Entity.DeletedUtc.HasValue)
            {
                return null;
            }

            if (objExistingInvoice.Status != InvoiceStatus.Draft)
            {
                throw new InvalidOperationException(
                    "Only draft invoices can be edited.");
            }

            String strInvoiceNumber =
                objInvoice.InvoiceNumber
                    ?.Trim()
                    .ToUpperInvariant() ??
                String.Empty;

            if (String.IsNullOrWhiteSpace(
                strInvoiceNumber))
            {
                throw new ArgumentException(
                    "Invoice number is required.",
                    nameof(objInvoice));
            }

            Boolean blnNumberExists =
                await _invoiceRepository.InvoiceNumberExistsAsync(
                    strInvoiceNumber,
                    objInvoice.Id,
                    objToken);

            if (blnNumberExists)
            {
                throw new InvalidOperationException(
                    $"Invoice number '{strInvoiceNumber}' already exists.");
            }

            objExistingInvoice.JobId =
                objInvoice.JobId;

            objExistingInvoice.CompanyId =
                objInvoice.CompanyId;

            objExistingInvoice.ContactId =
                objInvoice.ContactId;

            objExistingInvoice.InvoiceNumber =
                strInvoiceNumber;

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

            CleanInvoice(
                objExistingInvoice);

            objExistingInvoice.Lines =
                CleanLines(
                    objExistingInvoice.Id,
                    objInvoice.Lines);

            DateTime dteNow =
                DateTime.UtcNow;

            objExistingInvoice.Entity.DisplayName =
                strInvoiceNumber;

            objExistingInvoice.Entity.UpdatedUtc =
                dteNow;

            objExistingInvoice.Entity.UpdatedByUserId =
                objUserId;

            await _invoiceRepository.UpdateInvoiceAsync(
                objExistingInvoice,
                objToken);

            return await _invoiceRepository.GetInvoiceByIdAsync(
                objExistingInvoice.Id,
                false,
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Boolean> ArchiveInvoiceAsync(
            Guid objInvoiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Invoice? objInvoice =
                await GetEditableInvoiceAsync(
                    objInvoiceId,
                    objToken);

            if (objInvoice == null)
            {
                return false;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objInvoice.Entity.ArchivedUtc =
                dteNow;

            objInvoice.Entity.ArchivedByUserId =
                objUserId;

            objInvoice.Entity.UpdatedUtc =
                dteNow;

            objInvoice.Entity.UpdatedByUserId =
                objUserId;

            await _invoiceRepository.UpdateInvoiceAsync(
                objInvoice,
                objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> RestoreInvoiceAsync(
            Guid objInvoiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Invoice? objInvoice =
                await GetEditableInvoiceAsync(
                    objInvoiceId,
                    objToken);

            if (objInvoice == null)
            {
                return false;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objInvoice.Entity.ArchivedUtc =
                null;

            objInvoice.Entity.ArchivedByUserId =
                null;

            objInvoice.Entity.UpdatedUtc =
                dteNow;

            objInvoice.Entity.UpdatedByUserId =
                objUserId;

            await _invoiceRepository.UpdateInvoiceAsync(
                objInvoice,
                objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> DeleteInvoiceAsync(
            Guid objInvoiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Invoice? objInvoice =
                await GetEditableInvoiceAsync(
                    objInvoiceId,
                    objToken);

            if (objInvoice == null)
            {
                return false;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objInvoice.Entity.DeletedUtc =
                dteNow;

            objInvoice.Entity.DeletedByUserId =
                objUserId;

            objInvoice.Entity.UpdatedUtc =
                dteNow;

            objInvoice.Entity.UpdatedByUserId =
                objUserId;

            await _invoiceRepository.UpdateInvoiceAsync(
                objInvoice,
                objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Invoice?> IssueInvoiceAsync(
            Guid objInvoiceId,
            DateTime? dteIssueDateUtc = null,
            DateTime? dteDueDateUtc = null,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            if (objInvoiceId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Invoice id is required.",
                    nameof(objInvoiceId));
            }

            Invoice? objInvoice =
                await _invoiceRepository.GetInvoiceByIdAsync(
                    objInvoiceId,
                    false,
                    objToken);

            if (objInvoice == null ||
                objInvoice.Entity.DeletedUtc.HasValue)
            {
                return null;
            }

            if (objInvoice.Status !=
                InvoiceStatus.Draft)
            {
                throw new InvalidOperationException(
                    "Only draft invoices can be issued.");
            }

            if (objInvoice.Lines.Count == 0)
            {
                throw new InvalidOperationException(
                    "The invoice must contain at least one line before it can be issued.");
            }

            if (objInvoice.Lines.Any(
                objLine =>
                    String.IsNullOrWhiteSpace(
                        objLine.Description)))
            {
                throw new InvalidOperationException(
                    "Every invoice line must have a description.");
            }

            if (objInvoice.Lines.Any(
                objLine =>
                    objLine.Quantity <= 0m))
            {
                throw new InvalidOperationException(
                    "Every invoice line must have a quantity greater than zero.");
            }

            if (objInvoice.Lines.Any(
                objLine =>
                    objLine.UnitPrice < 0m))
            {
                throw new InvalidOperationException(
                    "Invoice line prices cannot be negative.");
            }

            DateTime dteIssue =
                dteIssueDateUtc?.Date ??
                DateTime.UtcNow.Date;

            DateTime dteDue =
                dteDueDateUtc?.Date ??
                dteIssue.AddDays(
                    30);

            if (dteDue <
                dteIssue)
            {
                throw new InvalidOperationException(
                    "The due date cannot be before the issue date.");
            }

            return await _invoiceRepository.IssueInvoiceAsync(
                objInvoiceId,
                dteIssue,
                dteDue,
                objUserId,
                objToken);
        }

        private async Task<Invoice?> GetEditableInvoiceAsync(
            Guid objInvoiceId,
            CancellationToken objToken)
        {
            if (objInvoiceId == Guid.Empty)
            {
                return null;
            }

            Invoice? objInvoice =
                await _invoiceRepository.GetInvoiceByIdAsync(
                    objInvoiceId,
                    true,
                    objToken);

            if (objInvoice == null ||
                objInvoice.Entity.DeletedUtc.HasValue)
            {
                return null;
            }

            return objInvoice;
        }

        private static List<InvoiceLine> CreateLinesFromJob(
            Guid objInvoiceId,
            Job objJob)
        {
            Int32 intSortOrder =
                0;

            return objJob.ServiceLinks
                .OrderBy(
                    objLink =>
                        objLink.Service.Name)
                .Select(
                    objLink =>
                    {
                        intSortOrder++;

                        Decimal dcmQuantity =
                            CleanQuantity(
                                objLink.Quantity);

                        /*
                         * CreateFromJobAsync already validates this,
                         * so null here indicates a programming error.
                         */
                        Decimal dcmUnitPrice =
                            objLink.UnitPrice
                            ?? throw new InvalidOperationException(
                                $"Service '{objLink.Service.Name}' " +
                                "does not have a job price.");

                        return new InvoiceLine
                        {
                            Id =
                                Guid.NewGuid(),

                            InvoiceId =
                                objInvoiceId,

                            ServiceId =
                                objLink.ServiceId,

                            SortOrder =
                                intSortOrder,

                            /*
                             * Snapshot the wording too. The invoice
                             * must not change if the Service is renamed.
                             */
                            Description =
                                objLink.Service.Name.Trim(),

                            Quantity =
                                dcmQuantity,

                            UnitPrice =
                                CleanUnitPrice(
                                    dcmUnitPrice)
                        };
                    })
                .ToList();
        }

        private static List<InvoiceLine> CleanLines(
            Guid objInvoiceId,
            IEnumerable<InvoiceLine>? colLines)
        {
            if (colLines == null)
            {
                return [];
            }

            Int32 intSortOrder =
                0;

            List<InvoiceLine> colResult =
                [];

            foreach (InvoiceLine objLine
                in colLines.OrderBy(
                    objLine =>
                        objLine.SortOrder))
            {
                if (String.IsNullOrWhiteSpace(
                    objLine.Description))
                {
                    continue;
                }

                intSortOrder++;

                colResult.Add(
                    new InvoiceLine
                    {
                        Id =
                            objLine.Id == Guid.Empty
                                ? Guid.NewGuid()
                                : objLine.Id,

                        InvoiceId =
                            objInvoiceId,

                        ServiceId =
                            objLine.ServiceId,

                        SortOrder =
                            intSortOrder,

                        Description =
                            objLine.Description.Trim(),

                        Quantity =
                            CleanQuantity(
                                objLine.Quantity),

                        UnitPrice =
                            CleanUnitPrice(
                                objLine.UnitPrice)
                    });
            }

            return colResult;
        }

        private static void CleanInvoice(
            Invoice objInvoice)
        {
            objInvoice.CustomerName =
                CleanString(
                    objInvoice.CustomerName);

            objInvoice.AddressLine1 =
                CleanString(
                    objInvoice.AddressLine1);

            objInvoice.AddressLine2 =
                CleanString(
                    objInvoice.AddressLine2);

            objInvoice.Town =
                CleanString(
                    objInvoice.Town);

            objInvoice.County =
                CleanString(
                    objInvoice.County);

            objInvoice.Postcode =
                CleanPostcode(
                    objInvoice.Postcode);

            objInvoice.Notes =
                CleanString(
                    objInvoice.Notes);

            if (objInvoice.DueDateUtc.HasValue &&
                objInvoice.IssueDateUtc.HasValue &&
                objInvoice.DueDateUtc.Value.Date <
                objInvoice.IssueDateUtc.Value.Date)
            {
                throw new ArgumentException(
                    "Invoice due date cannot be before the issue date.",
                    nameof(objInvoice));
            }
        }

        private async Task<String> GenerateDraftInvoiceNumberAsync(
            Guid objInvoiceId,
            CancellationToken objToken)
        {
            /*
             * This is deliberately a DRAFT reference rather than
             * pretending we already have our final accounting
             * sequence.
             *
             * When we add IssueInvoiceAsync we'll allocate the
             * final invoice number there.
             */
            String strBaseNumber =
                $"DRAFT-{objInvoiceId:N}"
                    .ToUpperInvariant();

            String strInvoiceNumber =
                strBaseNumber[..Math.Min(
                    20,
                    strBaseNumber.Length)];

            if (!await _invoiceRepository.InvoiceNumberExistsAsync(
                strInvoiceNumber,
                null,
                objToken))
            {
                return strInvoiceNumber;
            }

            /*
             * This should be practically impossible, but keep the
             * unique DB index protected anyway.
             */
            do
            {
                strInvoiceNumber =
                    $"DRAFT-{Guid.NewGuid():N}"
                        .ToUpperInvariant();

                strInvoiceNumber =
                    strInvoiceNumber[..Math.Min(
                        20,
                        strInvoiceNumber.Length)];
            }
            while (await _invoiceRepository.InvoiceNumberExistsAsync(
                strInvoiceNumber,
                null,
                objToken));

            return strInvoiceNumber;
        }

        private static String GetCustomerName(
            Job objJob)
        {
            if (objJob.Company != null &&
                !String.IsNullOrWhiteSpace(
                    objJob.Company.Name))
            {
                return objJob.Company.Name;
            }

            if (objJob.Contact != null &&
                !String.IsNullOrWhiteSpace(
                    objJob.Contact.Entity.DisplayName))
            {
                return objJob.Contact.Entity.DisplayName;
            }

            return objJob.Name;
        }

        private static Decimal CleanQuantity(
            Decimal dcmQuantity)
        {
            if (dcmQuantity <= 0m)
            {
                return 1m;
            }

            return Decimal.Round(
                dcmQuantity,
                2,
                MidpointRounding.AwayFromZero);
        }

        private static Decimal CleanUnitPrice(
            Decimal dcmUnitPrice)
        {
            if (dcmUnitPrice < 0m)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(dcmUnitPrice),
                    "Unit price cannot be negative.");
            }

            return Decimal.Round(
                dcmUnitPrice,
                2,
                MidpointRounding.AwayFromZero);
        }

        private static String? CleanString(
            String? strValue)
        {
            if (String.IsNullOrWhiteSpace(
                strValue))
            {
                return null;
            }

            return strValue.Trim();
        }

        private static String? CleanPostcode(
            String? strPostcode)
        {
            return CleanString(
                strPostcode)
                ?.ToUpperInvariant();
        }
    }
}