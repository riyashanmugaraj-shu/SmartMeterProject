using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;

namespace SmartMeter.Shared;

public interface IMessageAdapter<T>
{
    string Serialize(T message);
    T Deserialize(string raw);
}

public class JsonReadingAdapter : IMessageAdapter<Reading>
{
    public string Serialize(Reading r) => JsonSerializer.Serialize(r);
    public Reading Deserialize(string raw) => JsonSerializer.Deserialize<Reading>(raw)!;
}
