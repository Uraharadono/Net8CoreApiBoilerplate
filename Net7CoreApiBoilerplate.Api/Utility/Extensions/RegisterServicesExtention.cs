using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Net7CoreApiBoilerplate.DbContext.Infrastructure;
using Net7CoreApiBoilerplate.Infrastructure.DbUtility;
using Net7CoreApiBoilerplate.Infrastructure.Services;
using Net7CoreApiBoilerplate.Infrastructure.Settings;
using Net7CoreApiBoilerplate.Services.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Net7CoreApiBoilerplate.Api.Utility.Extensions
{
    // Note to self: This can be split in 2 different methods and files (other being RegisterAppConfigurations)
    public static class RegisterServicesExtention
    {
        public static void RegisterUtilityServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Unit of work
            var deronContext = Net7BoilerplateContext.Create(configuration.GetConnectionString("BloggingDb"));

            // Singleton - as we want to reuse DbContext for transactions, and not open it every time we need it
            services.AddSingleton<IUnitOfWork>(x => new UnitOfWork(deronContext));
        }

        public static void AutoRegisterServices(this IServiceCollection services)
        {
            // First we need to register ISettings, IEmailSettings etc. 
            // because if we don't, registration of services below is going to fail
            // Since for example: IEmailSettings won't be resolved to ISettings
            var appSettingsAssemblies = GetAppSettingsInjectableAssemblies();
            foreach (var assembly in appSettingsAssemblies)
            {
                RegisterAppSettingsFromAssembly(services, assembly);
            }

            // After that we register services e.g. data fetching, sales orders service etc.
            var servicesAssemblies = GetServicesInjectableAssemblies();
            foreach (var assembly in servicesAssemblies)
            {
                RegisterDeronServicesFromAssembly(services, assembly);
            }
        }

        private static IEnumerable<Assembly> GetAppSettingsInjectableAssemblies()
        {
            yield return Assembly.GetAssembly(typeof(IAppSettings)); // Infrastructure
        }

        private static IEnumerable<Assembly> GetServicesInjectableAssemblies()
        {
            yield return Assembly.GetAssembly(typeof(EmailService)); // Services
        }

        private static bool IsInjectable(Type t)
        {
            var interfaces = t.GetInterfaces();
            var injectable = interfaces.Any(i =>
                i.IsAssignableFrom(typeof(IService))
            );
            return injectable;
        }

        private static void RegisterAppSettingsFromAssembly(IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.Name.EndsWith("Settings"))
                {
                    services.AddSingleton(type, typeof(AppSettings));
                }
            }
        }

        private static void RegisterDeronServicesFromAssembly(IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IService).IsAssignableFrom(type))
                {
                    var childTypes =
                        type.Assembly
                            .GetTypes()
                            .Where(t => t.IsClass && t.GetInterface(type.Name) != null);

                    foreach (var childType in childTypes)
                    {
                        services.AddScoped(type, childType);
                    }
                }
            }
        }
    }
}
