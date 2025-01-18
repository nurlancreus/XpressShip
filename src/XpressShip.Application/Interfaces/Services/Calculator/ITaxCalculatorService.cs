using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Interfaces.Services.Calculator
{
    public interface ITaxCalculatorService
    {
        public static decimal CalculateTaxAppliedPrice(decimal totalCost, decimal taxPercentage)
        {
            return totalCost * (1 - taxPercentage / 100);
        }


        public static decimal CalculateTaxAppliedPrice(decimal totalCost, Address destination)
        {
            return totalCost * (1 - destination.City.Country.TaxPercentage / 100);
        }
    }
}
