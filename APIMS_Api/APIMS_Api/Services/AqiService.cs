using APIMS_Api.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace APIMS_Api.Services
{
    public class AqiService : IAqiService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private const string CacheKey = "aqi_all";

        public AqiService(HttpClient httpClient, IMemoryCache cache, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _cache = cache;
            _configuration = configuration;
        }

        private async Task<List<StationFeature>> GetRawDataAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out List<StationFeature>? features))
            {
                var baseUrl = _configuration["AqiApi:BaseUrl"];
                var queryParams = _configuration["AqiApi:QueryParams"];
                var cacheMinutes = _configuration.GetValue<int>("AqiApi:CacheMinutes", 5);

                var response = await _httpClient.GetAsync($"{baseUrl}{queryParams}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<AqiApiResponse>(json, options);

                features = apiResponse?.Features ?? new List<StationFeature>();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(cacheMinutes));

                _cache.Set(CacheKey, features, cacheEntryOptions);
            }

            return features ?? new List<StationFeature>();
        }

        public async Task<List<StationFeature>> GetAllAsync()
        {
            return await GetRawDataAsync();
        }

        public async Task<List<StationFeature>> GetByStateAsync(string stateName)
        {
            var all = await GetRawDataAsync();
            return all.Where(f => string.Equals(f.Attributes.STATE_NAME, stateName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<List<StationFeature>> GetByRegionAsync(string regionName)
        {
            var all = await GetRawDataAsync();
            return all.Where(f => string.Equals(f.Attributes.REGION_NAME, regionName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<List<StationFeature>> GetByClassAsync(string className)
        {
            var all = await GetRawDataAsync();
            return all.Where(f => string.Equals(f.Attributes.CLASS, className, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<List<StationFeature>> GetByStationIdAsync(string stationId)
        {
            var all = await GetRawDataAsync();
            return all.Where(f => string.Equals(f.Attributes.STATION_ID, stationId, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<List<StationFeature>> GetByFiltersAsync(string? state = null, string? region = null, string? className = null, string? stationId = null)
        {
            var all = await GetRawDataAsync();
            var result = all;

            if (!string.IsNullOrWhiteSpace(state))
                result = result.Where(f => string.Equals(f.Attributes.STATE_NAME, state, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(region))
                result = result.Where(f => string.Equals(f.Attributes.REGION_NAME, region, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(className))
                result = result.Where(f => string.Equals(f.Attributes.CLASS, className, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(stationId))
                result = result.Where(f => string.Equals(f.Attributes.STATION_ID, stationId, StringComparison.OrdinalIgnoreCase)).ToList();

            return result;
        }
    }
}
