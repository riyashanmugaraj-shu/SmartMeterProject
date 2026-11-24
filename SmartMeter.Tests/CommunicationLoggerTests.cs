using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace SmartMeter.Tests
{
    [TestFixture]
    [NonParallelizable]
    public class CommunicationLoggerTests
    {
        // Verify errors are written to a file
        [Test]
        public async Task LogErrorAsync_CommunicationFailure_WritesErrorToFile()
        {
            string uniqueMessage =
                "TEST-" + Guid.NewGuid().ToString();

            var exception = new IOException(
                "Simulated network failure");

            await CommunicationLogger.LogErrorAsync(
                uniqueMessage, exception);

            string logPath = Path.Combine(
                Environment.CurrentDirectory,
                "communication-errors.log");

            Assert.That(File.Exists(logPath), Is.True);

            string logContents =
                await File.ReadAllTextAsync(logPath);

            Assert.That(logContents,
                Does.Contain(uniqueMessage));

            Assert.That(logContents,
                Does.Contain("IOException: Simulated network failure"));
        }

        // Verify concurrent logging
        [Test]
        public async Task LogErrorAsync_ConcurrentErrors_RecordsAllMessages()
        {
            string testId = Guid.NewGuid().ToString();

            var tasks = Enumerable.Range(1, 10)
                .Select(i => CommunicationLogger.LogErrorAsync(
                    $"CONCURRENT-{testId}-{i:D2}"))
                .ToArray();

            await Task.WhenAll(tasks);

            string logPath = Path.Combine(
                Environment.CurrentDirectory,
                "communication-errors.log");

            string[] lines = await File.ReadAllLinesAsync(logPath);

            for (int i = 1; i <= 10; i++)
            {
                string expectedMessage =
                    $"CONCURRENT-{testId}-{i:D2}";

                Assert.That(
                    lines.Count(line =>
                        line.Contains(expectedMessage)),
                    Is.EqualTo(1));
            }
        }
    }
}
