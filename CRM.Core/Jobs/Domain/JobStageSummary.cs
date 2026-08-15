namespace CRM.Core.Jobs.Domain
{
    public sealed class JobStageSummary
    {
        public JobStage Stage { get; set; }

        public Int32 Count { get; set; }

        public Decimal Value { get; set; }
    }
}