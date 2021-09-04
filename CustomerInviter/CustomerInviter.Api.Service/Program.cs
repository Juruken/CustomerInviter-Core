using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CustomerInviter.Api.Service.AutofacModules;
using CustomerInviter.Core.AutofacModules;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Serilog.Enrichers;

namespace CustomerInviter.Api.Service
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogging();
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.SuppressStatusMessages(true);
                    builder.UseStartup<Startup>();
                })
                .ConfigureContainer<ContainerBuilder>(ConfigureAutofacContainer)
                .UseSerilog();
        }

        public static void ConfigureAutofacContainer(ContainerBuilder builder)
        {
            // Registers all modules in this assembly, if wanting a specific one just list the modules you wish to import
            builder.RegisterAssemblyModules(typeof(ServiceModule).Assembly, typeof(ConfigurationModule).Assembly);
            builder.RegisterType<CustomJsonSerializer>().As<JsonSerializer>();
        }

        private static void ConfigureLogging()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json")
                .Build();

            var assembly = Assembly.GetEntryAssembly().GetName();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", assembly.Name)
                .Enrich.WithProperty("Version", assembly.Version)
                .Enrich.With<EnvironmentUserNameEnricher>()
                .WriteTo.Seq(config["seqApplicationUrl"], apiKey: config["seqApplicationKey"])
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
