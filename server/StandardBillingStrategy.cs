using SmartMeter.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://refactoring.guru/design-patterns/strategy/csharp/example

public class StandardBillingStrategy : IBillingStrategy
{
    private readonly decimal _pricePerKwh;

    public StandardBillingStrategy(decimal pricePerKwh)
    {
        _pricePerKwh = pricePerKwh;
    }

    public decimal CalculateBill(IEnumerable<Reading> readings)
    {
        double totalKwh = readings.Sum(r => r.Kwh);

        decimal bill = (decimal)totalKwh * _pricePerKwh;

        return Math.Round(bill, 2);
    }
}
