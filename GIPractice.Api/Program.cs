using GIPractice.Api.Biopsies;
using GIPractice.Api.Patients;
using GIPractice.Api.Scheduling;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Patients;
using GIPractice.Contracts.Scheduling;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

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
builder.Services.AddScoped<GIPractice.Contracts.Biopsies.IBiopsiesService, GIPractice.Api.Biopsies.BiopsiesService>();

builder.Services.AddSingleton<GIPractice.Api.Pathology.IPathologyStore, GIPractice.Api.Pathology.InMemoryPathologyStore>();
builder.Services.AddScoped<GIPractice.Contracts.Pathology.IPathologyService, GIPractice.Api.Pathology.PathologyService>();

builder.Services.AddSingleton<GIPractice.Api.Infai.IInfaiStore, GIPractice.Api.Infai.InMemoryInfaiStore>();
builder.Services.AddScoped<GIPractice.Contracts.Infai.IInfaiService, GIPractice.Api.Infai.InfaiService>();

builder.Services.AddSingleton<GIPractice.Api.Settings.ISettingsStore, GIPractice.Api.Settings.InMemorySettingsStore>();
builder.Services.AddScoped<GIPractice.Contracts.Settings.ISettingsService, GIPractice.Api.Settings.SettingsService>();

builder.Services.AddSingleton<GIPractice.Api.Localization.ILocalizationStore, GIPractice.Api.Localization.InMemoryLocalizationStore>();
builder.Services.AddScoped<GIPractice.Contracts.Localization.ILocalizationService, GIPractice.Api.Localization.LocalizationService>();

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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
