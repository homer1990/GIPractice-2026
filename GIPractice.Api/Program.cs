using System.Text;
using GIPractice.Api.Auth;
using GIPractice.Api.Biopsies;
using GIPractice.Api.Endoscopies;
using GIPractice.Api.Pathologists;
using GIPractice.Api.Pathology;
using GIPractice.Api.Patients;
using GIPractice.Api.Scheduling;
using GIPractice.Api.Users;
using GIPractice.Contracts.Auth;
using GIPractice.Contracts.Biopsies;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Endoscopies;
using GIPractice.Contracts.Encounters;
using GIPractice.Contracts.Infai;
using GIPractice.Contracts.Localization;
using GIPractice.Contracts.Pathologists;
using GIPractice.Contracts.Pathology;
using GIPractice.Contracts.Patients;
using GIPractice.Contracts.Scheduling;
using GIPractice.Contracts.Settings;
using GIPractice.Contracts.Users;
using GIPractice.Core.Entities.Identity;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

namespace GIPractice.Api;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Options
        builder.Services.Configure<SchedulerOptions>(builder.Configuration.GetSection("Scheduler"));
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
        builder.Services.Configure<BootstrapApiAdminOptions>(builder.Configuration.GetSection("BootstrapAdmin"));

        // Controllers + JSON
        builder.Services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new StrongIntIdJsonConverterFactory());
                o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

        // Needed for Controller-based OpenAPI generation.
        builder.Services.AddEndpointsApiExplorer();

        // Db
        builder.Services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

        // Auth prerequisites
        builder.Services.AddHttpContextAccessor();

        builder.Services
            .AddIdentityCore<ApplicationUser>(opt =>
            {
                // Dev/test friendly (bootstrap admin uses "admin"). Tighten later.
                opt.Password.RequiredLength = 4;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        if (string.IsNullOrWhiteSpace(jwt.Key))
            throw new InvalidOperationException("Jwt:Key is required (set it in appsettings.Development.json for dev and secrets/env for prod)." );

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2)
                };
            });

        builder.Services.AddAuthorization(o =>
        {
            // Default locked-down API; explicitly [AllowAnonymous] where needed.
            o.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddHostedService<IdentityBootstrapperHostedService>();

        // Users
        builder.Services.AddScoped<IUsersStore, EfUsersStore>();
        builder.Services.AddScoped<IUsersService, UsersService>();

        // Scheduling
        builder.Services.AddSingleton<ISchedulingStore, InMemorySchedulingStore>();
        builder.Services.AddScoped<ISchedulingService, SchedulingService>();

        // Patients
        builder.Services.AddScoped<IPatientsStore, EfPatientsStore>();
        builder.Services.AddScoped<IPatientsService, PatientsService>();

        // Encounters
        builder.Services.AddSingleton<GIPractice.Api.Encounters.IEncountersStore, GIPractice.Api.Encounters.InMemoryEncountersStore>();
        builder.Services.AddScoped<IEncountersService, GIPractice.Api.Encounters.EncountersService>();

        // Endoscopies
        builder.Services.AddScoped<IEndoscopiesStore, EfEndoscopiesStore>();
        builder.Services.AddScoped<IEndoscopiesService, EndoscopiesService>();

        // Biopsies
        builder.Services.AddScoped<IBiopsiesStore, EfBiopsiesStore>();
        builder.Services.AddScoped<IBiopsiesService, BiopsiesService>();

        // Infai
        builder.Services.AddSingleton<GIPractice.Api.Infai.IInfaiStore, GIPractice.Api.Infai.InMemoryInfaiStore>();
        builder.Services.AddScoped<IInfaiService, GIPractice.Api.Infai.InfaiService>();

        // Settings
        builder.Services.AddSingleton<GIPractice.Api.Settings.ISettingsStore, GIPractice.Api.Settings.InMemorySettingsStore>();
        builder.Services.AddScoped<ISettingsService, GIPractice.Api.Settings.SettingsService>();

        // Localization
        builder.Services.AddSingleton<GIPractice.Api.Localization.ILocalizationStore, GIPractice.Api.Localization.InMemoryLocalizationStore>();
        builder.Services.AddScoped<ILocalizationService, GIPractice.Api.Localization.LocalizationService>();

        // Pathology (Option 1): parcels generated from endoscopies (EF-backed)
        builder.Services.AddScoped<IPathologyReportsStore, EfPathologyReportsStore>();
        builder.Services.AddScoped<IPathologyReportsService, PathologyReportsService>();
        builder.Services.AddScoped<IPathologyParcelsStore, EfPathologyParcelsStore>();
        builder.Services.AddScoped<IPathologyParcelsService, PathologyParcelsService>();

        // Pathologists
        builder.Services.AddScoped<IPathologistsStore, EfPathologistsStore>();
        builder.Services.AddScoped<IPathologistsService, PathologistsService>();

        // JSON for minimal APIs (kept; harmless)
        builder.Services.ConfigureHttpJsonOptions(o =>
        {
            o.SerializerOptions.Converters.Add(new StrongIntIdJsonConverterFactory());
            o.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

        // OpenAPI
        builder.Services.AddOpenApi(options =>
        {
            options.AddSchemaTransformer<GIPractice.Api.Common.StrongIntIdSchemaTransformer>();
        });

        var app = builder.Build();

        app.UseMiddleware<GIPractice.Api.Common.ExceptionResultMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi().AllowAnonymous();
            app.MapScalarApiReference().AllowAnonymous();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}

public class ApiEntryPoint { }
