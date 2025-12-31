using GIPractice.Api.Biopsies;
using GIPractice.Api.Pathologists;
using GIPractice.Api.Pathology;
using GIPractice.Api.Patients;
using GIPractice.Api.Scheduling;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Pathologists;
using GIPractice.Contracts.Pathology;
using GIPractice.Contracts.Patients;
using GIPractice.Contracts.Scheduling;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace GIPractice.Api;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<SchedulerOptions>(builder.Configuration.GetSection("Scheduler"));

        builder.Services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new StrongIntIdJsonConverterFactory());
            });

        // Needed for Controller-based OpenAPI generation.
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSingleton<ISchedulingStore, InMemorySchedulingStore>();
        builder.Services.AddScoped<ISchedulingService, SchedulingService>();

        //builder.Services.AddSingleton<IPatientsStore, InMemoryPatientsStore>();
        builder.Services.AddScoped<IPatientsStore, EfPatientsStore>();
        builder.Services.AddScoped<IPatientsService, PatientsService>();

        builder.Services.AddSingleton<GIPractice.Api.Encounters.IEncountersStore, GIPractice.Api.Encounters.InMemoryEncountersStore>();
        builder.Services.AddScoped<GIPractice.Contracts.Encounters.IEncountersService, GIPractice.Api.Encounters.EncountersService>();

        //builder.Services.AddSingleton<GIPractice.Api.Endoscopies.IEndoscopiesStore, GIPractice.Api.Endoscopies.InMemoryEndoscopiesStore>();
        builder.Services.AddScoped<GIPractice.Api.Endoscopies.IEndoscopiesStore, GIPractice.Api.Endoscopies.EfEndoscopiesStore>();
        builder.Services.AddScoped<GIPractice.Contracts.Endoscopies.IEndoscopiesService, GIPractice.Api.Endoscopies.EndoscopiesService>();

        //builder.Services.AddSingleton<GIPractice.Api.Biopsies.IBiopsiesStore, GIPractice.Api.Biopsies.InMemoryBiopsiesStore>();
        builder.Services.AddScoped<IBiopsiesStore, EfBiopsiesStore>();
        builder.Services.AddScoped<GIPractice.Contracts.Biopsies.IBiopsiesService, BiopsiesService>();

        builder.Services.AddSingleton<GIPractice.Api.Infai.IInfaiStore, GIPractice.Api.Infai.InMemoryInfaiStore>();
        builder.Services.AddScoped<GIPractice.Contracts.Infai.IInfaiService, GIPractice.Api.Infai.InfaiService>();

        builder.Services.AddSingleton<GIPractice.Api.Settings.ISettingsStore, GIPractice.Api.Settings.InMemorySettingsStore>();
        builder.Services.AddScoped<GIPractice.Contracts.Settings.ISettingsService, GIPractice.Api.Settings.SettingsService>();

        builder.Services.AddSingleton<GIPractice.Api.Localization.ILocalizationStore, GIPractice.Api.Localization.InMemoryLocalizationStore>();
        builder.Services.AddScoped<GIPractice.Contracts.Localization.ILocalizationService, GIPractice.Api.Localization.LocalizationService>();

        // Pathologists (you already have store/service, keep as singleton)
        builder.Services.AddSingleton<IPathologistsStore, InMemoryPathologistsStore>();
        builder.Services.AddSingleton<IPathologistsService, PathologistsService>();

        // Pathology shared repo
        builder.Services.AddSingleton<InMemoryPathologyRepository>();

        // Pathology reports
        builder.Services.AddSingleton<IPathologyReportsStore, InMemoryPathologyReportsStore>();
        builder.Services.AddSingleton<IPathologyReportsService, PathologyReportsService>();

        // Pathology parcels
        builder.Services.AddSingleton<IPathologyParcelsStore, InMemoryPathologyParcelsStore>();
        builder.Services.AddSingleton<IPathologyParcelsService, PathologyParcelsService>();

        builder.Services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

        builder.Services.ConfigureHttpJsonOptions(o =>
        {
            o.SerializerOptions.Converters.Add(new StrongIntIdJsonConverterFactory());
        });

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi(options =>
        {
            options.AddSchemaTransformer<GIPractice.Api.Common.StrongIntIdSchemaTransformer>();
        });


        var app = builder.Build();

        app.UseMiddleware<GIPractice.Api.Common.ExceptionResultMiddleware>(); //Exceptions Middleware

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}
public class ApiEntryPoint { }