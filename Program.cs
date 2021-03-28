using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace pwr_msi {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configureDelegate: (hostingContext, config) => {
                    config.AddEnvironmentVariables(prefix: "MSI_");
                })
                .ConfigureWebHostDefaults(configure: webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}
