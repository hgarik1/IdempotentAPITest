using IdempotentAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace IdempotentAPITest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IDistributedCache _cache;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }


        [HttpPost(Name = "GetWeatherForecast")]
        [Idempotent(Enabled = true)]
        public async Task<IActionResult> Post()
        {
            var obj = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();

            var cachedDataString = JsonSerializer.Serialize(obj);

            var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);

            // Setting up the cache options
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
                .SetSlidingExpiration(TimeSpan.FromMinutes(3));

            _cache.Set("WeatherForecast", dataToCache, options);

            return Ok(obj);
        }


        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            byte[] cachedData = await _cache.GetAsync("WeatherForecast");

            var cachedDataString = Encoding.UTF8.GetString(cachedData);

            return Ok(cachedDataString);
        }
    }
}