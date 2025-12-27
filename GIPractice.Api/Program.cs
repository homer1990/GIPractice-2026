using GIPractice.Api.Scheduling;
using GIPractice.Api.Patients;
using GIPractice.Contracts;
using GIPractice.Contracts.Scheduling;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Patients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SchedulerOptions>(builder.Configuration.GetSection("Scheduler"));

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new StrongIntIdJsonConverterFactory());
    });

builder.Services.AddSingleton<ISchedulingStore, InMemorySchedulingStore>();
builder.Services.AddScoped<ISchedulingService, SchedulingService>();

builder.Services.AddSingleton<IPatientsStore, InMemoryPatientsStore>();
builder.Services.AddScoped<IPatientsService, PatientsService>();

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.Converters.Add(new StrongIntIdJsonConverterFactory());
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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
