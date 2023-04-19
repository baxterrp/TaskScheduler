using System.Text.Json.Serialization;

namespace TaskScheduler.Models
{
    public class FuelPriceApiResponseRoot
    {
        [JsonPropertyName("response")]
        public FuelPriceApiResponse FuelPriceResponse { get; set; }
    }

    public class FuelPriceApiResponse
    {
        [JsonPropertyName("data")]
        public FuelPrice[]? Prices { get; set; }
    }

    public class FuelPrice
    {
        [JsonPropertyName("period")]
        public DateTime? Period { get; set; }

        [JsonPropertyName("value")]
        public float Price { get; set; }
    }
}
