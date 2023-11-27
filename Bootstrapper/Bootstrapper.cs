using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartHome.Classes.Database;
using SmartHome.Classes.Deconz;
using SmartHome.Classes.ModbusWrapper;
using SmartHome.Classes.ModbusWrapper.Config;
using SmartHome.Classes.Shelly;
using SmartHome.Classes.SmartHome;
using SmartHome.Classes.SmartHome.Interfaces;

namespace Bootstrapper
{
    public static class Bootstrapper
    {
        //todo: fehlende Systeme zufügen. 
        public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration config)

        {
            
            services.Configure<ModbusOptions>(config.GetSection(ModbusOptions.ConfigSection));
            services.Configure<LanguageOptions>(config.GetSection(LanguageOptions.ConfigSection));
            services.Configure<DatabaseOptions>(config.GetSection(DatabaseOptions.ConfigSection));
            return services;

        }
#pragma warning disable IDE0060 // Nicht verwendete Parameter entfernen
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
#pragma warning restore IDE0060 // Nicht verwendete Parameter entfernen

        {
            services.AddSingleton<IDatabaseWrapper, DatabaseWrapper>();
            services.AddSingleton<IDeyeModbus, DeyeModbus>();
            services.AddSingleton<ISmartHomeWrapper, SmartHomeWrapper>();
            services.AddSingleton<IShellyWorker, ShellyWorker>();
            services.AddSingleton<ISmartHomeHelper, SmartHomeHelper>();
            services.AddSingleton<ISmartHomeTimerWorker, SmartHomeTimerWorker>();
            services.AddSingleton<IDeconzWrapper, DeconzWrapper>();
            return services;

        }

    }
}