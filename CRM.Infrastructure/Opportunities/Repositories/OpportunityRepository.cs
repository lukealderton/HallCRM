using CRM.Core.Opportunities.Abstractions;
using CRM.Core.Opportunities.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Opportunities.Repositories
{
    public sealed class OpportunityRepository : IOpportunityRepository
    {
        private readonly CRMDbContext _context;

        public OpportunityRepository(CRMDbContext objContext)
        {
            _context = objContext;
        }

        public Task<Opportunity?> GetOpportunityByIdAsync(Guid objOpportunityId, Boolean blnTracking = false, CancellationToken objToken = default)
        {
            IQueryable<Opportunity> objQuery = _context.Opportunities
                .Include(objOpportunity => objOpportunity.Entity)
                .Include(objOpportunity => objOpportunity.Company)
                    .ThenInclude(objCompany => objCompany!.Entity)
                .Include(objOpportunity => objOpportunity.Contact)
                    .ThenInclude(objContact => objContact!.Entity);

            if (!blnTracking)
            {
                objQuery = objQuery.AsNoTracking();
            }

            return objQuery.FirstOrDefaultAsync(objOpportunity => objOpportunity.Id == objOpportunityId, objToken);
        }

        public Task<List<Opportunity>> GetOpportunitiesAsync(
            String? strSearch = null,
            OpportunityStage? enmStage = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            IQueryable<Opportunity> objQuery = _context.Opportunities
                .AsNoTracking()
                .Include(objOpportunity => objOpportunity.Entity)
                .Include(objOpportunity => objOpportunity.Company)
                    .ThenInclude(objCompany => objCompany!.Entity)
                .Include(objOpportunity => objOpportunity.Contact)
                    .ThenInclude(objContact => objContact!.Entity);

            if (!blnIncludeDeleted)
            {
                objQuery = objQuery.Where(objOpportunity => !objOpportunity.Entity.DeletedUtc.HasValue);
            }

            if (!blnIncludeArchived)
            {
                objQuery = objQuery.Where(objOpportunity => !objOpportunity.Entity.ArchivedUtc.HasValue);
            }

            if (enmStage.HasValue)
            {
                objQuery = objQuery.Where(objOpportunity => objOpportunity.Stage == enmStage.Value);
            }

            if (objCompanyId.HasValue)
            {
                objQuery = objQuery.Where(objOpportunity => objOpportunity.CompanyId == objCompanyId.Value);
            }

            if (!String.IsNullOrWhiteSpace(strSearch))
            {
                String strKeyword = strSearch.Trim();

                objQuery = objQuery.Where(objOpportunity =>
                    objOpportunity.Name.Contains(strKeyword) ||
                    (objOpportunity.Description != null && objOpportunity.Description.Contains(strKeyword)) ||
                    (objOpportunity.Source != null && objOpportunity.Source.Contains(strKeyword)) ||
                    (objOpportunity.Company != null && objOpportunity.Company.Name.Contains(strKeyword)) ||
                    (objOpportunity.Contact != null && objOpportunity.Contact.Entity.DisplayName.Contains(strKeyword)));
            }

            return objQuery
                .OrderByDescending(objOpportunity => objOpportunity.ExpectedCloseDateUtc.HasValue)
                .ThenBy(objOpportunity => objOpportunity.ExpectedCloseDateUtc)
                .ThenBy(objOpportunity => objOpportunity.Name)
                .ToListAsync(objToken);
        }

        public Task AddOpportunityAsync(Opportunity objOpportunity, CancellationToken objToken = default)
        {
            return _context.Opportunities.AddAsync(objOpportunity, objToken).AsTask();
        }

        public Task SaveChangesAsync(CancellationToken objToken = default)
        {
            return _context.SaveChangesAsync(objToken);
        }
    }
}