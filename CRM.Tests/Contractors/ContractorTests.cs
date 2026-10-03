using System.ComponentModel.DataAnnotations;
using CRM.Core.Contractors.Abstractions;
using CRM.Core.Contractors.Domain;
using CRM.Core.Contractors.Services;
using CRM.Core.Entities.Domain;
using CRM.Core.Jobs.Abstractions;
using CRM.Core.Jobs.Domain;
using CRM.Core.Jobs.Services;
using CRM.Infrastructure.Migrations;
using CRM.Tests.Medias;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRM.Tests.Contractors;

[TestClass]
public sealed class ContractorTests
{
    private readonly MemoryContractorRepository objContractors = new();
    private Job? objStoredJob;
    private Guid? objFilteredContractorId;
    private Boolean blnUnassignedOnly;
    private Boolean blnSavedJob;

    private JobService CreateJobService()
    {
        IJobRepository objJobs = MediaTests.TestProxy.Create<IJobRepository>((objMethod, colArgs) =>
        {
            switch (objMethod.Name)
            {
                case nameof(IJobRepository.GetJobByIdAsync): return Task.FromResult(objStoredJob);
                case nameof(IJobRepository.AddJobAsync):
                case nameof(IJobRepository.UpdateJobAsync):
                    objStoredJob = (Job)colArgs![0]!;
                    blnSavedJob = true;
                    return Task.CompletedTask;
                case nameof(IJobRepository.SetJobServicesAsync): return Task.CompletedTask;
                case nameof(IJobRepository.GetJobsAsync):
                    objFilteredContractorId = (Guid?)colArgs![4];
                    blnUnassignedOnly = (Boolean)colArgs[5]!;
                    return Task.FromResult(new List<Job>());
                default: throw new NotSupportedException(objMethod.Name);
            }
        });
        return new JobService(objJobs, objContractors);
    }

    [TestMethod]
    public async Task NewContractorGetsAnIdAndCleanContactDetails()
    {
        ContractorService objService = new(objContractors);
        Contractor objContractor = await objService.SaveContractorAsync(new Contractor
        {
            Name = "  Jane Smith  ", CompanyName = "  Repairs Ltd  ",
            Email = "  jane@example.com  ", Phone = "  07123456789  ", Notes = "   "
        });
        Assert.AreNotEqual(Guid.Empty, objContractor.Id);
        Assert.AreEqual("Jane Smith", objContractor.Name);
        Assert.AreEqual("Repairs Ltd", objContractor.CompanyName);
        Assert.AreEqual("jane@example.com", objContractor.Email);
        Assert.AreEqual("07123456789", objContractor.Phone);
        Assert.IsNull(objContractor.Notes);
        Assert.IsTrue(objContractor.Enabled);
        Assert.AreNotEqual(default(DateTime), objContractor.CreatedUtc);
        Assert.AreEqual(1, objContractors.Items.Count);
    }

    [TestMethod]
    public async Task InvalidNameOrEmailCannotBeSaved()
    {
        ContractorService objService = new(objContractors);
        await Assert.ThrowsAsync<ValidationException>(() => objService.SaveContractorAsync(new Contractor { Name = "   " }));
        await Assert.ThrowsAsync<ValidationException>(() => objService.SaveContractorAsync(new Contractor { Name = "Jane", Email = "invalid" }));
        Assert.AreEqual(0, objContractors.Items.Count);
    }

    [TestMethod]
    public async Task FailedNewContractorSaveCanBeRetriedFromSameForm()
    {
        ContractorService objService = new(objContractors);
        Contractor objForm = new() { Name = "Jane" };
        objContractors.FailNextSave = true;
        await Assert.ThrowsAsync<InvalidOperationException>(() => objService.SaveContractorAsync(objForm));
        Assert.AreEqual(Guid.Empty, objForm.Id);
        await objService.SaveContractorAsync(objForm);
        Assert.AreEqual(1, objContractors.Items.Count);
    }

    [TestMethod]
    public async Task DeactivatingContractorPreservesCreationDateAndCanBeIncludedInLookups()
    {
        Contractor objOriginal = AddContractor(true);
        ContractorService objService = new(objContractors);
        Contractor objUpdated = await objService.SaveContractorAsync(new Contractor
        {
            Id = objOriginal.Id, Name = "Jane", Enabled = false, CreatedUtc = DateTime.UtcNow
        });
        Assert.AreEqual(objOriginal.CreatedUtc, objUpdated.CreatedUtc);
        Assert.IsNotNull(objUpdated.UpdatedUtc);
        Assert.AreEqual(0, (await objService.GetContractorsAsync()).Count);
        Assert.AreEqual(1, (await objService.GetContractorsAsync(true)).Count);
    }

    [TestMethod]
    public async Task ActiveContractorCanBeAssignedToNewJob()
    {
        Contractor objContractor = AddContractor(true);
        Job objSaved = await CreateJobService().AddJobAsync(new Job { Name = "Repair", AssignedContractorId = objContractor.Id });
        Assert.AreEqual(objContractor.Id, objSaved.AssignedContractorId);
        Assert.IsTrue(blnSavedJob);
    }

    [TestMethod]
    public async Task InactiveAndMissingContractorsCannotReceiveNewJobs()
    {
        Contractor objContractor = AddContractor(false);
        JobService objService = CreateJobService();
        await Assert.ThrowsAsync<ArgumentException>(() => objService.AddJobAsync(new Job { Name = "Repair", AssignedContractorId = objContractor.Id }));
        await Assert.ThrowsAsync<ArgumentException>(() => objService.AddJobAsync(new Job { Name = "Repair", AssignedContractorId = Guid.NewGuid() }));
        Assert.IsFalse(blnSavedJob);
    }

    [TestMethod]
    public async Task ExistingInactiveAssignmentCanBeKeptWhenEditingJob()
    {
        Contractor objContractor = AddContractor(false);
        objStoredJob = new Job { Id = Guid.NewGuid(), Name = "Repair", AssignedContractorId = objContractor.Id, Entity = new CrmEntity() };
        Job? objUpdated = await CreateJobService().UpdateJobAsync(new Job
        {
            Id = objStoredJob.Id, Name = "Repair updated", AssignedContractorId = objContractor.Id
        });
        Assert.IsNotNull(objUpdated);
        Assert.AreEqual(objContractor.Id, objUpdated.AssignedContractorId);
        Assert.AreEqual("Repair updated", objUpdated.Name);
    }

    [TestMethod]
    public async Task ReassignmentToInactiveContractorIsRejectedBeforeSaving()
    {
        Contractor objContractor = AddContractor(false);
        objStoredJob = new Job { Id = Guid.NewGuid(), Name = "Repair", Entity = new CrmEntity() };
        await Assert.ThrowsAsync<ArgumentException>(() => CreateJobService().UpdateJobAsync(new Job
        {
            Id = objStoredJob.Id, Name = "Repair", AssignedContractorId = objContractor.Id
        }));
        Assert.IsNull(objStoredJob.AssignedContractorId);
        Assert.IsFalse(blnSavedJob);
    }

    [TestMethod]
    public async Task AssignmentCanBeClearedAndNewJobsCanBeUnassigned()
    {
        Contractor objContractor = AddContractor(false);
        objStoredJob = new Job { Id = Guid.NewGuid(), Name = "Repair", AssignedContractorId = objContractor.Id, Entity = new CrmEntity() };
        JobService objService = CreateJobService();
        Job? objUpdated = await objService.UpdateJobAsync(new Job { Id = objStoredJob.Id, Name = "Repair" });
        Assert.IsNotNull(objUpdated);
        Assert.IsNull(objUpdated.AssignedContractorId);
        Job objNew = await objService.AddJobAsync(new Job { Name = "New repair" });
        Assert.IsNull(objNew.AssignedContractorId);
    }

    [TestMethod]
    public async Task JobsFiltersUseContractorIdAndUnassignedFlag()
    {
        Guid objId = Guid.NewGuid();
        await CreateJobService().GetJobsAsync(objAssignedContractorId: objId, blnUnassignedOnly: true);
        Assert.AreEqual(objId, objFilteredContractorId);
        Assert.IsTrue(blnUnassignedOnly);
    }

    [TestMethod]
    public void MigrationRenamesAssignmentAndCreatesContractorsBeforeAddingForeignKey()
    {
        var colOperations = new AddContractorsAndJobAssignments().UpOperations;
        Assert.AreEqual(0, colOperations.OfType<DropColumnOperation>().Count());
        RenameColumnOperation objRename = colOperations.OfType<RenameColumnOperation>().Single();
        Assert.AreEqual("jobAssignedUserId", objRename.Name);
        Assert.AreEqual("jobAssignedContractorId", objRename.NewName);
        Int32 intImport = colOperations.ToList().FindIndex(objOperation => objOperation is SqlOperation);
        Int32 intForeignKey = colOperations.ToList().FindIndex(objOperation => objOperation is AddForeignKeyOperation);
        Assert.IsTrue(intImport >= 0 && intImport < intForeignKey);
        StringAssert.Contains(((SqlOperation)colOperations[intImport]).Sql, "LEFT JOIN [T_User]");
    }

    private Contractor AddContractor(Boolean blnEnabled)
    {
        Contractor objContractor = new() { Id = Guid.NewGuid(), Name = "Jane", Enabled = blnEnabled, CreatedUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        objContractors.Items.Add(objContractor);
        return objContractor;
    }

    private sealed class MemoryContractorRepository : IContractorRepository
    {
        public List<Contractor> Items { get; } = [];
        public Boolean FailNextSave { get; set; }
        public Task<Contractor?> GetContractorAsync(Guid objId, CancellationToken objToken = default) => Task.FromResult(Items.FirstOrDefault(objContractor => objContractor.Id == objId));
        public Task<List<Contractor>> GetContractorsAsync(Boolean blnIncludeInactive = false, String? strSearch = null, CancellationToken objToken = default) => Task.FromResult(Items.Where(objContractor => blnIncludeInactive || objContractor.Enabled).ToList());
        public Task SaveAsync(Contractor objContractor, Boolean blnNew, CancellationToken objToken = default)
        {
            if (FailNextSave)
            {
                FailNextSave = false;
                throw new InvalidOperationException("Simulated database failure");
            }
            Items.RemoveAll(objItem => objItem.Id == objContractor.Id);
            Items.Add(objContractor);
            return Task.CompletedTask;
        }
    }
}
