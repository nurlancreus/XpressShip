using Geolocation;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Interfaces.Services.Calculator
{
    public interface ICalculatorService
    {  
        public static int CalculateVolume(string dimensions)
        {
            IValidationService.ValidateDimensions(dimensions);

            return dimensions.Split('x').Select(int.Parse).Aggregate((x, y) => x * y);
        }
    }
}
