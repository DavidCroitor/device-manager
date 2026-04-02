using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.SqlClient;
using Domain.Interfaces;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence;

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
            return services;
        }
    }
}