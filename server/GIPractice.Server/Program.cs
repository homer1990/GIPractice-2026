using GIPractice.Server;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/version", () => Results.Ok(new
{
    application = "GIPractice",
    architecture = "v3-simple-clinical-core"
}));

app.Run();

namespace GIPractice.Server;
