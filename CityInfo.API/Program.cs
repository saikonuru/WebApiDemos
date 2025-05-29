using Microsoft.OpenApi.Models; // Add this using directive for Swagger support
using Swashbuckle.AspNetCore.Swagger; // Ensure this using directive is added for Swagger middleware
using System.IO; // Add this using directive for Path.Combine

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CityInfo API", Version = "v1" });
    
});

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