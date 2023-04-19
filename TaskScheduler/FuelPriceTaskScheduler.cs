using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TaskScheduler.interfaces;
using TaskScheduler.Models;

namespace TaskScheduler
{
    public class FuelPriceTaskScheduler : IFuelPriceTaskScheduler
    {
        private readonly SchedulerConfiguration _schedulerConfiguration;
        private readonly ILogger<FuelPriceTaskScheduler> _logger;
        private readonly IApiProvider _apiProvider;
        private readonly IFuelPriceRepository _dataRepository;

        public FuelPriceTaskScheduler(
            SchedulerConfiguration schedulerConfiguration, 
            ILogger<FuelPriceTaskScheduler> logger, 
            IApiProvider apiProvider, 
            IFuelPriceRepository dataRepository)
        {
            _schedulerConfiguration = schedulerConfiguration;
            _logger = logger;
            _apiProvider = apiProvider;
            _dataRepository = dataRepository;
        }

        public async Task Run()
        {
            while (true)
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    var fuelPriceResponse = (await _apiProvider.Get<FuelPriceApiResponseRoot>(_schedulerConfiguration.RequestUrl)).FuelPriceResponse;
                    var targetFromDate = DateTime.Now.AddDays(_schedulerConfiguration.DataExpirationDays * -1);

                    var filteredResponses = fuelPriceResponse.Prices?.Where(price => price.Period > targetFromDate) ?? new List<FuelPrice>();
                    await _dataRepository.Save(filteredResponses);

                    stopwatch.Stop();
                    _logger.LogTrace("finished fuel price download in {0} seconds", stopwatch.Elapsed.Seconds);
                    _logger.LogTrace("scheduler will execute again on {0}", DateTime.Now.AddDays(_schedulerConfiguration.TaskExecutionDelayDays).ToShortDateString());

                    Thread.Sleep(_schedulerConfiguration.TaskExecutionDelayInMilliseconds);
                }
                catch (Exception exception)
                {
                    _logger.LogError("Error occurred while attempting to download fuel prices", exception);
                }
            }
        }
    }
}
