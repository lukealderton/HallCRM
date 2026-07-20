using CRM.Primitives.Common.Abstraction;

namespace CRM.Core.Entities.Domain
{
    public sealed class CrmEntity : IDbItem
    {
        public Guid Id { get; set; }

        public Int32 EntityTypeId { get; set; }

        public String DisplayName { get; set; } = String.Empty;

        public Guid? OwnerUserId { get; set; }

        public DateTime CreatedUtc { get; set; }
        public Guid? CreatedByUserId { get; set; }

        public DateTime? UpdatedUtc { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        public DateTime? ArchivedUtc { get; set; }
        public Guid? ArchivedByUserId { get; set; }

        public DateTime? DeletedUtc { get; set; }
        public Guid? DeletedByUserId { get; set; }

        public Byte[] RowVersion { get; set; } = [];

        public CrmEntityType? EntityType { get; set; }
    }
}