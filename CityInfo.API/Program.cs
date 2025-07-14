using System.Reflection;
using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using CityInfo.API;
using CityInfo.API.DbContexts;
using CityInfo.API.Models;
using CityInfo.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

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
builder.Services.AddSingleton<CitiesDataStore>();
builder.Services.AddDbContext<CityInfoContext>(dbContextOptions => dbContextOptions.UseSqlite(
    builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]
    ));

builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer(options =>
        {

            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Authentication:Issuer"],
                ValidAudience = builder.Configuration["Authentication:Audience"],
                //IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecurityForKey"]))
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Authentication:SecurityForKey"]))
            };
        });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFromLondon", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "London");
    });
});


builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.ReportApiVersions = true;
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new ApiVersion(1, 0);
}).AddMvc()
.AddApiExplorer(
    setupAction =>
    {
        setupAction.SubstituteApiVersionInUrl = true;
    }
);

var apiVersionDescriptionProvider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

if (apiVersionDescriptionProvider != null)
{
    builder.Services.AddSwaggerGen(setupAction =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            setupAction.SwaggerDoc(

                $"{description.GroupName}",
                new()
                {
                    Title = "City Info API",
                    Version = description.ApiVersion.ToString(),
                    Description = "The Cities API "
                }
            );
        }

        setupAction.SwaggerDoc("v1", new OpenApiInfo { Title = "CityInfo API", Version = "v1" });
        setupAction.SwaggerDoc("v2", new OpenApiInfo { Title = "CityInfo API", Version = "v2" });

        var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

        setupAction.IncludeXmlComments(xmlCommentsFullPath);

        setupAction.AddSecurityDefinition("CityInfoApiBearerAuth", new()
        {
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            Description = "Input a valid token to access to this API"

        });

        setupAction.AddSecurityRequirement(new()
        { 
            {
                new ()
                {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = "CityInfoApiBearerAuth" }
                    },
                    new List<string>()
                }
        });

    });
}

builder.Services.AddEndpointsApiExplorer();

//http://localhost:5096/swagger/index.html


builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions.Add("additional Info", "addition info demo");
        ctx.ProblemDetails.Extensions.Add("Server", Environment.MachineName);
    };
});

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(setupAction =>
    {
        var desiriptions = app.DescribeApiVersions();

        foreach (var description in desiriptions)
        {
            setupAction.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant()
             );
        }
    });
}

app.UseSerilogRequestLogging(); // <-- Add this line to log HTTP requests

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapSwagger();

app.Run();