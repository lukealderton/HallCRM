using CRM.Core;
using CRM.Core.Common.Abstraction;
using CRM.Core.Common.Configuration;
using CRM.Infrastructure;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Identity;
using CRM.Web;
using CRM.Web.Components;
using CRM.Web.Components.Account;
using CRM.Web.State;
using CRM.Web.Users.Abstraction;
using CRM.Web.Users.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

WebApplicationBuilder objBuilder = WebApplication.CreateBuilder(args);

// Add services to the container.
objBuilder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

objBuilder.Services.AddControllersWithViews();
objBuilder.Services.AddHttpContextAccessor();

// Add our Config object so it can be injected
objBuilder.Services.Configure<CRMConfiguration>(objBuilder.Configuration.GetSection("CRM"));

// Application services
objBuilder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
objBuilder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
objBuilder.Services.AddSingleton<IAppPathProvider, WebAppPathProvider>();

// Core and infrastructure services
objBuilder.Services.AddCore(objBuilder.Configuration);
objBuilder.Services.AddInfrastructure(objBuilder.Configuration);

// Identity
objBuilder.Services.AddCascadingAuthenticationState();
objBuilder.Services.AddScoped<IdentityRedirectManager>();
objBuilder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

objBuilder.Services.AddScoped<PageHeaderState>();
objBuilder.Services.AddScoped<ICurrentUserState, CurrentUserState>();

objBuilder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<CRMDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

objBuilder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();
objBuilder.Services.AddAuthorization();

objBuilder.Services.ConfigureApplicationCookie(objOptions =>
{
    objOptions.LoginPath = "/account/login";
    objOptions.AccessDeniedPath = "/account/denied";
});


WebApplication objApp = objBuilder.Build();

// Configure the HTTP request pipeline.
if (!objApp.Environment.IsDevelopment())
{
    objApp.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    objApp.UseHsts();
}

objApp.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
objApp.UseHttpsRedirection();
objApp.UseStaticFiles();

objApp.UseAuthentication();
objApp.UseAuthorization();

objApp.UseAntiforgery();

objApp.MapStaticAssets();
objApp.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
objApp.MapAdditionalIdentityEndpoints();

//await CreateTemporaryAdminUserAsync(objApp);

objApp.Run();

//static async Task CreateTemporaryAdminUserAsync(WebApplication objApp)
//{
//    using IServiceScope objScope = objApp.Services.CreateScope();

//    UserManager<ApplicationUser> objUserManager =
//        objScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//    RoleManager<IdentityRole> objRoleManager =
//        objScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//    String strAdminEmail = "";
//    String strAdminPassword = "";
//    String strAdminRole = "Admin";

//    IdentityRole? objAdminRole = await objRoleManager.FindByNameAsync(strAdminRole);

//    if (objAdminRole == null)
//    {
//        IdentityResult objRoleResult = await objRoleManager.CreateAsync(new IdentityRole
//        {
//            Name = strAdminRole
//        });

//        if (!objRoleResult.Succeeded)
//        {
//            throw new InvalidOperationException(
//                "Failed to create admin role: " +
//                String.Join(", ", objRoleResult.Errors.Select(objError => objError.Description)));
//        }
//    }

//    ApplicationUser? objExistingUser = await objUserManager.FindByEmailAsync(strAdminEmail);

//    if (objExistingUser == null)
//    {
//        Guid objUserId = Guid.NewGuid();
//        ApplicationUser objAdminUser = new()
//        {
//            Id = objUserId.ToString(),
//            UserName = strAdminEmail,
//            Email = strAdminEmail,
//            EmailConfirmed = true,
//            DomainUserId = objUserId,
//            Forename = strForename,
//            Surname = strSurname,
//            Enabled = true
//        };

//        IdentityResult objUserResult = await objUserManager.CreateAsync(objAdminUser, strAdminPassword);

//        if (!objUserResult.Succeeded)
//        {
//            throw new InvalidOperationException(
//                "Failed to create admin user: " +
//                String.Join(", ", objUserResult.Errors.Select(objError => objError.Description)));
//        }

//        objExistingUser = objAdminUser;
//    }

//    Boolean blnIsInRole = await objUserManager.IsInRoleAsync(objExistingUser, strAdminRole);

//    if (!blnIsInRole)
//    {
//        IdentityResult objAddRoleResult = await objUserManager.AddToRoleAsync(objExistingUser, strAdminRole);

//        if (!objAddRoleResult.Succeeded)
//        {
//            throw new InvalidOperationException(
//                "Failed to add admin role: " +
//                String.Join(", ", objAddRoleResult.Errors.Select(objError => objError.Description)));
//        }
//    }
//}