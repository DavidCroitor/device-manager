using Microsoft.Extensions.DependencyInjection;
using Domain.Interfaces;
using Infrastructure.Persistence.Repositories;
using Application.Interfaces;
using Infrastructure.Authentication;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            string connectionString)
        {

            services.AddScoped<IUserRepository>(sp => new UserRepository(connectionString));
            services.AddScoped<IDeviceRepository>(sp => new DeviceRepository(connectionString));
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            return services;
        }
    }
}