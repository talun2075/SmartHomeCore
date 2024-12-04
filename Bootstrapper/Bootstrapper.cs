using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartHome.Classes.Database;
using SmartHome.Classes.Deconz;

namespace Bootstrapper
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration config)

        {
            services.Configure<LanguageOptions>(config.GetSection(LanguageOptions.ConfigSection));
            services.Configure<DatabaseOptions>(config.GetSection(DatabaseOptions.ConfigSection));
            return services;

        }
#pragma warning disable IDE0060 // Nicht verwendete Parameter entfernen
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
#pragma warning restore IDE0060 // Nicht verwendete Parameter entfernen

        {
            //todo: add aurora
            services.AddSingleton<IDatabaseWrapper, DatabaseWrapper>();
            services.AddSingleton<IDeconzWrapper, DeconzWrapper>();
            return services;

        }

    }
}