using CRM.Core.Services.Domain;

namespace CRM.Core.Services.Abstractions
{
    public interface IServiceRepository
    {
        Task<Service?> GetServiceByIdAsync(
            Guid objServiceId,
            Boolean blnTracking = false,
            CancellationToken objToken = default);

        Task<List<Service>> GetServicesAsync(
            String? strSearch = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        Task AddServiceAsync(
            Service objService,
            CancellationToken objToken = default);

        Task UpdateServiceAsync(
            Service objService,
            CancellationToken objToken = default);
    }
}