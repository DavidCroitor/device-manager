using Microsoft.Extensions.DependencyInjection;
using Domain.Interfaces;
using Infrastructure.Persistence.Repositories;
using Application.Interfaces;
using Infrastructure.Authentication;
using GroqApiLibrary;
using Infrastructure.AI;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            string connectionString,
            string groqApiKey)
        {

            services.AddSingleton<IGroqApiClient>(sp => new GroqApiClient(groqApiKey));
            services.AddScoped<IUserRepository>(sp => new UserRepository(connectionString));
            services.AddScoped<IDeviceRepository>(sp => new DeviceRepository(connectionString));
            services.AddScoped<IDeviceDescriptionGenerator, AiDeviceDescriptorGenerator>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            return services;
        }
    }
}