using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMeter.Shared;

public sealed class ReadingRepository : IreadingRepository
{
    private static readonly Lazy<ReadingRepository> _instance =
        new(() => new ReadingRepository());

    public static ReadingRepository Instance => _instance.Value;

    private readonly object _lock = new();   // many handlers write at once, so guard the data
    private readonly List<Reading> _readings = new();
    private readonly Dictionary<string, DateTime> _lastSeen = new();

    private ReadingRepository() { }          
    public void Add(Reading reading)
    {
        lock (_lock)
        {
            _readings.Add(reading);
            _lastSeen[reading.ClientId] = reading.Timestamp;   // client details
        }
    }

    public IReadOnlyList<Reading> GetAll()
    {
        lock (_lock) { return _readings.ToList(); }
    }

    public IReadOnlyList<Reading> GetByClient(string clientId)
    {
        lock (_lock) { return _readings.Where(r => r.ClientId == clientId).ToList(); }
    }

    public DateTime? GetLastSeen(string clientId)
    {
        lock (_lock) { return _lastSeen.TryGetValue(clientId, out var t) ? t : null; }
    }

    public int Count
    {
        get { lock (_lock) { return _readings.Count; } }
    }
}