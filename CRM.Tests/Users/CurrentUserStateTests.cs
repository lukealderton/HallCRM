using System.Security.Claims;
using CRM.Core.Entities.Domain;
using CRM.Core.Invoices.Abstractions;
using CRM.Core.Invoices.Domain;
using CRM.Core.Invoices.Services;
using CRM.Core.Jobs.Abstractions;
using CRM.Core.Jobs.Domain;
using CRM.Core.Users.Abstraction.Services;
using CRM.Core.Users.Domain;
using CRM.Tests.Medias;
using CRM.Web.Users.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRM.Tests.Users;

[TestClass]
public sealed class CurrentUserStateTests
{
    private static CurrentUserState CreateState(
        IUserService objUsers, Boolean blnAuthenticated, params Claim[] colClaims) =>
        new(objUsers, new TestAuthenticationStateProvider(new ClaimsPrincipal(
            new ClaimsIdentity(colClaims, blnAuthenticated ? "Test" : null))));

    [TestMethod]
    public async Task InvoiceFromJobUsesDomainUserIdWhenLoginIdIsDifferent()
    {
        String strIdentityId = Guid.NewGuid().ToString();
        User objUser = new() { Id = Guid.NewGuid() };
        Int32 intLookups = 0;
        IUserService objUsers = MediaTests.TestProxy.Create<IUserService>((objMethod, colArgs) =>
        {
            Assert.AreEqual(nameof(IUserService.GetUserByIdentityIdAsync), objMethod.Name);
            Assert.AreEqual(strIdentityId, colArgs![0]);
            intLookups++;
            return Task.FromResult<User?>(objUser);
        });
        CurrentUserState objState = CreateState(objUsers, true,
            new Claim(ClaimTypes.NameIdentifier, strIdentityId));

        Job objJob = new()
        {
            Id = Guid.NewGuid(), Name = "Repair", Entity = new CrmEntity(),
            ServiceLinks = [new JobServiceLink
            {
                Service = new CRM.Core.Services.Domain.Service { Name = "Painting" },
                Quantity = 2m, UnitPrice = 75m
            }]
        };
        IJobService objJobs = MediaTests.TestProxy.Create<IJobService>((objMethod, _) =>
        {
            Assert.AreEqual(nameof(IJobService.GetJobByIdAsync), objMethod.Name);
            return Task.FromResult<Job?>(objJob);
        });
        Invoice? objSaved = null;
        IInvoiceRepository objInvoices = MediaTests.TestProxy.Create<IInvoiceRepository>((objMethod, colArgs) =>
        {
            switch (objMethod.Name)
            {
                case nameof(IInvoiceRepository.InvoiceNumberExistsAsync): return Task.FromResult(false);
                case nameof(IInvoiceRepository.AddInvoiceAsync):
                    objSaved = (Invoice)colArgs![0]!;
                    Assert.AreEqual(objUser.Id, objSaved.Entity.CreatedByUserId);
                    Assert.AreEqual(objUser.Id, objSaved.Entity.OwnerUserId);
                    return Task.CompletedTask;
                case nameof(IInvoiceRepository.GetInvoiceByIdAsync): return Task.FromResult(objSaved);
                default: throw new NotSupportedException(objMethod.Name);
            }
        });

        Invoice objInvoice = await new InvoiceService(objInvoices, objJobs)
            .CreateFromJobAsync(objJob.Id, await objState.GetCurrentUserIdAsync());

        Assert.AreSame(objSaved, objInvoice);
        Assert.AreEqual(objUser.Id, await objState.GetCurrentUserIdAsync());
        Assert.AreSame(objUser, await objState.GetAsync());
        Assert.AreEqual(1, intLookups);
    }

    [TestMethod]
    public async Task IdentityAccountTakesPrecedenceOverCustomIdClaim()
    {
        User objUser = new() { Id = Guid.NewGuid() };
        IUserService objUsers = MediaTests.TestProxy.Create<IUserService>((objMethod, colArgs) =>
        {
            Assert.AreEqual(nameof(IUserService.GetUserByIdentityIdAsync), objMethod.Name);
            Assert.AreEqual("string-identity-key", colArgs![0]);
            return Task.FromResult<User?>(objUser);
        });
        CurrentUserState objState = CreateState(objUsers, true,
            new Claim("Id", Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, "string-identity-key"));

        Assert.AreEqual(objUser.Id, await objState.GetCurrentUserIdAsync());
    }

    [TestMethod]
    public async Task UnknownAccountDoesNotReturnLoginOrCustomIdAsDomainId()
    {
        IUserService objUsers = MediaTests.TestProxy.Create<IUserService>((objMethod, _) =>
        {
            Assert.AreEqual(nameof(IUserService.GetUserByIdentityIdAsync), objMethod.Name);
            return Task.FromResult<User?>(null);
        });
        CurrentUserState objState = CreateState(objUsers, true,
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim("Id", Guid.NewGuid().ToString()));

        Assert.IsNull(await objState.GetCurrentUserIdAsync());
        Assert.IsNull(objState.CurrentUserId);
        Assert.IsNull(objState.CurrentUser);
    }

    [TestMethod]
    public async Task LegacyDomainClaimIsValidatedAgainstUserService()
    {
        User objUser = new() { Id = Guid.NewGuid() };
        IUserService objUsers = MediaTests.TestProxy.Create<IUserService>((objMethod, colArgs) =>
        {
            Assert.AreEqual(nameof(IUserService.GetUserAsync), objMethod.Name);
            Assert.AreEqual(objUser.Id, colArgs![0]);
            return Task.FromResult<User?>(objUser);
        });
        CurrentUserState objState = CreateState(objUsers, true, new Claim("Id", objUser.Id.ToString()));

        Assert.AreEqual(objUser.Id, await objState.GetCurrentUserIdAsync());
    }

    [TestMethod]
    public async Task UnknownLegacyDomainClaimReturnsNull()
    {
        IUserService objUsers = MediaTests.TestProxy.Create<IUserService>((_, _) => Task.FromResult<User?>(null));
        CurrentUserState objState = CreateState(objUsers, true, new Claim("Id", Guid.NewGuid().ToString()));

        Assert.IsNull(await objState.GetCurrentUserIdAsync());
    }

    [TestMethod]
    public async Task AnonymousAndInvalidPrincipalsDoNotResolveAUser()
    {
        IUserService objUsers = MediaTests.TestProxy.Create<IUserService>((_, _) =>
            throw new AssertFailedException("No user lookup should occur."));

        Assert.IsNull(await CreateState(objUsers, false,
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())).GetCurrentUserIdAsync());
        Assert.IsNull(await CreateState(objUsers, true).GetCurrentUserIdAsync());
        Assert.IsNull(await CreateState(objUsers, true, new Claim("Id", "invalid")).GetCurrentUserIdAsync());
        Assert.IsNull(await CreateState(objUsers, true, new Claim("Id", Guid.Empty.ToString())).GetCurrentUserIdAsync());
    }

    [TestMethod]
    public async Task EmptyDomainIdIsNotCached()
    {
        IUserService objUsers = MediaTests.TestProxy.Create<IUserService>((_, _) =>
            Task.FromResult<User?>(new User { Id = Guid.Empty }));
        CurrentUserState objState = CreateState(objUsers, true,
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));

        Assert.IsNull(await objState.GetCurrentUserIdAsync());
        Assert.IsNull(objState.CurrentUser);
    }

    private sealed class TestAuthenticationStateProvider(ClaimsPrincipal objPrincipal)
        : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(new AuthenticationState(objPrincipal));
    }
}
