using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services.TryAddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddUnitOfWork<T>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where T : Microsoft.EntityFrameworkCore.DbContext
        {
            services.TryAddScoped<IUnitOfWork<T>, UnitOfWork<T>>();
            return services;
        }

        // This is not working anymore, see comments inside
        public static void RegisterUtilityServices_depricated(this IServiceCollection services, IConfiguration configuration)
        {
            // Reason that registering the UnitOfWork won't work like shown in code below is that 
            // no matter which type of registration is picked up it will yield issues in long run

            // Singleton will produce following error if you are calling more than 2 methods at the same time.
            // Most notably I had this happen on sales order details screen, where I called 7-8 api endpoints on page load.
            // An exception occurred while iterating over the results of a query for context type 'DeronCore.DataContext.Infrastructure.DeronContext'.
            // System.InvalidOperationException: A second operation was started on this context instance before a previous operation completed.
            // This is usually caused by different threads concurrently using the same instance of DbContext.
            // services.AddSingleton<IUnitOfWork>(x => new UnitOfWork(connection));

            // Scoped will dispose of DBContext:
            // System.ObjectDisposedException: 'Cannot access a disposed context instance. A common cause of this error is disposing a context instance 
            // that was resolved from dependency injection and then later trying to use the same context instance elsewhere in your application. 
            // services.AddScoped<IUnitOfWork>(x => new UnitOfWork(connection));

            // Transient will result in error similar to that we had in Scoped
            // services.AddTransient<IUnitOfWork>(x => new UnitOfWork(connection));

            // Original implementation below

            // Unit of work
            var bloggingContext = Net7BoilerplateContext.Create(configuration.GetConnectionString("BloggingDb"));

            // Singleton - as we want to reuse DbContext for transactions, and not open it every time we need it
            // Or in other words: Only Singleton will work - as UoW will get recycled after 1 call and then it will become unusable 
            services.AddSingleton<IUnitOfWork>(x => new UnitOfWork(bloggingContext));
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
                RegisterBloggingServicesFromAssembly(services, assembly);
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

        private static void RegisterBloggingServicesFromAssembly(IServiceCollection services, Assembly assembly)
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
