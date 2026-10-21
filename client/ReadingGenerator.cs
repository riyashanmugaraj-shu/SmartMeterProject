using SmartMeter.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
     public class ReadingGenerator
    {
        private readonly Random _rand = new();
        private double _lastValue = 2.0;

        // makes a reading that  slightly changes from the previous so it looks realistic
        public Reading GenerateNext(string clientId)
        {
            _lastValue = Math.Max(0.1, _lastValue + (_rand.NextDouble() - 0.5) * 0.4);
            return new Reading(clientId, DateTime.UtcNow, Math.Round(_lastValue, 2));
        }

        // random wait between 15 and 60 seconds
        public int NextDelayMs() => _rand.Next(15000, 60001);
    }
}
