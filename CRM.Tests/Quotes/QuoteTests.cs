using CRM.Core.Entities.Domain;
using CRM.Core.Jobs.Domain;
using CRM.Core.Quotes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRM.Tests.Quotes;

[TestClass]
public sealed class QuoteTests
{
    private static Job CreateJob() => new()
    {
        Id = Guid.NewGuid(), Name = "Kitchen repair", AddressLine1 = "1 High Street",
        Notes = "Original scope", Entity = new CrmEntity(),
        ServiceLinks = [new JobServiceLink
        {
            Service = new CRM.Core.Services.Domain.Service { Name = "Painting" },
            Quantity = 2m, UnitPrice = 75m
        }]
    };

    [TestMethod]
    public void QuoteRetainsOriginalServicesAndCustomerDetails()
    {
        var job = CreateJob();
        var quote = Quote.FromJob(job);
        job.Name = "Changed job";
        job.AddressLine1 = "New address";
        job.Notes = "New scope";
        var service = job.ServiceLinks.Single();
        service.Service.Name = "Renamed service";
        service.Quantity = 8m;
        service.UnitPrice = 100m;
        job.ServiceLinks.Clear();

        Assert.AreEqual("Kitchen repair", quote.JobName);
        Assert.AreEqual("Kitchen repair", quote.CustomerName);
        Assert.AreEqual("1 High Street", quote.AddressLine1);
        Assert.AreEqual("Original scope", quote.Notes);
        Assert.AreEqual("Painting", quote.Lines.Single().Description);
        Assert.AreEqual(150m, quote.Total);
    }

    [TestMethod]
    public void QuotesRejectMissingPricesAndEmptyJobs()
    {
        var job = CreateJob();
        job.ServiceLinks.Single().UnitPrice = null;
        Assert.ThrowsExactly<InvalidOperationException>(() => Quote.FromJob(job));
        job.ServiceLinks.Clear();
        Assert.ThrowsExactly<InvalidOperationException>(() => Quote.FromJob(job));
    }

    [TestMethod]
    public void RevisedQuoteCapturesNewPricesWithoutChangingOriginal()
    {
        var job = CreateJob();
        var first = Quote.FromJob(job);
        job.ServiceLinks.Single().UnitPrice = 90m;
        var second = Quote.FromJob(job);
        Assert.AreNotEqual(first.QuoteNumber, second.QuoteNumber);
        Assert.AreEqual(150m, first.Total);
        Assert.AreEqual(180m, second.Total);
    }

    [TestMethod]
    public async Task SavedQuoteExportsAsPdfIncludingMultiplePages()
    {
        var quote = Quote.FromJob(CreateJob());
        quote.Lines = Enumerable.Range(0, 100).Select(index => new QuoteLine
        {
            Id = Guid.NewGuid(), SortOrder = index, Description = $"Service {index}",
            Quantity = 2m, UnitPrice = 75m
        }).ToList();
        var service = CRM.Tests.Medias.MediaTests.TestProxy.Create<IQuoteService>((method, args) =>
            method.Name == nameof(IQuoteService.GetByIdAsync)
                ? Task.FromResult<Quote?>(quote)
                : throw new NotSupportedException(method.Name));
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        var document = new CRM.Infrastructure.Quotes.QuoteDocumentService(service);
        var bytes = await document.GenerateQuoteAsync(quote.Id);
        Assert.IsTrue(bytes.Length > 1000);
        Assert.AreEqual("%PDF-", System.Text.Encoding.ASCII.GetString(bytes, 0, 5));
    }
}
