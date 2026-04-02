using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var clientURL = "http://localhost:4200";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Client", 
                      policy =>
                        {
                            policy.WithOrigins(clientURL)
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                        });
});

builder.Services.AddInfrastructure(connectionString);
builder.Services.AddApplication();

DatabaseInitializer.Initialize(connectionString);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Client");

app.UseHttpsRedirection();

app.MapDeviceEndpoints();
app.MapUserEndpoints();

app.Run();


public partial class Program { }