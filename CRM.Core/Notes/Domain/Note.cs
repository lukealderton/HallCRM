using CRM.Core.Entities.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Core.Notes.Domain
{
    public sealed class Note
    {
        public Guid Id { get; set; }

        public Guid EntityId { get; set; }

        public String Body { get; set; } = String.Empty;

        public DateTime CreatedUtc { get; set; }
        public Guid? CreatedByUserId { get; set; }

        public DateTime? UpdatedUtc { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        public DateTime? DeletedUtc { get; set; }
        public Guid? DeletedByUserId { get; set; }

        public CrmEntity? Entity { get; set; }

        [NotMapped]
        public String? AuthorDisplayName { get; set; }
    }
}