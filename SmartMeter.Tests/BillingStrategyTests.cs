using SmartMeter.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SmartMeter.Tests
{
    [TestFixture]
    public class BillingStrategyTests
    {
        // Calculate bill for one reading
        [Test]
        public void CalculateBill_SingleReading_ReturnsCorrectAmount()
        {
            var strategy = new StandardBillingStrategy(0.25m);

            var readings = new List<Reading>
            {
                new Reading("client-01", DateTime.UtcNow, 2.0)
            };

            decimal result = strategy.CalculateBill(readings);

            Assert.That(result, Is.EqualTo(0.50m));
        }

        // Calculate bill for multiple readings
        [Test]
        public void CalculateBill_MultipleReadings_ReturnsTotalAmount()
        {
            var strategy = new StandardBillingStrategy(0.25m);

            var readings = new List<Reading>
            {
                new Reading("client-01", DateTime.UtcNow, 2.0),
                new Reading("client-01", DateTime.UtcNow, 3.0),
                new Reading("client-01", DateTime.UtcNow, 1.0)
            };

            decimal result = strategy.CalculateBill(readings);

            Assert.That(result, Is.EqualTo(1.50m));
        }

        // Empty readings collection
        [Test]
        public void CalculateBill_NoReadings_ReturnsZero()
        {
            var strategy = new StandardBillingStrategy(0.25m);

            var readings = new List<Reading>();

            decimal result = strategy.CalculateBill(readings);

            Assert.That(result, Is.EqualTo(0.00m));
        }

        // Bill rounding
        [Test]
        public void CalculateBill_DecimalAmount_RoundsToTwoPlaces()
        {
            var strategy = new StandardBillingStrategy(0.25m);

            var readings = new List<Reading>
            {
                new Reading("client-01", DateTime.UtcNow, 1.02)
            };

            decimal result = strategy.CalculateBill(readings);

            Assert.That(result, Is.EqualTo(0.26m));
        }

        // Test ability to switch strategies
        [Test]
        public void BillingCalculator_ChangingStrategy_UsesNewStrategy()
        {
            var calculator = new BillingCalculator(
                new StandardBillingStrategy(0.25m));

            var readings = new List<Reading>
            {
                new Reading("client-01", DateTime.UtcNow, 4.0)
            };

            decimal originalBill = calculator.CalculateBill(readings);

            calculator.SetBillingStrategy(
                new StandardBillingStrategy(0.30m));

            decimal updatedBill = calculator.CalculateBill(readings);

            Assert.That(originalBill, Is.EqualTo(1.00m));
            Assert.That(updatedBill, Is.EqualTo(1.20m));
        }
    }
}
