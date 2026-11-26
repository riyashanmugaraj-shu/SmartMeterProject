using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using client;
using NUnit.Framework;

namespace SmartMeter.Tests;

// Tests for the client's fake meter: ReadingGenerator makes realistic readings
// and decides how long to wait between sends (15-60 seconds, per the brief).
public class ReadingGeneratorTests
{
    [Test]
    public void GenerateNext_UsesTheGivenClientId()
    {
        var generator = new ReadingGenerator();

        // server tells clients apart by this ID
         Assert.That(generator.GenerateNext("client-07").ClientId, Is.EqualTo("client-07"));
    }

    [Test]
    public void GenerateNext_KwhNeverBelowMinimum()
    {
        var generator = new ReadingGenerator();

        // repeat many times because the values are random
        for (int i = 0; i < 1000; i++)
            Assert.That(generator.GenerateNext("c").Kwh, Is.GreaterThanOrEqualTo(0.1));
    }

    [Test]
    public void GenerateNext_KwhIsRoundedToTwoDecimalPlaces()
    {
        var generator = new ReadingGenerator();

        for (int i = 0; i < 200; i++)
        {
            double kwh = generator.GenerateNext("c").Kwh;
           Assert.That(Math.Round(kwh, 2), Is.EqualTo(kwh));
        }
    }

    [Test]
    public void GenerateNext_ConsecutiveReadingsStayClose()
    {
        var generator = new ReadingGenerator();
        double previous = generator.GenerateNext("c").Kwh;

        for (int i = 0; i < 1000; i++)
        {
            double current = generator.GenerateNext("c").Kwh;

            Assert.That(Math.Abs(current - previous), Is.LessThanOrEqualTo(0.21));
           previous = current;
        }
    }

    [Test]
    public void GenerateNext_TimestampIsRecentUtc()
    {
        var generator = new ReadingGenerator();

       var reading = generator.GenerateNext("c");

        // Timestamp should be "now" in UTC
        Assert.That(reading.Timestamp, Is.EqualTo(DateTime.UtcNow).Within(5).Seconds);
    }

    [Test]
    public void NextDelayMs_IsAlwaysBetween15And60Seconds()
    {
        var generator = new ReadingGenerator();

        //  catches a short testing delay left in by mistake.
        for (int i = 0; i < 1000; i++)
            Assert.That(generator.NextDelayMs(), Is.InRange(15000, 60000));
    }
}