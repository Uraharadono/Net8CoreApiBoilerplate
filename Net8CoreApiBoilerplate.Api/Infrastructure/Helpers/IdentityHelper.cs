using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Net8CoreApiBoilerplate.DbContext.Entities.Identity;
using Net8CoreApiBoilerplate.DbContext.Infrastructure;

namespace Net8CoreApiBoilerplate.Api.Infrastructure.Helpers
{
    public class IdentityHelper
    {
        public static void ConfigureService(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<Net7BoilerplateContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                // Ideally it should be no less than 6, Vue boilerplate forced me to scale it way down,
                // becase people are lazy and complaining all the time
                options.Password.RequiredUniqueChars = 1; 

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });
        }
    }
}
