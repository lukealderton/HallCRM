namespace CRM.Core.Jobs.Abstractions
{
    public interface IJobSheetService
    {
        Task<Byte[]> GenerateJobSheetAsync(
            Guid objJobId,
            CancellationToken objToken = default);
    }
}