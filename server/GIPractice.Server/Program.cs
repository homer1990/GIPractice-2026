using GIPractice.Server.Api;
using GIPractice.Server.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryPracticeReadStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/version", () => Results.Ok(new
{
    application = "GIPractice",
    architecture = "v3-simple-clinical-core"
}));

app.MapPracticeReadApi();

app.Run();
