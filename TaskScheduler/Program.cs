using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskScheduler.interfaces;
using TaskScheduler.Models;

namespace TaskScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional : false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();

            // add logging to console
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            // configure scheduler configuration for DI
            var schedulerConfig = new SchedulerConfiguration();
            config.GetSection("SchedulerConfiguration").Bind(schedulerConfig);
            services.AddSingleton(schedulerConfig);

            // configure db connection for DI
            var dbConfig = new DatabaseConfiguration();
            config.GetSection("DatabaseConfiguration").Bind(dbConfig);
            services.AddSingleton(dbConfig);

            services.AddSingleton<IFuelPriceTaskScheduler, FuelPriceTaskScheduler>();
            services.AddSingleton<IApiProvider, ApiProvider>();

            services.AddSingleton<IFuelPriceRepository, FuelPriceRepository>();

            services.BuildServiceProvider().GetRequiredService<IFuelPriceTaskScheduler>().Run().Wait();
        }
    }
}