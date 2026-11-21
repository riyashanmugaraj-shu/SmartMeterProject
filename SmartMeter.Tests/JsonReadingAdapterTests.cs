using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using NUnit.Framework;
using SmartMeter.Shared;

namespace SmartMeter.Tests;

// tests for the Adapter pattern: JsonReadingAdapter translates a Reading object
// to JSON text (client side) and back to an object (server side).
public class JsonReadingAdapterTests
{
    private readonly JsonReadingAdapter _adapter = new();

    [Test]
    public void SerializeThenDeserialize_ReturnsEqualReading()
    {
        // Arrange: a reading with fixed values
        var original = new Reading("client-01", new DateTime(2026, 11, 19, 9, 0, 0, DateTimeKind.Utc), 2.35);

        // Act: send it "over the wire" and back
        var result = _adapter.Deserialize(_adapter.Serialize(original));

        // Assert: nothing was lost or changed on the way
        Assert.That(result, Is.EqualTo(original));
    }

    [Test]
    public void Serialize_ContainsExpectedFieldNames()
    {
        var json = _adapter.Serialize(new Reading("client-01", DateTime.UtcNow, 1.5));

        // The server expects these three fields in the message
        Assert.That(json, Does.Contain("ClientId"));
        Assert.That(json, Does.Contain("Timestamp"));
        Assert.That(json, Does.Contain("Kwh"));
    }

    [Test]
    public void Serialize_ProducesSingleLine_SoNewlineFramingWorks()
    {
        var json = _adapter.Serialize(new Reading("client-01", DateTime.UtcNow, 1.5));

        // Messages are separated by a newline so a newline inside the JSON
        // would split one reading into two broken messages
        Assert.That(json, Does.Not.Contain("\n"));
    }

    [Test]
    public void Deserialize_InvalidJson_ThrowsJsonException()
    {
        // Failure case: bad input must fail in a controlled way
        Assert.Throws<JsonException>(() => _adapter.Deserialize("not json"));
    }
}