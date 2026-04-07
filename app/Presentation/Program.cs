using Application;
using GroqApiLibrary;
using Infrastructure;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
string groqApiKey = builder.Configuration["GroqApiKey"]
    ?? throw new InvalidOperationException("GroqApiKey was not found.");

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddInfrastructure(connectionString, groqApiKey);
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

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("Client");

app.UseHttpsRedirection();

app.MapDeviceEndpoints();
app.MapUserEndpoints();

app.Run();


public partial class Program { }