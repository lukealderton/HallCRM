using CRM.Primitives.Common.Abstraction;

namespace CRM.Core.Entities.Domain
{
    public abstract class CrmEntityRecord : IDbItem
    {
        public Guid Id { get; set; }

        public CrmEntity Entity { get; set; } = null!;

        public Boolean IsArchived =>
            Entity?.ArchivedUtc.HasValue == true;

        public Boolean IsDeleted =>
            Entity?.DeletedUtc.HasValue == true;

        public Boolean IsActive =>
            !IsArchived && !IsDeleted;
    }
}
