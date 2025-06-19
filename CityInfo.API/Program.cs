using Microsoft.AspNetCore.StaticFiles;
using Microsoft.OpenApi.Models; // Add this using directive for Swagger support
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddControllers();
//builder.Services.AddControllers(options => { options.ReturnHttpNotAcceptable = true; });

builder.Services.AddControllers(options=>
{
    options.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters(); // Add XML support

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions.Add("additional Info", "addition info demo");
        ctx.ProblemDetails.Extensions.Add("Server", Environment.MachineName);
    };
});

//Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
 builder.Services.AddSwaggerGen(c =>
 {
     c.SwaggerDoc("v1", new OpenApiInfo { Title = "CityInfo API", Version = "v1" });

 });

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Ensure Swagger middleware is added
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapSwagger();

app.Run();