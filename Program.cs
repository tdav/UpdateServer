using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Reflection;

namespace AsbtCore.Update.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "UpdateManage";
            ConfigureLogging();
            CreateHost(args);
        }

        private static void CreateHost(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (System.Exception ex)
            {
                Log.Fatal($"Failed to start {Assembly.GetExecutingAssembly().GetName().Name}", ex);
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                      .ConfigureWebHostDefaults(webBuilder =>
                      {
                          webBuilder
                         .UseStartup<Startup>()
                         .UseKestrel();
                      })
                      .ConfigureAppConfiguration(configuration =>
                      {
                          configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                          configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
                      })
                      .UseSerilog();
        }

        private static void ConfigureLogging()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Information)
                .Enrich.WithProperty("Environment", environment)
                .WriteTo.RollingFile("logs/log.txt", Serilog.Events.LogEventLevel.Information, "@{Timestamp:HH:mm:ss.fff} --- {Message}{NewLine}{Exception}{NewLine}")
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
