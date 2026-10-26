using System;
using SmartMeter.Shared;

// other code uses this interface,
// so the real store can be swapped for a mock in unit tests
public interface IreadingRepository
{
    void Add(Reading reading);
    IReadOnlyList<Reading> GetAll();
    IReadOnlyList<Reading> GetByClient(string clientId);
    DateTime? GetLastSeen(string clientId);
    int Count { get; }
}
