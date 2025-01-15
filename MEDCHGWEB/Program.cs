using Blazored.LocalStorage;
using Blazored.SessionStorage;
using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository;
using MEDCHGClassLibrary.Repository.BillingRepository;
using MEDCHGClassLibrary.Repository.BuildingRepository;
using MEDCHGClassLibrary.Repository.ChargeCodes;
using MEDCHGClassLibrary.Repository.DiagnosticCodes;
using MEDCHGClassLibrary.Repository.FloorRepository;
using MEDCHGClassLibrary.Repository.Hospital;
using MEDCHGClassLibrary.Repository.Implementation;
using MEDCHGClassLibrary.Repository.PatientRepository;
using MEDCHGClassLibrary.Repository.PatientVisitHistory;
using MEDCHGClassLibrary.Repository.ProviderRepository;
using MEDCHGClassLibrary.Repository.RoomRepository;
using MEDCHGClassLibrary.Repository.User;
using MEDCHGClassLibrary.Services;
using MEDCHGClassLibrary.Utilities.Enum;
using MEDCHGWEB.Components;
using MEDCHGWEB.Components.Account;
using MEDCHGWEB.Middleware;
using MEDCHGWEB.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Net;
using static MEDCHGWEB.Components.Layout.NavMenu;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "YourAntiforgeryCookie";
    options.HeaderName = "X-XSRF-TOKEN";
});

// Add JWT configuration
RsaSecurityKey key = new(TokenService.LoadRsaPrivateKey());
AppSettingJwt AppSettingJwt = JsonConvert.DeserializeObject<AppSettingJwt>(File.ReadAllText(Directory.GetCurrentDirectory() + "/appsettings.json").ToString());

// JWT authentication config.
builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Use JWT for challenges in APIs
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Error";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            var user = context.HttpContext.User;
            if (user != null && user.IsInRole("Superadmin"))
            {
                context.RedirectUri = "/Account/LoginAdmin?ReturnUrl=%2F";
            }
            else
            {
                context.RedirectUri = "/Account/Login?ReturnUrl=%2F";
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            context.Response.Redirect("/Account/Login?ReturnUrl=%2F");
            return Task.CompletedTask;
        }
    };

}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = AppSettingJwt.Jwt.Issuer,
        ValidAudience = AppSettingJwt.Jwt.Audience,
        IssuerSigningKey = key,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true,
        ValidAlgorithms = [SecurityAlgorithms.RsaSha512, SecurityAlgorithms.RsaSha512Signature],
        RefreshBeforeValidation = false,
        ClockSkew = TimeSpan.Zero,
    };

    o.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            //context.Response.Redirect("/Error?message=Authentication failed");
            context.Response.Redirect("/Account/Login?ReturnUrl=%2F");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/Account/Login");
                context.HandleResponse(); // Suppress the default unauthorized response
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization(options =>
{
    // Create a policy for JWT authentication
    options.AddPolicy("ApiPolicy", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
    });
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddControllers();
builder.Services.AddScoped<NavMenuState>();
builder.Services.AddScoped<MedChgContext>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBuildingRepository, BuildingsRepository>();
builder.Services.AddScoped<IRoomRepository, RoomsRepository>();
builder.Services.AddScoped<IFloorRepository, FloorsRepository>();
builder.Services.AddScoped<IUsersHospital, UserRepository>();
builder.Services.AddScoped<IHospitalRepository, HospitalsRepository>();
builder.Services.AddScoped<IUsersChargeCode, UserRepository>();
builder.Services.AddScoped<IUsersDiagnosticCode, UserRepository>();
builder.Services.AddSingleton<AppState>();
builder.Services.AddSingleton<StateService>();
builder.Services.AddSingleton<ResponseMessage>();
builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
builder.Services.AddScoped<IBillingRepository, BillingsRepository>();
builder.Services.AddScoped<IPatientRepository, PatientsRepository>();
builder.Services.AddScoped<IDiagnosticCodeRepository, DiagnosticCodeRepository>();
builder.Services.AddScoped<IChargeCodeRepository, ChargeCodeRepository>();
builder.Services.AddScoped<IPatientVisitHistoryRespository, PatientVisitHistoryRepository>();
builder.Services.AddScoped<IRepository<Hospital>, Repository<Hospital>>();
builder.Services.AddScoped<IRepository<Building>, Repository<Building>>();
builder.Services.AddScoped<IRepository<Floor>, Repository<Floor>>();
builder.Services.AddScoped<IRepository<Room>, Repository<Room>>();
builder.Services.AddScoped<IRepository<Patient>, Repository<Patient>>();
builder.Services.AddScoped<IRepository<PatientTempCode>, Repository<PatientTempCode>>();
builder.Services.AddScoped<IRepository<Provider>, Repository<Provider>>();

builder.Services.AddScoped(http => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration.GetSection("BaseAddress").Value!)
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies();
builder.Services.AddScoped<SignalRService>();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var connectionString = builder.Configuration.GetConnectionString("PostgreConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<MedChgContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MedChgContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Other Identity options like Password requirements, etc.
    options.Password.RequireDigit = false; // Remove requirement for digits
    options.Password.RequireLowercase = false; // Remove requirement for lowercase letters
    options.Password.RequireUppercase = false; // Remove requirement for uppercase letters
    options.Password.RequireNonAlphanumeric = false; // Remove requirement for non-alphanumeric characters
    options.Password.RequiredLength = 6; // You might still want to set a minimum length
});
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSignalR();
builder.Services.AddSignalRCore();

// Add code to generate roles here
using (var serviceScope = builder.Services?.BuildServiceProvider().CreateScope())
{
    var roleManager = serviceScope!.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var roleName in UserRoles.GetAllRoles())
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

builder.Services?.AddDistributedMemoryCache();
var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

// Make sure to place UseSession before UseRouting
//app.UseSession();
app.UseRouting();

// Place authentication middleware here
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery(); // This can be placed after authentication/authorization if needed

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/NotificationHub");
});

app.Run();
