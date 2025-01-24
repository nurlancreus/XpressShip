using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.DTOs;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Application.Abstractions.Services
{
    public interface IGeoInfoService
    {
        Task<Result<LocationGeoInfoDTO>> GetLocationGeoInfoByNameAsync(string locationName, CancellationToken cancellationToken = default);
        Task<Result<LocationGeoInfoDTO>> GetLocationGeoInfoByNameAsync(string countryName, string districtName, CancellationToken cancellationToken = default);
    }
}
