using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.DTOs;
using XpressShip.Application.Options;
using XpressShip.Domain.Abstractions;

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

        public async Task<Result<LocationGeoInfoDTO>> GetLocationGeoInfoByNameAsync(string locationName, CancellationToken cancellationToken = default)
        {
            var url = $"{_apiOptions.BaseUrl}/search?q={locationName}&format=json&limit=1&api_key={_apiOptions.ApiKey}";

            var response = await _httpClient.GetStringAsync(url, cancellationToken);

            var geoInfoList = JsonSerializer.Deserialize<List<LocationGeoInfoDTO>>(response);

            if (geoInfoList == null || geoInfoList.Count == 0)
            {
                return Result<LocationGeoInfoDTO>.Failure(Error.UnexpectedError("Something happened when deserializing GeoInfo Response"));
            }

            var geoInfo = geoInfoList[0];

            return Result<LocationGeoInfoDTO>.Success(geoInfo);
        }

        public async Task<Result<LocationGeoInfoDTO>> GetLocationGeoInfoByNameAsync(string countryName, string districtName, CancellationToken cancellationToken = default)
        {
            var result = await GetLocationGeoInfoByNameAsync(districtName, cancellationToken);

            if (!result.IsSuccess)
                result = await GetLocationGeoInfoByNameAsync(countryName, cancellationToken);
            
            return result;
        }
    }
}
