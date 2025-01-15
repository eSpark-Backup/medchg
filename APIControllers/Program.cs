using APIControllers.Configuration;
using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.Repository.BuildingRepository;
using MEDCHGClassLibrary.Repository.ChargeCodes;
using MEDCHGClassLibrary.Repository.DiagnosticCodes;
using MEDCHGClassLibrary.Repository.FloorRepository;
using MEDCHGClassLibrary.Repository.Hospital;
using MEDCHGClassLibrary.Repository.Implementation;
using MEDCHGClassLibrary.Repository.PatientRepository;
using MEDCHGClassLibrary.Repository.PatientVisitHistory;
using MEDCHGClassLibrary.Repository.RoomRepository;
using MEDCHGClassLibrary.Repository.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace APIControllers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Bind appsettings to a POCO class
            builder.Services.Configure<AppSettings>(builder.Configuration);

            // Register the DbContext and other services
            builder.Services.AddDbContext<MedChgContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreConnection")));

            // Add JWT Authentication
            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            //    options.RequireHttpsMetadata = false;
            //    options.SaveToken = true;
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidIssuer = jwtSettings.Issuer,
            //        ValidateAudience = true,
            //        ValidAudience = jwtSettings.Audience,
            //        ValidateLifetime = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            //        ClockSkew = TimeSpan.Zero // Optional: set to zero to remove time skew
            //    };
            //});
            // Add services to the container.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IBuildingRepository, BuildingsRepository>();
            builder.Services.AddScoped<IRoomRepository, RoomsRepository>();
            builder.Services.AddScoped<IFloorRepository, FloorsRepository>();
            builder.Services.AddScoped<IUsersHospital, UserRepository>();
            builder.Services.AddScoped<IHospitalRepository, HospitalsRepository>();
            builder.Services.AddScoped<IUsersChargeCode, UserRepository>();
            builder.Services.AddScoped<IUsersDiagnosticCode, UserRepository>();
            builder.Services.AddScoped<IPatientRepository, PatientsRepository>();
            builder.Services.AddScoped<IDiagnosticCodeRepository, DiagnosticCodeRepository>();
            builder.Services.AddScoped<IChargeCodeRepository, ChargeCodeRepository>();
            builder.Services.AddScoped<IPatientVisitHistoryRespository, PatientVisitHistoryRepository>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MEDCHG API", Version = "v1" });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
