//https://www.youtube.com/watch?v=eYHOZhMA7OI
//https://github.com/mrconter1/Simple-TCP-Communication-SSLStream/blob/master/server.pfx
using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using SmartMeter.Shared;
using client;


class ClientCode
{
    private const int PORT = 7000;

    static async Task Main(string[] args)

        
    {// every client gets their own id from command line
        string clientID = args.Length > 0 ? args[0] : "client-01";
        try
        {
            TcpClient client = new TcpClient("127.0.0.1", PORT);
            Console.WriteLine($"{clientID} connected to server");

            //NetworkStream stream = client.GetStream();

            SslStream sslStream = new SslStream(client.GetStream(), false,
            new RemoteCertificateValidationCallback(CertificateValidationCallback));
            sslStream.AuthenticateAsClient("clientName");

            // print anything the server sends back (acknowledgements now, bills and alerts later)
            using var reader = new StreamReader(sslStream, Encoding.UTF8);
            _ = Task.Run(async () =>
            {
                string? reply;
                while ((reply = await reader.ReadLineAsync()) != null)
                    Console.WriteLine("Server says: " + reply);
            });

            var generator = new ReadingGenerator();
            var adapter = new JsonReadingAdapter();

            // Runs forever: generate a reading, send it as JSON, wait 15-60 seconds
            while (true)
            {
                Reading reading = generator.GenerateNext(clientID);
                string json = adapter.Serialize(reading);
                Console.WriteLine("Sending: " + json);

                byte[] data = Encoding.UTF8.GetBytes(json + "\n");
                await sslStream.WriteAsync(data, 0, data.Length);

                await Task.Delay(generator.NextDelayMs());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static bool CertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        // Accepts our self-signed development certificate
        return true;
    }
}

