using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net7CoreApiBoilerplate.Api.Infrastructure;
using Net7CoreApiBoilerplate.Api.Infrastructure.Helpers;
using Net7CoreApiBoilerplate.Api.Utility;
using Net7CoreApiBoilerplate.Api.Utility.Extensions;
using Net7CoreApiBoilerplate.DbContext.Infrastructure;
using Net7CoreApiBoilerplate.Infrastructure.Settings;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Net7CoreApiBoilerplate.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }
        public IAppSettings Settings { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
            Settings = new AppSettings(configuration);
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // I am literally registering this DB Context here, just so I can use EF Core Identity
            services.AddDbContext<Net7BoilerplateContext>(options => options.UseSqlServer(Configuration.GetConnectionString("BloggingDb")));

            // The validation system in .NET Core 3.0 and later treats non-nullable parameters or bound properties as if they had a [Required] attribute.
            // Value types such as decimal and int are non-nullable. To turn it off:
            // AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)

            services.AddControllers() // we need only controllers for our api now
                // Why do we use config below? Well basically to handle DI-based activator with the AddControllersAsServices
                // Read more with examples here: https://andrewlock.net/controller-activation-and-dependency-injection-in-asp-net-core-mvc/
                .AddControllersAsServices()
                .AddNewtonsoftJson(options =>
                {
                    // To prevent "A possible object cycle was detected which is not supported" error
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                    // To get our property names serialized in the first letter lowercased
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            // Helpers
            IdentityHelper.ConfigureService(services);
            AuthenticationHelper.ConfigureService(services, Settings.Issuer, Settings.Audience, Settings.Key);
            // AuthenticationHelper.ConfigureServiceSimple(services, Settings.Issuer, Settings.Audience, Settings.Key);

            // Register UoW and other helper services 
            // services.RegisterUtilityServices_depricated(Configuration); // This method won't work, read comments on this method

            // However, below is the solution with generic UoW implementation
            // Idea to upgrade my Unit of work to generic seen here: https://github.com/ffernandolima/ef-core-data-access/tree/ef-core-7
            services.AddScoped<Microsoft.EntityFrameworkCore.DbContext, Net7BoilerplateContext>();
            services.AddUnitOfWork();
            services.AddUnitOfWork<Net7BoilerplateContext>(); // This does not need to be registered. Everythong will work without this line of code. But I will leave it here just for good measure.


            // Do our magic to load services automatically, and resolve their DI
            services.AutoRegisterServices();

            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins(Settings.ClientBaseUrl)
                        .SetIsOriginAllowed(isOriginAllowed: _ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            var logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .Enrich.WithProperty("ApplicationName", "My application name")
                                .WriteTo.Console()
                                .MinimumLevel.Override("  Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
                                .WriteTo.File($"{Settings.BaseFolder}{Settings.LogsFolder}\\my_application_log.txt", rollingInterval: RollingInterval.Hour);


            if (HostingEnvironment.IsDevelopment())
            {
                SwaggerHelper.ConfigureService(services);

                // If deploying to Azure, I should probably add lines below as they are going to enable us to view events on the Azure: https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview?tabs=net
                //var telemetryConfiguration = new TelemetryConfiguration(globConfig.AppInsights.InstrumentationKey);
                //logger.WriteTo.ApplicationInsights(telemetryConfiguration, telemetryConverter: TelemetryConverter.Traces);
            }

            Log.Logger = logger.CreateLogger();

            // We are creating folders for uploads and stuff in case they don't exist on startup
            FolderBuilder.BuildFolders(Settings);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("default"); // because of CORS issue - this is try to fix it as seen here: https://stackoverflow.com/a/56984245/4267429 - Ordering matters apparently

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            #region Swagger

            if (HostingEnvironment.IsDevelopment())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = "";
                });
            }

            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
