using CRM.Core.Companies.Abstractions;
using CRM.Core.Contacts.Abstractions;
using CRM.Core.Entities.Abstractions;
using CRM.Core.Geocoding.Abstraction;
using CRM.Core.Jobs.Abstractions;
using CRM.Core.Logging.Abstraction;
using CRM.Core.Mailing.Abstraction;
using CRM.Core.Notes.Abstractions;
using CRM.Core.Notifications.Abstractions;
using CRM.Core.Tickets.Abstractions;
using CRM.Core.Users.Abstraction.Repositories;
using CRM.Infrastructure.Companies.Repositories;
using CRM.Infrastructure.Contacts.Repositories;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Entities.Repositories;
using CRM.Infrastructure.Geocoding.Services;
using CRM.Infrastructure.Logging.Repositories;
using CRM.Infrastructure.Mailing.Services;
using CRM.Infrastructure.Notes.Repositories;
using CRM.Infrastructure.Notifications.Services;
using CRM.Infrastructure.Jobs.Repositories;
using CRM.Infrastructure.Tickets.Repositories;
using CRM.Infrastructure.Users.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection colServices,
            IConfiguration objConfiguration)
        {
            String strDataConnectionString = objConfiguration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DataConnectionString' not found.");

            colServices.AddDbContextFactory<CRMDbContext>(objOptions =>
            {
                objOptions.UseSqlServer(strDataConnectionString, objSql =>
                {
                    objSql.MigrationsAssembly(typeof(CRMDbContext).Assembly.GetName().Name);
                });
            });

            colServices.AddScoped<IUserRepository, UserRepository>();

            colServices.AddScoped<ILogRepository,       SqlLogRepository>();
            colServices.AddScoped<IMailService,         SmtpMailService>();

            colServices.AddScoped<ICrmEntityRepository, CrmEntityRepository>();

            colServices.AddScoped<ICompanyRepository,       CompanyRepository>();
            colServices.AddScoped<IContactRepository,       ContactRepository>();
            colServices.AddScoped<IJobRepository,           JobRepository>();
            colServices.AddScoped<INoteRepository,          NoteRepository>();

            colServices.AddScoped<ITicketRepository, TicketRepository>();

            colServices.AddScoped<IToastService,        ToastService>();

            colServices.AddHttpClient<IGeocodingService, GoogleGeocodingService>();

            return colServices;
        }
    }
}
