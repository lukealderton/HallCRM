using CRM.Core.Companies.Abstractions;
using CRM.Core.Companies.Services;
using CRM.Core.Contacts.Abstractions;
using CRM.Core.Contacts.Services;
using CRM.Core.Entities.Abstractions;
using CRM.Core.Entities.Services;
using CRM.Core.Logging.Abstraction;
using CRM.Core.Logging.Services;
using CRM.Core.Notes.Abstractions;
using CRM.Core.Notes.Services;
using CRM.Core.Jobs.Abstractions;
using CRM.Core.Jobs.Services;
using CRM.Core.Tickets.Abstractions;
using CRM.Core.Tickets.Services;
using CRM.Core.Users.Abstraction.Services;
using CRM.Core.Users.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(
            this IServiceCollection colServices,
            IConfiguration objConfiguration)
        {
            colServices.AddScoped<IUserService, UserService>();
            colServices.AddScoped<ILogService,  LogService>();
            
            colServices.AddScoped<ICrmEntityService, CrmEntityService>();

            colServices.AddScoped<ICompanyService,      CompanyService>();
            colServices.AddScoped<IContactService,      ContactService>();
            colServices.AddScoped<IJobService,          JobService>();
            colServices.AddScoped<INoteService,         NoteService>();

            colServices.AddScoped<ITicketService, TicketService>();

            return colServices;
        }
    }
}
