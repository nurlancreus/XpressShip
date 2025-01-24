using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Application.Options.Token
{
    public class TokenSettings
    {
        public AccessSettings Access { get; set; } = null!;
        public RefreshSettings Refresh { get; set; } = null!;
    }
}
