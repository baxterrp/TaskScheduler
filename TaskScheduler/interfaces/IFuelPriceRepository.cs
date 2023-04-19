using TaskScheduler.Models;

namespace TaskScheduler.interfaces
{
    public interface IFuelPriceRepository
    {
        Task Save(IEnumerable<FuelPrice> fuelPrices);
    }
}
