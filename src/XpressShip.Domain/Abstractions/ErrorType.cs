using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Domain.Abstractions
{
    public enum ErrorType
    {
        None,
        Unauthorized,
        Register,
        Login,
        BadRequest,
        Forbidden,
        NotFound,
        Conflict,
        Validation,
        Token,
        Unexpected,
        Unhandled
    }
}
