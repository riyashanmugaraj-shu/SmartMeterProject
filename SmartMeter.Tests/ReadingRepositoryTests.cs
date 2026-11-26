using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmartMeter.Shared;

namespace SmartMeter.Tests;

// Tests for the Singleton data store on the server.
// The repository keeps its data between tests (it's a Singleton), so each test
// uses a unique client ID, or compares the count before and after.
public class ReadingRepositoryTests
{
    // unique client ID so tests don't interfere with each other
         private static string NewId() => "test-" + Guid.NewGuid();

     [Test]
        public void Instance_ReturnsTheSameObjectEveryTime()
        {
            // singleton rule: only one instance ever exists
            Assert.That(ReadingRepository.Instance, Is.SameAs(ReadingRepository.Instance));
         }

     [Test]
        public void Add_IncreasesCountByOne()
        {
        var repo = ReadingRepository.Instance;
        int before = repo.Count;

        repo.Add(new Reading(NewId(), DateTime.UtcNow, 1.0));

        Assert.That(repo.Count, Is.EqualTo(before + 1));
         }

    [Test]
        public void GetByClient_ReturnsOnlyThatClientsReadings()
        {
           var repo = ReadingRepository.Instance;
            string a = NewId(), b = NewId();
            repo.Add(new Reading(a, DateTime.UtcNow, 1.0));
            repo.Add(new Reading(a, DateTime.UtcNow, 1.1));
            repo.Add(new Reading(b, DateTime.UtcNow, 2.0));
            
              var result = repo.GetByClient(a);

            // Two for client a, and none of client b's
             Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.All(r => r.ClientId == a), Is.True);
         }

    [Test]
    public void GetLastSeen_ReturnsLatestTimestamp_AndNullForUnknownClient()
    {
        var repo = ReadingRepository.Instance;
        string id = NewId();
        var time = new DateTime(2026, 11, 19, 10, 30, 0, DateTimeKind.Utc);
        repo.Add(new Reading(id, time, 1.0));

        Assert.That(repo.GetLastSeen(id), Is.EqualTo(time));   // known client
        Assert.That(repo.GetLastSeen(NewId()), Is.Null);        // unknown client
    }

    [Test]
    public void Add_FromManyThreadsAtOnce_LosesNoReadings()
    {
        var repo = ReadingRepository.Instance;
        int before = repo.Count;

        // Simulate 12 clients each adding 100 readings at the same moment.
        // Without the lock in ReadingRepository, readings would be lost 
        Parallel.For(0, 12, new ParallelOptions { MaxDegreeOfParallelism = 12 }, clientNumber =>
        {
           string id = NewId();
            for (int i = 0; i < 100; i++)
                repo.Add(new Reading(id, DateTime.UtcNow, 1.0));
        });

       Assert.That(repo.Count, Is.EqualTo(before + 1200));
    }
}