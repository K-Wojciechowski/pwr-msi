using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using pwr_msi.Models;
using pwr_msi.Serialization;
using pwr_msi.Services;

namespace pwr_msi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            var appConfig = AppConfig.FromConfiguration(Configuration);

            services.AddSingleton(appConfig);

            services.AddScoped<AuthService, AuthService>();
            services.AddScoped<AccountEmailService, AccountEmailService>();
            services.AddScoped<AdminCommonService, AdminCommonService>();
            services.AddScoped<MenuService, MenuService>();
            services.AddScoped<OrderDetailsService, OrderDetailsService>();
            services.AddScoped<OrderTaskService, OrderTaskService>();
            services.AddScoped<PaymentService, PaymentService>();
            services.AddScoped<S3Service, S3Service>();

            services.AddLocalization(setupAction: options => options.ResourcesPath = "Resources");
            services.AddHttpClient();

            services.AddControllersWithViews().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization().AddNewtonsoftJson(s => {
                    // s.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                    s.SerializerSettings.DateParseHandling = DateParseHandling.None;
                    s.SerializerSettings.FloatParseHandling = FloatParseHandling.Decimal;
                    s.SerializerSettings.Converters.Add(
                        new StringEnumConverter(new UpperSnakeCaseNamingStrategy(), allowIntegerValues: false)
                    );
                    s.SerializerSettings.Converters.Add(
                        new ZonedDateTimeJsonConverter()
                    );
                    s.SerializerSettings.Converters.Add(
                        new NullableZonedDateTimeJsonConverter()
                    );
                });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration: configuration => { configuration.RootPath = "ClientApp/dist"; });
            services.AddDbContext<MsiDbContext>(optionsAction: options =>
                options
                    .UseNpgsql(appConfig.DbConnectionString,
                        npgsqlOptionsAction: o => o.UseNodaTime().EnableRetryOnFailure().UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery))
            );
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(configureOptions: o => {
                    o.Authority = appConfig.ServerAddress;
                    o.Audience = appConfig.ServerAddress;
                    o.SaveToken = true;
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = appConfig.JwtValidationParameters;
                    o.Configuration = new OpenIdConnectConfiguration {Issuer = appConfig.ServerAddress};
                });
            services.AddStackExchangeRedisCache(options => {
                options.Configuration = appConfig.RedisConnectionString;
                options.InstanceName = appConfig.RedisInstanceName;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger) {
            if (Configuration.GetValue<string>(key: "JWT_KEY", defaultValue: null) == null)
                logger.LogWarning(message: "No JWT_KEY provided, using random value!");

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler(errorHandlingPath: "/Error");

            var supportedCultures = new[] {"en-US", "pl"};
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(defaultCulture: supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<AuthMiddleware>();

            app.UseEndpoints(configure: endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(configuration: spa => {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment()) spa.UseAngularCliServer(npmScript: "start");
            });
        }
    }
}
