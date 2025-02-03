using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Domain.Exceptions
{
    public class XpressShipException : Exception
    {
        public XpressShipException() { }
        public XpressShipException(string message) : base(message) { }
        public XpressShipException(string message, Exception inner) : base(message, inner) { }

    }
}
