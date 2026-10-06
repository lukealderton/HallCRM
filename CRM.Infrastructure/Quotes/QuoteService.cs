using CRM.Core.Jobs.Abstractions;
using CRM.Core.Quotes;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Quotes;

public sealed class QuoteService(IDbContextFactory<CRMDbContext> factory, IJobService jobs) : IQuoteService
{
    public async Task<Quote> CreateFromJobAsync(Guid jobId, Guid? userId = null, CancellationToken token = default)
    {
        var job = await jobs.GetJobByIdAsync(jobId, token)
            ?? throw new InvalidOperationException("The selected job could not be found.");
        var quote = Quote.FromJob(job, userId);
        await using var db = await factory.CreateDbContextAsync(token);
        db.Quotes.Add(quote);
        await db.SaveChangesAsync(token);
        return quote;
    }

    public async Task<Quote?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        await using var db = await factory.CreateDbContextAsync(token);
        return await db.Quotes.AsNoTracking().Include(quote => quote.Lines).FirstOrDefaultAsync(quote => quote.Id == id, token);
    }

    public async Task<List<Quote>> GetForJobAsync(Guid jobId, CancellationToken token = default)
    {
        await using var db = await factory.CreateDbContextAsync(token);
        return await db.Quotes.AsNoTracking().Include(quote => quote.Lines)
            .Where(quote => quote.JobId == jobId).OrderByDescending(quote => quote.CreatedUtc).ToListAsync(token);
    }
}
