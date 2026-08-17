using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Core.Jobs.Domain
{
    public sealed class JobServiceLink
    {
        public Guid JobId { get; set; }

        public Job Job { get; set; } = null!;

        public Guid ServiceId { get; set; }

        public CRM.Core.Services.Domain.Service Service { get; set; } = null!;

        public Decimal Quantity { get; set; } = 1m;

        public Decimal? UnitPrice { get; set; }

        [NotMapped]
        public Decimal LineTotal =>
            Quantity *
            (UnitPrice ?? 0m);
    }
}