using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.DTOs;

namespace XpressShip.Application.Interfaces.Services
{
    public interface IGeoInfoService
    {
        Task<LocationGeoInfoDTO> GetLocationGeoInfoByNameAsync(string locationName, CancellationToken cancellationToken = default);
        Task<LocationGeoInfoDTO> GetLocationGeoInfoByNameAsync(string countryName, string districtName, CancellationToken cancellationToken = default);
    }
}
