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
        decimal CalculateTaxAppliedPrice(decimal totalCost, decimal taxPercentage);

        decimal CalculateTaxAppliedPrice(decimal totalCost, Address destination);
    }
}
