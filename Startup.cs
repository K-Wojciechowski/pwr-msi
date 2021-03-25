using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using pwr_msi.Models;
using pwr_msi.Services;

namespace pwr_msi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        private const string DefaultConnectionString = "Host=localhost;Database=msi;Username=msi;Password=msi";
        private const string DefaultServerAddress = "http://localhost:5000/";

        private byte[] GenerateJwtKey() {
            var jwtKeyString = Configuration.GetValue<string>("JWT_KEY", defaultValue: null);
            if (jwtKeyString == null) {
                var jwtKey = new byte[32];
                var rngProvider = new RNGCryptoServiceProvider();
                rngProvider.GetNonZeroBytes(jwtKey);
                rngProvider.Dispose();
                return jwtKey;
            }

            return Encoding.UTF8.GetBytes(jwtKeyString);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            var jwtKey = GenerateJwtKey();
            var jwtTokenValidationParameters = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                ValidateIssuer = true,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
            };

            services.Configure<AppConfig>(ac => {
                    ac.AuthTokenLifetime =
                        TimeSpan.FromSeconds(Configuration.GetValue("AUTH_TOKEN_LIFETIME", defaultValue: 300));
                    ac.RefreshTokenLifetime =
                        TimeSpan.FromSeconds(Configuration.GetValue("AUTH_REFRESH_TOKEN_LIFETIME", defaultValue: 3600));
                    ac.ServerAddress = Configuration.GetValue("SERVER_ADDRESS", DefaultServerAddress);
                    ac.JwtKey = jwtKey;
                    ac.JwtValidationParameters = jwtTokenValidationParameters;
                }
            );

            services.Configure<AuthService>(_ => {});
            services.AddControllersWithViews();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });
            services.AddDbContext<MsiDbContext>(options =>
                options.UseNpgsql(Configuration.GetValue("DB_CONNECTION_STRING", DefaultConnectionString)));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o => {
                    o.Authority = Configuration.GetValue("SERVER_ADDRESS", DefaultServerAddress);
                    o.Audience = Configuration.GetValue("SERVER_ADDRESS", DefaultServerAddress);
                    o.SaveToken = true;
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = jwtTokenValidationParameters;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger) {
            if (Configuration.GetValue<string>("JWT_KEY", null) == null) {
                logger.LogWarning("No JWT_KEY provided, using random value!");
            }

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseMiddleware<AuthMiddleware>();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa => {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment()) {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
