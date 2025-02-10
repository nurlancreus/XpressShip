using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Tests.Common.Constants
{
    public static partial class Constants
    {
        public static class Country
        {

            public static readonly Dictionary<string, (string postalCodePattern, string countryCode, string[] cities, decimal taxPercentage)> CountryData = new()
                {
                    {"Azerbaijan", (@"AZ\s\d{4}$", "AZE", ["Baku", "Ganja"], 20) },
                    {"Russia", (@"\d{6}$", "RUS", ["Moscow", "Saint Petersburg", "Novosibirsk"], 20) },
                    {"Georgia", (@"\d{4}$", "GEO", ["Tbilisi", "Batumi", "Kutaisi"], 18) },
                    {"Turkiye", (@"\d{5}$", "TUR", ["Istanbul", "Ankara", "Izmir"], 18) },
                    {"Iran", (@"\d{5}$", "IRN", ["Tehran", "Mashhad", "Isfahan"], 9) }
                };
        }
    }
}
