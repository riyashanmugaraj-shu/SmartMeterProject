using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using client;
using NUnit.Framework;
using SmartMeter.Shared;

namespace SmartMeter.Tests;

// Integration tests (black box)
// A simulated meter sends readings over a real TCP connection to a test server,
// which parses, stores and acknowledges them: the generator, JSON adapter, network
// framing and repository working together. The test server copies the real server's
// flow (accept loop, one task per client, line-by-line reading, ACK).
// TLS is left out here and is checked manually.
public class ClientToServerTest
{
    private TcpListener _listener = null!;
    private Task _acceptLoop = null!;
    private int _port;
    private readonly JsonReadingAdapter _adapter = new();

    [SetUp]
    public void StartTestServer()
    {
        _listener = new TcpListener(IPAddress.Loopback, 0);   // port 0 = any free port
        _listener.Start();
        _port = ((IPEndPoint)_listener.LocalEndpoint).Port;
        _acceptLoop = Task.Run(AcceptLoopAsync);
    }

    [TearDown]
    public void StopTestServer()
    {
        _listener.Stop();
        _acceptLoop.Wait(2000);
        _listener.Dispose();   // frees the listener, which silences the warning
    }

    private async Task AcceptLoopAsync()
    {
        try
        {
            while (true)
            {
                var tcp = await _listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleAsync(tcp));      // one task per client
            }
        }
        catch (Exception)
        {
            // listener was stopped at the end of the test
        }
    }

    private async Task HandleAsync(TcpClient tcp)
    {
        using var tcpClient = tcp;
        using var stream = tcpClient.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            var reading = _adapter.Deserialize(line);
            ReadingRepository.Instance.Add(reading);
            await writer.WriteLineAsync("ACK:" + reading.ClientId);
        }
    }

    // Acts as one smart meter: sends `count` readings and returns the replies
    private async Task<List<string>> SendReadingsAsync(string clientId, int count)
    {
        var generator = new ReadingGenerator();
        var replies = new List<string>();

        using var tcp = new TcpClient();
        await tcp.ConnectAsync(IPAddress.Loopback, _port);
        using var stream = tcp.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        for (int i = 0; i < count; i++)
        {
            await writer.WriteLineAsync(_adapter.Serialize(generator.GenerateNext(clientId)));
            replies.Add((await reader.ReadLineAsync())!);
        }
        return replies;
    }

    [Test]
    public async Task OneClient_ReadingsTravelOverTcp_AreAcknowledgedAndStored()
    {
        string id = "flow-" + Guid.NewGuid();

        var replies = await SendReadingsAsync(id, 3);

        Assert.That(replies, Is.EqualTo(new[] { "ACK:" + id, "ACK:" + id, "ACK:" + id }));
        Assert.That(ReadingRepository.Instance.GetByClient(id), Has.Count.EqualTo(3));
    }

    [Test]
    public async Task TwelveClientsAtOnce_AllReadingsAreStored()
    {
        var ids = Enumerable.Range(1, 12).Select(i => $"flow-{i:D2}-{Guid.NewGuid()}").ToList();

        // 12 meters connect and send 5 readings each at the same time
        var all = Task.WhenAll(ids.Select(id => SendReadingsAsync(id, 5)));
        var finished = await Task.WhenAny(all, Task.Delay(15000));

        Assert.That(finished, Is.SameAs(all), "The 12 clients did not finish within 15 seconds");
        await all;

        foreach (var id in ids)
            Assert.That(ReadingRepository.Instance.GetByClient(id), Has.Count.EqualTo(5));
    }
}