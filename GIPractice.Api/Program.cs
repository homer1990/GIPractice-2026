using AutoMapper;
using GIPractice.Api.Infrastructure;
using GIPractice.Api.Services;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("GIPractice.Infrastructure")));

// Media storage options + service
builder.Services.Configure<MediaStorageOptions>(
    builder.Configuration.GetSection("MediaStorage"));

builder.Services.AddSingleton<IMediaStorageService, FileSystemMediaStorageService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(opts => { opts.JsonSerializerOptions.WriteIndented = true; });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Static file serving so /media/... works
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
