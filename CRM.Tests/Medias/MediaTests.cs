using System.Reflection;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using CRM.Core.Common.Abstraction;
using CRM.Core.Common.Configuration;
using CRM.Core.Entities.Domain;
using CRM.Core.Jobs.Abstractions;
using CRM.Core.Jobs.Domain;
using CRM.Core.Medias.Abstractions;
using CRM.Core.Medias.Domain;
using CRM.Core.Medias.Services;
using CRM.Core.Users.Abstraction.Services;
using CRM.Core.Users.Domain;
using CRM.Infrastructure.Medias.Storage;
using CRM.Web.Medias;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRM.Tests.Medias;

[TestClass]
public sealed class MediaTests
{
    private readonly Guid objUserId = Guid.NewGuid();
    private readonly Job objJob = new() { Id = Guid.NewGuid(), Entity = new CrmEntity() };
    private readonly MemoryMediaRepository objRepository = new();
    private readonly CRMConfiguration objConfiguration = new();
    private String strTestRoot = String.Empty;
    private LocalFileMediaStorage objStorage = null!;
    private MediaService objService = null!;
    private Boolean blnUserEnabled = true;

    [TestInitialize]
    public void Initialize()
    {
        strTestRoot = Path.Combine(Path.GetTempPath(), "HallCRM.MediaTests", Guid.NewGuid().ToString("N"));
        objConfiguration.MediaSettings.Root = "private-media";
        objConfiguration.MediaSettings.MaxFileSizeBytes = 1024;
        IOptions<CRMConfiguration> objOptions = Options.Create(objConfiguration);
        objStorage = new(objOptions, new TestPaths(strTestRoot));
        IJobService objJobs = TestProxy.Create<IJobService>((objMethod, colArgs) =>
            objMethod.Name == nameof(IJobService.GetJobByIdAsync)
                ? Task.FromResult((Guid)colArgs![0]! == objJob.Id ? objJob : null)
                : throw new NotSupportedException());
        IUserService objUsers = TestProxy.Create<IUserService>((objMethod, colArgs) =>
            objMethod.Name == nameof(IUserService.GetUserAsync)
                ? Task.FromResult<User?>((Guid)colArgs![0]! == objUserId ? new User { Id = objUserId, Enabled = blnUserEnabled } : null)
                : throw new NotSupportedException());
        objService = new(objRepository, objStorage, objJobs, objUsers, objOptions, NullLogger<MediaService>.Instance);
    }

    [TestCleanup]
    public void Cleanup()
    {
        String strBase = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "HallCRM.MediaTests")) + Path.DirectorySeparatorChar;
        Assert.IsTrue(Path.GetFullPath(strTestRoot).StartsWith(strBase, StringComparison.OrdinalIgnoreCase));
        if (Directory.Exists(strTestRoot)) Directory.Delete(strTestRoot, true);
    }

    [TestMethod]
    public async Task UploadListReadAndDeletePreservesOriginalAndHidesRemovedMedia()
    {
        Byte[] colContent = "%PDF-1.7\noriginal bytes"u8.ToArray();
        Media objMedia = await objService.SaveMediaAsync(objJob.Id, objUserId, "C:\\fakepath\\report.pdf", new MemoryStream(colContent));
        Assert.AreEqual("report.pdf", objMedia.FileName);
        Assert.AreEqual("application/pdf", objMedia.ContentType);
        Assert.AreEqual((Int64)colContent.Length, objMedia.SizeBytes);
        Assert.AreEqual(objUserId, objMedia.OwnerId);
        Assert.AreEqual(1, (await objService.GetByJobIdAsync(objJob.Id, objUserId)).Count);
        MediaReadResult? objRead = await objService.OpenForReadAsync(objMedia.Id, objUserId);
        Assert.IsNotNull(objRead);
        await using (objRead.Content)
        {
            using MemoryStream stmCopy = new();
            await objRead.Content.CopyToAsync(stmCopy);
            CollectionAssert.AreEqual(colContent, stmCopy.ToArray());
        }

        await objService.DeleteAsync(objMedia.Id, objUserId);
        Assert.AreEqual(0, (await objService.GetByJobIdAsync(objJob.Id, objUserId)).Count);
        Assert.IsNull(await objService.OpenForReadAsync(objMedia.Id, objUserId));
        Assert.AreEqual(objUserId, objRepository.Items[0].DeletedByUserId);
        Assert.AreEqual(0, Directory.GetFiles(strTestRoot, "*", SearchOption.AllDirectories).Length);
    }

    [TestMethod]
    public async Task InvalidFileSignatureLeavesNoFileOrMetadata()
    {
        await Assert.ThrowsAsync<InvalidDataException>(() => objService.SaveMediaAsync(
            objJob.Id, objUserId, "photo.jpg", new MemoryStream("<script>bad</script>"u8.ToArray())));
        Assert.AreEqual(0, objRepository.Items.Count);
        Assert.AreEqual(0, Directory.GetFiles(strTestRoot, "*", SearchOption.AllDirectories).Length);
    }

    [TestMethod]
    public async Task OversizedUploadCleansTemporaryFile()
    {
        await Assert.ThrowsAsync<InvalidDataException>(() => objService.SaveMediaAsync(
            objJob.Id, objUserId, "large.pdf", new MemoryStream(new Byte[1025])));
        Assert.AreEqual(0, objRepository.Items.Count);
        Assert.AreEqual(0, Directory.GetFiles(strTestRoot, "*", SearchOption.AllDirectories).Length);
    }

    [TestMethod]
    public async Task MetadataFailureRemovesSavedFile()
    {
        objRepository.FailAdd = true;
        await Assert.ThrowsAsync<InvalidOperationException>(() => objService.SaveMediaAsync(
            objJob.Id, objUserId, "report.pdf", new MemoryStream("%PDF-1.7\nvalid"u8.ToArray())));
        Assert.AreEqual(0, Directory.GetFiles(strTestRoot, "*", SearchOption.AllDirectories).Length);
    }

    [TestMethod]
    public async Task DisabledOrUnknownUserCannotReadUploadOrDelete()
    {
        Media objMedia = await SavePdfAsync();
        blnUserEnabled = false;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => objService.GetByJobIdAsync(objJob.Id, objUserId));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => objService.OpenForReadAsync(objMedia.Id, objUserId));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => SavePdfAsync());
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => objService.DeleteAsync(objMedia.Id, objUserId));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => objService.GetByJobIdAsync(objJob.Id, Guid.Empty));
        Assert.IsNull(objMedia.DeletedUtc);
    }

    [TestMethod]
    public async Task ArchivedJobAllowsReadButBlocksUploadAndDelete()
    {
        Media objMedia = await SavePdfAsync();
        objJob.Entity.ArchivedUtc = DateTime.UtcNow;
        Assert.AreEqual(1, (await objService.GetByJobIdAsync(objJob.Id, objUserId)).Count);
        MediaReadResult? objRead = await objService.OpenForReadAsync(objMedia.Id, objUserId);
        Assert.IsNotNull(objRead);
        await objRead.Content.DisposeAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => SavePdfAsync());
        await Assert.ThrowsAsync<InvalidOperationException>(() => objService.DeleteAsync(objMedia.Id, objUserId));
    }

    [TestMethod]
    public async Task DeletedOrMissingJobBlocksAccess()
    {
        Media objMedia = await SavePdfAsync();
        objJob.Entity.DeletedUtc = DateTime.UtcNow;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => objService.OpenForReadAsync(objMedia.Id, objUserId));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => SavePdfAsync());
        await Assert.ThrowsAsync<KeyNotFoundException>(() => objService.GetByJobIdAsync(Guid.NewGuid(), objUserId));
    }

    [TestMethod]
    public async Task MissingFileReturnsNotFound()
    {
        Media objMedia = await SavePdfAsync();
        await objStorage.DeleteAsync(objMedia.Key);
        Assert.IsNull(await objService.OpenForReadAsync(objMedia.Id, objUserId));
    }

    [TestMethod]
    public async Task NonSeekableUploadWorksAndDuplicateNamesUseUniqueKeys()
    {
        using Stream stmFirst = new NonSeekableStream("%PDF-1.7\nfirst"u8.ToArray());
        Media objFirst = await objService.SaveMediaAsync(objJob.Id, objUserId, "report.pdf", stmFirst);
        Media objSecond = await SavePdfAsync();
        Assert.AreNotEqual(objFirst.Key, objSecond.Key);
        Assert.AreEqual(2, (await objService.GetByJobIdAsync(objJob.Id, objUserId)).Count);
    }

    [TestMethod]
    public async Task CancellationCleansPartialUpload()
    {
        using CancellationTokenSource objCancellation = new();
        using Stream stmContent = new CancellingStream(objCancellation);
        await Assert.ThrowsAsync<OperationCanceledException>(() => objService.SaveMediaAsync(
            objJob.Id, objUserId, "report.pdf", stmContent, objCancellation.Token));
        Assert.AreEqual(0, objRepository.Items.Count);
        Assert.AreEqual(0, Directory.GetFiles(strTestRoot, "*", SearchOption.AllDirectories).Length);
    }

    [TestMethod]
    public async Task StorageRejectsTraversalAbsolutePathsAndSiblingPrefixEscape()
    {
        foreach (String strKey in new[] { "../escape.pdf", "../private-media-other/escape.pdf", "..\\escape.pdf", "C:\\escape.pdf", "/escape.pdf" })
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => objStorage.SaveAsync(strKey, new MemoryStream([1]), 1024));
        }
    }

    [TestMethod]
    public void StorageRejectsPublicWebRoot()
    {
        objConfiguration.MediaSettings.Root = "wwwroot/uploads";
        Assert.Throws<InvalidOperationException>(() => new LocalFileMediaStorage(Options.Create(objConfiguration), new TestPaths(strTestRoot)));
    }

    [TestMethod]
    public async Task UnsupportedExtensionRejectedBeforeSaving()
    {
        await Assert.ThrowsAsync<InvalidDataException>(() => objService.SaveMediaAsync(
            objJob.Id, objUserId, "unsafe.svg", new MemoryStream("<svg/>"u8.ToArray())));
        Assert.IsFalse(Directory.Exists(strTestRoot));
    }

    [TestMethod]
    public async Task HttpMediaRouteRequiresAuthenticationSupportsRangesAndDownloads()
    {
        Media objMedia = await SavePdfAsync();
        WebApplicationBuilder objBuilder = WebApplication.CreateBuilder();
        objBuilder.Logging.ClearProviders();
        objBuilder.Services.AddSingleton<IMediaService>(objService);
        objBuilder.Services.AddAuthentication("MediaTest").AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("MediaTest", _ => { });
        objBuilder.Services.AddAuthorization();
        await using WebApplication objApp = objBuilder.Build();
        objApp.Urls.Add("http://127.0.0.1:0");
        objApp.UseAuthentication();
        objApp.UseAuthorization();
        objApp.MapMediaEndpoints();
        await objApp.StartAsync();
        using HttpClient objClient = new() { BaseAddress = new Uri(objApp.Urls.Single()) };

        using HttpResponseMessage objAnonymous = await objClient.GetAsync(objMedia.Url);
        Assert.AreEqual(HttpStatusCode.Unauthorized, objAnonymous.StatusCode);

        objClient.DefaultRequestHeaders.Add("X-Media-Test-User", objUserId.ToString());
        using HttpResponseMessage objDownload = await objClient.GetAsync(objMedia.Url + "?download=true");
        Assert.AreEqual(HttpStatusCode.OK, objDownload.StatusCode);
        Assert.AreEqual("application/pdf", objDownload.Content.Headers.ContentType?.MediaType);
        Assert.AreEqual("attachment", objDownload.Content.Headers.ContentDisposition?.DispositionType);
        Assert.IsTrue(objDownload.Headers.CacheControl?.NoStore);
        Assert.AreEqual("nosniff", objDownload.Headers.GetValues("X-Content-Type-Options").Single());

        using HttpRequestMessage objRangeRequest = new(HttpMethod.Get, objMedia.Url);
        objRangeRequest.Headers.Range = new RangeHeaderValue(0, 4);
        using HttpResponseMessage objRange = await objClient.SendAsync(objRangeRequest);
        Assert.AreEqual(HttpStatusCode.PartialContent, objRange.StatusCode);
        CollectionAssert.AreEqual("%PDF-"u8.ToArray(), await objRange.Content.ReadAsByteArrayAsync());

        Byte[] colPng = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+a2XQAAAAASUVORK5CYII=");
        Media objImage = await objService.SaveMediaAsync(objJob.Id, objUserId, "photo.png", new MemoryStream(colPng));
        using HttpResponseMessage objPreview = await objClient.GetAsync(objImage.Url);
        Assert.AreEqual(HttpStatusCode.OK, objPreview.StatusCode);
        Assert.AreEqual("image/png", objPreview.Content.Headers.ContentType?.MediaType);
        Assert.IsNull(objPreview.Content.Headers.ContentDisposition);
        CollectionAssert.AreEqual(colPng, await objPreview.Content.ReadAsByteArrayAsync());

        blnUserEnabled = false;
        using HttpResponseMessage objDisabled = await objClient.GetAsync(objMedia.Url);
        Assert.AreEqual(HttpStatusCode.Forbidden, objDisabled.StatusCode);
        blnUserEnabled = true;
        objJob.Entity.DeletedUtc = DateTime.UtcNow;
        using HttpResponseMessage objDeleted = await objClient.GetAsync(objMedia.Url);
        Assert.AreEqual(HttpStatusCode.NotFound, objDeleted.StatusCode);
        await objApp.StopAsync();
    }

    private Task<Media> SavePdfAsync() => objService.SaveMediaAsync(
        objJob.Id, objUserId, "report.pdf", new MemoryStream("%PDF-1.7\nvalid"u8.ToArray()));

    private sealed record TestPaths(String ContentRootPath) : IAppPathProvider
    {
        public String WebRootPath => Path.Combine(ContentRootPath, "wwwroot");
    }

    private sealed class MemoryMediaRepository : IMediaRepository
    {
        public List<Media> Items { get; } = [];
        public Boolean FailAdd { get; set; }
        public Task<Media?> GetAsync(Guid objId, CancellationToken objToken = default) => Task.FromResult(Items.FirstOrDefault(objMedia => objMedia.Id == objId && objMedia.DeletedUtc == null));
        public Task<List<Media>> GetByJobIdAsync(Guid objId, CancellationToken objToken = default) => Task.FromResult(Items.Where(objMedia => objMedia.JobId == objId && objMedia.DeletedUtc == null).ToList());
        public Task AddAsync(Media objMedia, CancellationToken objToken = default)
        {
            if (FailAdd) throw new InvalidOperationException("Simulated database failure");
            Items.Add(objMedia);
            return Task.CompletedTask;
        }
        public Task MarkDeletedAsync(Guid objId, Guid objUserId, CancellationToken objToken = default)
        {
            Media objMedia = Items.Single(objItem => objItem.Id == objId);
            objMedia.DeletedUtc = DateTime.UtcNow;
            objMedia.DeletedByUserId = objUserId;
            return Task.CompletedTask;
        }
    }

    public class TestProxy : DispatchProxy
    {
        public Func<MethodInfo, Object?[]?, Object?> Handler { get; set; } = null!;
        protected override Object? Invoke(MethodInfo? objMethod, Object?[]? colArgs) => Handler(objMethod!, colArgs);
        public static T Create<T>(Func<MethodInfo, Object?[]?, Object?> objHandler) where T : class
        {
            T objProxy = Create<T, TestProxy>();
            ((TestProxy)(Object)objProxy).Handler = objHandler;
            return objProxy;
        }
    }

    public sealed class TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> objOptions, ILoggerFactory objLogger, UrlEncoder objEncoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(objOptions, objLogger, objEncoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Media-Test-User", out var strUserId))
                return Task.FromResult(AuthenticateResult.NoResult());
            ClaimsPrincipal objPrincipal = new(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, strUserId.ToString())], Scheme.Name));
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(objPrincipal, Scheme.Name)));
        }
    }

    private sealed class NonSeekableStream(Byte[] colContent) : MemoryStream(colContent)
    {
        public override Boolean CanSeek => false;
        public override Int64 Seek(Int64 lngOffset, SeekOrigin enmOrigin) => throw new NotSupportedException();
    }

    private sealed class CancellingStream(CancellationTokenSource objCancellation) : MemoryStream("%PDF-1.7\nvalid"u8.ToArray())
    {
        public override ValueTask<Int32> ReadAsync(Memory<Byte> colBuffer, CancellationToken objToken = default)
        {
            objCancellation.Cancel();
            return ValueTask.FromCanceled<Int32>(objCancellation.Token);
        }
    }
}
