using Dapper;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using TaskScheduler.interfaces;
using TaskScheduler.Models;

namespace TaskScheduler
{
    public class FuelPriceRepository : IFuelPriceRepository
    {
        private readonly ILogger<FuelPriceRepository> _logger;
        private readonly DatabaseConfiguration _databaseConfiguration;
        private static readonly string _saveFuelPrices =
@"
    INSERT INTO FuelPrices (period, price)
        SELECT period, price FROM (VALUES (@Period, @Price)) as NewFuelPrice(period, price)
    WHERE NOT EXISTS (SELECT 1 FROM FuelPrices WHERE FuelPrices.period = NewFuelPrice.period)
        SELECT @@ROWCOUNT
";

        public FuelPriceRepository(ILogger<FuelPriceRepository> logger, DatabaseConfiguration databaseConfiguration)
        {
            _logger = logger;
            _databaseConfiguration = databaseConfiguration;
        }

        public async Task Save(IEnumerable<FuelPrice> fuelPrices)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);
            var insertedFuelPriceCount = 0;

            foreach (var price in fuelPrices)
            {
                try
                {
                    _logger.LogTrace("attempting to save fuel price {0} with period {1}", price.Price, price.Period);
                    insertedFuelPriceCount += await connection.QuerySingleAsync<int>(_saveFuelPrices, price);
                }
                catch (Exception exception)
                {
                    // log failed attempt and continue to save remaining fuel prices
                    _logger.LogError("error occured attempting to save fuel price", exception);
                }
            }

            _logger.LogTrace("saved {0} new fuel records", insertedFuelPriceCount);
        }
    }
}
