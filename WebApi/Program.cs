using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((builderContext, config) =>
            {
                IHostingEnvironment env = builderContext.HostingEnvironment;
                if (env.IsProduction())
                {
                    config.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
                }
            })
            .UseStartup<Startup>();
    }
}
