using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Application.Options.Cache
{
    public class InMemoryCacheSettings
    {
        public const string CountryData = "CountryData";

        public int SlidingExpirationInHours { get; set; }
        public int AbsoluteExpirationInHours { get; set; }
        public string CacheKey { get; set; } = string.Empty;
    }
}
