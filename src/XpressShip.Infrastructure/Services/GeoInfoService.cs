using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using XpressShip.Application.DTOs;
using XpressShip.Application.Interfaces.Services;
using XpressShip.Application.Options;

namespace XpressShip.Infrastructure.Services
{
    public class GeoInfoService : IGeoInfoService
    {
        private readonly HttpClient _httpClient;
        private readonly APISettings _apiOptions;

        public GeoInfoService(IHttpClientFactory httpClientFactory, IOptionsSnapshot<APISettings> options)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiOptions = options.Get(APISettings.GeoCodeAPI);
        }

        public async Task<LocationGeoInfoDTO?> GetLocationGeoInfoByNameAsync(string locationName, CancellationToken cancellationToken = default)
        {
            // Define the API endpoint
            var url = $"{_apiOptions.BaseUrl}/search?q={locationName}&format=json&limit=1&api_key={_apiOptions.ApiKey}";

            // Send the GET request
            var response = await _httpClient.GetStringAsync(url, cancellationToken);

            // Deserialize the response JSON into a list of objects
            var geoInfoList = JsonSerializer.Deserialize<List<GeoInfo>>(response);

            // Return the first result (or null if no results)
            if (geoInfoList == null || geoInfoList.Count == 0)
            {
                // implement results pattern 
                return null;
            }

            var geoInfo = geoInfoList[0];

            // Map the response to the CountryGeoInfo DTO
            return new LocationGeoInfoDTO
            {
                Name = geoInfo.DisplayName,
                Latitude = double.Parse(geoInfo.Lat),
                Longitude = double.Parse(geoInfo.Lon),
            };
        }

        public async Task<LocationGeoInfoDTO> GetLocationGeoInfoByNameAsync(string countryName, string districtName, CancellationToken cancellationToken = default)
        {
            var response = await GetLocationGeoInfoByNameAsync(districtName, cancellationToken);

            response ??= await GetLocationGeoInfoByNameAsync(countryName, cancellationToken);

            if (response == null)
            {
                // implement results pattern 
                return null;
            }

            return response;
        }
    }

    // Helper model to deserialize the API response
    public class GeoInfo
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;
        [JsonPropertyName("lat")]
        public string Lat { get; set; } = string.Empty;
        [JsonPropertyName("lon")]
        public string Lon { get; set; } = string.Empty;
    }
}
