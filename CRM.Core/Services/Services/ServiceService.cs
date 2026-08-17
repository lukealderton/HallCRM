using CRM.Core.Entities.Domain;
using CRM.Core.Services.Abstractions;
using CRM.Core.Services.Domain;

namespace CRM.Core.Services.Services
{
    public sealed class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(
            IServiceRepository objServiceRepository)
        {
            _serviceRepository =
                objServiceRepository;
        }

        public Task<Service?> GetServiceByIdAsync(
            Guid objServiceId,
            CancellationToken objToken = default)
        {
            return _serviceRepository.GetServiceByIdAsync(
                objServiceId,
                false,
                objToken);
        }

        public Task<List<Service>> GetServicesAsync(
            String? strSearch = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            return _serviceRepository.GetServicesAsync(
                strSearch,
                blnIncludeArchived,
                blnIncludeDeleted,
                objToken);
        }

        public async Task<Service> AddServiceAsync(
            Service objService,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            if (String.IsNullOrWhiteSpace(
                objService.Name))
            {
                throw new ArgumentException(
                    "Service name is required.",
                    nameof(objService));
            }

            Guid objServiceId =
                objService.Id == Guid.Empty
                    ? Guid.NewGuid()
                    : objService.Id;

            DateTime dteNow =
                DateTime.UtcNow;

            String strServiceName =
                objService.Name.Trim();

            objService.Id =
                objServiceId;

            objService.Name =
                strServiceName;

            objService.Description =
                CleanString(
                    objService.Description);

            objService.Notes =
                CleanString(
                    objService.Notes);

            objService.Entity =
                new CrmEntity
                {
                    Id = objServiceId,

                    EntityTypeId =
                        (Int32)PredefinedEntityType.Service,

                    DisplayName =
                        strServiceName,

                    OwnerUserId =
                        objUserId,

                    CreatedUtc =
                        dteNow,

                    CreatedByUserId =
                        objUserId
                };

            await _serviceRepository.AddServiceAsync(
                objService,
                objToken);

            return objService;
        }

        public async Task<Service?> UpdateServiceAsync(
            Service objService,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            if (objService.Id == Guid.Empty)
            {
                throw new ArgumentException(
                    "Service id is required.",
                    nameof(objService));
            }

            if (String.IsNullOrWhiteSpace(
                objService.Name))
            {
                throw new ArgumentException(
                    "Service name is required.",
                    nameof(objService));
            }

            Service? objExistingService =
                await _serviceRepository
                    .GetServiceByIdAsync(
                        objService.Id,
                        true,
                        objToken);

            if (objExistingService == null ||
                objExistingService.Entity
                    .DeletedUtc.HasValue)
            {
                return null;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            String strServiceName =
                objService.Name.Trim();

            objExistingService.Name =
                strServiceName;

            objExistingService.Description =
                CleanString(
                    objService.Description);

            objExistingService.DefaultPrice =
                objService.DefaultPrice;

            objExistingService.Notes =
                CleanString(
                    objService.Notes);

            objExistingService.Entity.DisplayName =
                strServiceName;

            objExistingService.Entity.UpdatedUtc =
                dteNow;

            objExistingService.Entity.UpdatedByUserId =
                objUserId;

            await _serviceRepository.UpdateServiceAsync(
                objExistingService,
                objToken);

            return objExistingService;
        }

        public async Task<Boolean> ArchiveServiceAsync(
            Guid objServiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Service? objService =
                await _serviceRepository
                    .GetServiceByIdAsync(
                        objServiceId,
                        true,
                        objToken);

            if (objService == null ||
                objService.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objService.Entity.ArchivedUtc =
                dteNow;

            objService.Entity.ArchivedByUserId =
                objUserId;

            objService.Entity.UpdatedUtc =
                dteNow;

            objService.Entity.UpdatedByUserId =
                objUserId;

            await _serviceRepository.UpdateServiceAsync(
                objService,
                objToken);

            return true;
        }

        public async Task<Boolean> RestoreServiceAsync(
            Guid objServiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Service? objService =
                await _serviceRepository
                    .GetServiceByIdAsync(
                        objServiceId,
                        true,
                        objToken);

            if (objService == null ||
                objService.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objService.Entity.ArchivedUtc =
                null;

            objService.Entity.ArchivedByUserId =
                null;

            objService.Entity.UpdatedUtc =
                dteNow;

            objService.Entity.UpdatedByUserId =
                objUserId;

            await _serviceRepository.UpdateServiceAsync(
                objService,
                objToken);

            return true;
        }

        public async Task<Boolean> DeleteServiceAsync(
            Guid objServiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Service? objService =
                await _serviceRepository
                    .GetServiceByIdAsync(
                        objServiceId,
                        true,
                        objToken);

            if (objService == null ||
                objService.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objService.Entity.DeletedUtc =
                dteNow;

            objService.Entity.DeletedByUserId =
                objUserId;

            objService.Entity.UpdatedUtc =
                dteNow;

            objService.Entity.UpdatedByUserId =
                objUserId;

            await _serviceRepository.UpdateServiceAsync(
                objService,
                objToken);

            return true;
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
    }
}