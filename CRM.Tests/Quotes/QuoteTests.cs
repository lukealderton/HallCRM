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
        Notes = "Original scope", Description = "Repair and repaint the kitchen", Entity = new CrmEntity(),
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
        job.Description = "Changed description";
        var service = job.ServiceLinks.Single();
        service.Service.Name = "Renamed service";
        service.Quantity = 8m;
        service.UnitPrice = 100m;
        job.ServiceLinks.Clear();

        Assert.AreEqual("Kitchen repair", quote.JobName);
        Assert.AreEqual("Kitchen repair", quote.CustomerName);
        Assert.AreEqual("1 High Street", quote.AddressLine1);
        Assert.AreEqual("Original scope", quote.Notes);
        Assert.AreEqual("Repair and repaint the kitchen", quote.JobDescription);
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

    [TestMethod]
    public async Task InvoiceSnapshotsJobDescriptionOnCreation()
    {
        var job = CreateJob();
        CRM.Core.Invoices.Domain.Invoice? stored = null;
        var jobs = CRM.Tests.Medias.MediaTests.TestProxy.Create<CRM.Core.Jobs.Abstractions.IJobService>((method, args) =>
            method.Name == "GetJobByIdAsync" ? Task.FromResult<Job?>(job) : throw new NotSupportedException(method.Name));
        var repository = CRM.Tests.Medias.MediaTests.TestProxy.Create<CRM.Core.Invoices.Abstractions.IInvoiceRepository>((method, args) =>
        {
            switch (method.Name)
            {
                case "InvoiceNumberExistsAsync": return Task.FromResult(false);
                case "AddInvoiceAsync":
                    stored = (CRM.Core.Invoices.Domain.Invoice)args![0]!;
                    return Task.CompletedTask;
                case "GetInvoiceByIdAsync": return Task.FromResult(stored);
                default: throw new NotSupportedException(method.Name);
            }
        });
        var service = new CRM.Core.Invoices.Services.InvoiceService(repository, jobs);
        var invoice = await service.CreateFromJobAsync(job.Id);
        job.Description = "Changed description";
        Assert.AreEqual("Repair and repaint the kitchen", invoice.JobDescription);
    }
}
