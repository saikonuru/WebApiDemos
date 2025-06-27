// Pseudocode plan:
// 1. Ensure Serilog is configured before the host is built.
// 2. Use Serilog for logging by calling UseSerilog() on the host builder.
// 3. Make sure the log file path is valid and writable (use "logs/log.txt" instead of "/logs/log.txt" for cross-platform compatibility).
// 4. Remove commented-out or conflicting logging code.
// 5. Ensure app.UseSerilogRequestLogging() is called to log HTTP requests.

using CityInfo.API.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.OpenApi.Models;
using System;
using Serilog;
using Serilog.AspNetCore;
using CityInfo.API.Services;

Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .WriteTo.Console()
                 .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                 .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers(options =>
{
    options.OutputFormatters.Add(new StringOutputFormatter());
})
 .AddNewtonsoftJson()
 .AddXmlDataContractSerializerFormatters();

builder.Services.AddScoped<IValidator<Person>, PersonValidator>();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions.Add("additional Info", "addition info demo");
        ctx.ProblemDetails.Extensions.Add("Server", Environment.MachineName);
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CityInfo API", Version = "v1" });
});

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddTransient<LocalMailService>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(); // <-- Add this line to log HTTP requests

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapSwagger();

app.Run();