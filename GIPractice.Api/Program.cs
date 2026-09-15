using System.Text;
using GIPractice.Api.Hosting;
using GIPractice.Infrastructure;
using GIPractice.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseWindowsService(options => options.ServiceName = "GIPractice API")
    .UseSystemd();

var databaseProvider = builder.Configuration["Database:Provider"] ?? "SqlServer";
var connectionStringName = builder.Configuration["Database:ConnectionStringName"] ?? "DefaultConnection";
var connectionString = builder.Configuration.GetConnectionString(connectionStringName)
    ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    DatabaseProviderConfigurator.Configure(options, databaseProvider, connectionString));

var jwtSigningKey = builder.Configuration["Jwt:SigningKey"];
var jwtConfigured = !string.IsNullOrWhiteSpace(jwtSigningKey);

if (jwtConfigured)
{
    var issuer = builder.Configuration["Jwt:Issuer"] ?? "GIPractice.Api";
    var audience = builder.Configuration["Jwt:Audience"] ?? "GIPractice.Client";

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey!)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });
}
else if (!builder.Environment.IsDevelopment())
{
    throw new InvalidOperationException(
        "Jwt:SigningKey is required outside Development. Supply it through environment/secret configuration; never commit it to source.");
}

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problem = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://www.rfc-editor.org/rfc/rfc9110#name-400-bad-request",
                Title = "One or more validation errors occurred.",
                Instance = context.HttpContext.Request.Path
            };
            problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            return new BadRequestObjectResult(problem);
        };
    })
    .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "GIPractice API", Version = "v1" });

    if (!jwtConfigured)
        return;

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header. Example: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// Schema changes are deliberately explicit. A daemon must never mutate a medical
// database merely because the service restarted after a deployment.
if (await DatabaseCli.TryRunAsync(args, app.Services))
    return;

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

if (jwtConfigured)
    app.UseAuthentication();

app.UseAuthorization();

app.MapGet("/health/live", () => Results.Ok(new { status = "live" }))
    .AllowAnonymous();

app.MapGet("/health/ready", async (AppDbContext db, CancellationToken cancellationToken) =>
{
    var canConnect = await db.Database.CanConnectAsync(cancellationToken);
    return canConnect
        ? Results.Ok(new { status = "ready", provider = databaseProvider })
        : Results.Problem("Database connection failed.", statusCode: StatusCodes.Status503ServiceUnavailable);
}).AllowAnonymous();

app.MapControllers();
await app.RunAsync();
