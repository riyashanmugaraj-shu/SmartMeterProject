using SmartMeter.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BillingCalculator
{
    private IBillingStrategy _billingStrategy;

    public BillingCalculator(IBillingStrategy billingStrategy)
    {
        _billingStrategy = billingStrategy;
    }

    public void SetBillingStrategy(IBillingStrategy billingStrategy)
    {
        _billingStrategy = billingStrategy;
    }

    public decimal CalculateBill(IEnumerable<Reading> readings)
    {
        return _billingStrategy.CalculateBill(readings);
    }
}