using System.ComponentModel.DataAnnotations.Schema;
using CRM.Core.Jobs.Domain;

namespace CRM.Core.Quotes;

public sealed class Quote
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public string JobName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Town { get; set; }
    public string? County { get; set; }
    public string? Postcode { get; set; }
    public string? Notes { get; set; }
    public string? JobDescription { get; set; }
    public List<QuoteLine> Lines { get; set; } = [];
    [NotMapped] public decimal Subtotal => Lines.Sum(line => line.LineTotal);
    [NotMapped] public decimal Total => Subtotal;

    public static Quote FromJob(Job job, Guid? userId = null)
    {
        if (job.Entity.DeletedUtc.HasValue)
            throw new InvalidOperationException("The selected job could not be found.");
        if (job.ServiceLinks.Count == 0)
            throw new InvalidOperationException("Add at least one service before creating a quote.");
        if (job.ServiceLinks.Any(line => !line.UnitPrice.HasValue || line.UnitPrice < 0 || line.Quantity <= 0))
            throw new InvalidOperationException("All services need a non-negative price and a positive quantity before creating a quote.");

        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        return new Quote
        {
            Id = id, JobId = job.Id, JobName = job.Name,
            QuoteNumber = $"QUO-{now:yyyyMMdd}-{id:N}".ToUpperInvariant(),
            CreatedUtc = now, CreatedByUserId = userId,
            CustomerName = job.Company?.Name ?? job.Contact?.Entity.DisplayName ?? job.Name,
            AddressLine1 = job.AddressLine1, AddressLine2 = job.AddressLine2,
            Town = job.Town, County = job.County, Postcode = job.Postcode,
            Notes = job.Notes, JobDescription = job.Description,
            Lines = job.ServiceLinks.OrderBy(line => line.Service.Name).Select((line, index) => new QuoteLine
            {
                Id = Guid.NewGuid(), Description = line.Service.Name,
                ServiceDescription = line.Service.Description,
                Quantity = decimal.Round(line.Quantity, 2, MidpointRounding.AwayFromZero),
                UnitPrice = decimal.Round(line.UnitPrice!.Value, 2, MidpointRounding.AwayFromZero), SortOrder = index
            }).ToList()
        };
    }
}

public sealed class QuoteLine
{
    public string? ServiceDescription { get; set; }
    public Guid Id { get; set; }
    public int SortOrder { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    [NotMapped] public decimal LineTotal => Quantity * UnitPrice;
}

public interface IQuoteService
{
    Task<Quote> CreateFromJobAsync(Guid jobId, Guid? userId = null, CancellationToken token = default);
    Task<Quote?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<List<Quote>> GetForJobAsync(Guid jobId, CancellationToken token = default);
    Task<List<Quote>> GetRecentForCompanyAsync(Guid companyId, CancellationToken token = default);
}

public interface IQuoteDocumentService
{
    Task<byte[]> GenerateQuoteAsync(Guid id, CancellationToken token = default);
}
