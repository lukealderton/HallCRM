using CRM.Core.Companies.Domain;
using CRM.Core.Contacts.Domain;
using CRM.Core.Entities.Domain;
using CRM.Core.Logging.Domain;
using CRM.Core.Notes.Domain;
using CRM.Core.Opportunities.Domain;
using CRM.Core.Tickets.Domain;
using CRM.Core.Users.Domain;
using CRM.Infrastructure.Companies.Configurations;
using CRM.Infrastructure.Contacts.Configurations;
using CRM.Infrastructure.Entities.Configurations;
using CRM.Infrastructure.Identity;
using CRM.Infrastructure.Logging.Configurations;
using CRM.Infrastructure.Notes.Configurations;
using CRM.Infrastructure.Opportunities.Configurations;
using CRM.Infrastructure.Tickets.Configurations;
using CRM.Infrastructure.Users.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Data
{
    public sealed class CRMDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        // To Create a New Migration after making changes to the model:
        // dotnet ef migrations add SummaryNameOfChanges --project CRM.Infrastructure --startup-project CRM.Web --context CRMDbContext

        // To Apply Migrations to Database:
        // dotnet ef database update --project CRM.Infrastructure --startup-project CRM.Web --context CRMDbContext

        public CRMDbContext(DbContextOptions<CRMDbContext> objOptions)
            : base(objOptions)
        {
        }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

        public DbSet<CrmEntityType> EntityTypes => Set<CrmEntityType>();
        public DbSet<CrmEntity> Entities => Set<CrmEntity>();

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<Opportunity> Opportunities => Set<Opportunity>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<Log> Logs => Set<Log>();

        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketComment> TicketComments => Set<TicketComment>();
        public DbSet<TicketTimeEntry> TicketTimeEntries => Set<TicketTimeEntry>();
        public DbSet<TicketStatusHistory> TicketStatusHistory => Set<TicketStatusHistory>();

        protected override void OnModelCreating(ModelBuilder objModelBuilder)
        {
            base.OnModelCreating(objModelBuilder);

            objModelBuilder.ApplyConfiguration(new UserConfiguration());
            objModelBuilder.ApplyConfiguration(new RoleConfiguration());
            objModelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            objModelBuilder.ApplyConfiguration(new UserClaimConfiguration());
            objModelBuilder.ApplyConfiguration(new RoleClaimConfiguration());
            objModelBuilder.ApplyConfiguration(new UserLoginConfiguration());
            objModelBuilder.ApplyConfiguration(new UserTokenConfiguration());
            objModelBuilder.ApplyConfiguration(new UserProfileConfiguration());

            objModelBuilder.ApplyConfiguration(new EntityTypeConfiguration());
            objModelBuilder.ApplyConfiguration(new EntityConfiguration());

            objModelBuilder.ApplyConfiguration(new CompanyConfiguration());
            objModelBuilder.ApplyConfiguration(new ContactConfiguration());
            objModelBuilder.ApplyConfiguration(new OpportunityConfiguration());
            objModelBuilder.ApplyConfiguration(new LogConfiguration());
            objModelBuilder.ApplyConfiguration(new NoteConfiguration());

            objModelBuilder.ApplyConfiguration(new TicketConfiguration());
            objModelBuilder.ApplyConfiguration(new TicketCommentConfiguration());
            objModelBuilder.ApplyConfiguration(new TicketTimeEntryConfiguration());
            objModelBuilder.ApplyConfiguration(new TicketStatusHistoryConfiguration());
        }
    }
}