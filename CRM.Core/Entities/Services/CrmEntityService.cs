using CRM.Core.Entities.Abstractions;
using CRM.Core.Entities.Domain;

namespace CRM.Core.Entities.Services
{
    public sealed class CrmEntityService : ICrmEntityService
    {
        private readonly ICrmEntityRepository _entityRepository;

        public CrmEntityService(ICrmEntityRepository objEntityRepository)
        {
            _entityRepository = objEntityRepository;
        }

        ///<inheritdoc/>
        public Task<CrmEntity?> GetEntityByIdAsync(Guid objEntityId, CancellationToken objToken = default)
        {
            return _entityRepository.GetEntityByIdAsync(objEntityId, false, objToken);
        }

        ///<inheritdoc/>
        public Task<List<CrmEntity>> GetEntitiesAsync(
            Int32? intEntityTypeId = null,
            String? strSearch = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            return _entityRepository.GetEntitiesAsync(
                intEntityTypeId,
                strSearch,
                blnIncludeArchived,
                blnIncludeDeleted,
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Boolean> ArchiveEntityAsync(Guid objEntityId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            CrmEntity? objEntity = await _entityRepository.GetEntityByIdAsync(objEntityId, true, objToken);

            if (objEntity == null || objEntity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objEntity.ArchivedUtc = dteNow;
            objEntity.ArchivedByUserId = objUserId;
            objEntity.UpdatedUtc = dteNow;
            objEntity.UpdatedByUserId = objUserId;

            await _entityRepository.SaveChangesAsync(objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> RestoreEntityAsync(Guid objEntityId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            CrmEntity? objEntity = await _entityRepository.GetEntityByIdAsync(objEntityId, true, objToken);

            if (objEntity == null || objEntity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objEntity.ArchivedUtc = null;
            objEntity.ArchivedByUserId = null;
            objEntity.UpdatedUtc = dteNow;
            objEntity.UpdatedByUserId = objUserId;

            await _entityRepository.SaveChangesAsync(objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> DeleteEntityAsync(Guid objEntityId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            CrmEntity? objEntity = await _entityRepository.GetEntityByIdAsync(objEntityId, true, objToken);

            if (objEntity == null || objEntity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objEntity.DeletedUtc = dteNow;
            objEntity.DeletedByUserId = objUserId;
            objEntity.UpdatedUtc = dteNow;
            objEntity.UpdatedByUserId = objUserId;

            await _entityRepository.SaveChangesAsync(objToken);

            return true;
        }
    }
}