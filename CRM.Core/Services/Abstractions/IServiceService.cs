using CRM.Core.Services.Domain;

namespace CRM.Core.Services.Abstractions
{
    public interface IServiceService
    {
        Task<Service?> GetServiceByIdAsync(
            Guid objServiceId,
            CancellationToken objToken = default);

        Task<List<Service>> GetServicesAsync(
            String? strSearch = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        Task<Service> AddServiceAsync(
            Service objService,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        Task<Service?> UpdateServiceAsync(
            Service objService,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        Task<Boolean> ArchiveServiceAsync(
            Guid objServiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        Task<Boolean> RestoreServiceAsync(
            Guid objServiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        Task<Boolean> DeleteServiceAsync(
            Guid objServiceId,
            Guid? objUserId = null,
            CancellationToken objToken = default);
    }
}