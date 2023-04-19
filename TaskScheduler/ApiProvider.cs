using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskScheduler.interfaces;

namespace TaskScheduler
{
    public class ApiProvider : IApiProvider
    {
        private readonly ILogger<ApiProvider> _logger;

        public ApiProvider(ILogger<ApiProvider> logger)
        {
            _logger = logger;
        }

        public async Task<TApiResponse> Get<TApiResponse>(string url)
        {
            _logger.LogTrace("attempting to GET type {0} from {1}", nameof(TApiResponse), url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var client = new HttpClient();

            var response = await client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TApiResponse>(stringResponse);
        }
    }
}
