using client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SmartMeter.Tests
{
    [TestFixture]
    public class CommunicationObserverTests
    {
        // Test observer that records notifications
        private class TestObserver : ICommunicationErrorObserver
        {
            public List<string> Messages { get; } = new();

            public void OnCommunicationError(string message)
            {
                Messages.Add(message);
            }
        }

        // Observer receives notification
        [Test]
        public void Notify_AttachedObserver_ReceivesErrorMessage()
        {
            var notifier = new CommunicationErrorNotifier();
            var observer = new TestObserver();

            notifier.Attach(observer);

            notifier.Notify("Server connection failed");

            Assert.That(observer.Messages.Count, Is.EqualTo(1));
            Assert.That(observer.Messages[0],
                Is.EqualTo("Server connection failed"));
        }

        // Detached observer is not notified
        [Test]
        public void Notify_DetachedObserver_ReceivesNoMessages()
        {
            var notifier = new CommunicationErrorNotifier();
            var observer = new TestObserver();

            notifier.Attach(observer);
            notifier.Detach(observer);

            notifier.Notify("Server connection failed");

            Assert.That(observer.Messages, Is.Empty);
        }

        // Duplicate registration should be ignored
        [Test]
        public void Attach_SameObserverTwice_OnlyNotifiedOnce()
        {
            var notifier = new CommunicationErrorNotifier();
            var observer = new TestObserver();

            notifier.Attach(observer);
            notifier.Attach(observer);

            notifier.Notify("Communication error");

            Assert.That(observer.Messages.Count, Is.EqualTo(1));
        }
    }
}
