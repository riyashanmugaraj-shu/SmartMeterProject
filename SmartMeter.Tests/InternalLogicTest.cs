using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using client;
using NUnit.Framework;
using SmartMeter.Shared;

namespace SmartMeter.Tests;

// White-box unit tests.
// Written with knowledge of the internal code, to cover specific paths:
// the generator's minimum-value clamp, the repository's defensive copy,
// the last-seen overwrite, and the Singleton's structure.
public class InternalLogicTest
{
    [Test]
    public void Generator_ClampsAtMinimum_WhenTheRandomWalkWouldGoBelow()
    {
        var generator = new ReadingGenerator();
        typeof(ReadingGenerator)
            .GetField("_lastValue", BindingFlags.NonPublic | BindingFlags.Instance)!
           .SetValue(generator, 0.1);

       var values = new List<double>();
       for (int i = 0; i < 1000; i++)
            values.Add(generator.GenerateNext("c").Kwh);

        Assert.That(values, Is.All.GreaterThanOrEqualTo(0.1));   // never below the minimum
        Assert.That(values, Has.Some.EqualTo(0.1));              // the clamp path was actually used
    }

    [Test]
    public void Repository_GetAll_ReturnsACopy_NotTheInternalList()
    {
       var repo = ReadingRepository.Instance;
      repo.Add(new Reading("internal-" + Guid.NewGuid(), DateTime.UtcNow, 1.0));
       int before = repo.Count;

       //  know GetAll() returns _readings.ToList(), a separate List copy
       var copy = (List<Reading>)repo.GetAll();
       copy.Clear();

     // clearing the copy must not affect the real store
        Assert.That(repo.Count, Is.EqualTo(before));
    }

    [Test]
    public void Repository_LastSeen_IsOverwrittenByTheNewestReading()
    {
       var repo = ReadingRepository.Instance;
       string id = "internal-" + Guid.NewGuid();
        var first = new DateTime(2026, 11, 19, 9, 0, 0, DateTimeKind.Utc);
        var second = first.AddSeconds(30);

        // path where the client is already in the _lastSeen dictionary
       repo.Add(new Reading(id, first, 1.0));

        repo.Add(new Reading(id, second, 1.1));

       Assert.That(repo.GetLastSeen(id), Is.EqualTo(second));
        Assert.That(repo.GetByClient(id), Has.Count.EqualTo(2));
    }

    [Test]
    public void Repository_IsSealed_WithNoPublicConstructor()
    {
        //  no one can create a second instance or subclass it
        var type = typeof(ReadingRepository);

       Assert.That(type.IsSealed, Is.True);
        Assert.That(type.GetConstructors(BindingFlags.Public | BindingFlags.Instance), Is.Empty);
    }
}
