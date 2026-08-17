using CRM.Core.Entities.Domain;
using CRM.Core.Jobs.Domain;

namespace CRM.Core.Services.Domain
{
    public sealed class Service : CrmEntityRecord
    {
        public String Name { get; set; } =
            String.Empty;

        public String? Description { get; set; }

        public Decimal? DefaultPrice { get; set; }

        public String? Notes { get; set; }

        public ICollection<JobServiceLink> JobLinks { get; set; } = [];
    }
}