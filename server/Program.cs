//https://www.youtube.com/watch?v=eYHOZhMA7OI
//https://github.com/mrconter1/Simple-TCP-Communication-SSLStream/blob/master/server.pfx
using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SmartMeter.Shared;


public class MultiClientServer
{
    //listens to client connection requests
    private static TcpListener? server;

    private const int PORT = 7000;
    private static readonly IreadingRepository _repository = ReadingRepository.Instance;
    // singleton - the single instance is reached through ReadingRepository
    // every client handler writes to this same store

    private static readonly BillingCalculator _billingCalculator = new BillingCalculator(new StandardBillingStrategy(0.25m));

    static async Task Main()
    {
        server = new TcpListener(IPAddress.Any, PORT);
        server.Start();
        Console.WriteLine("Server has started listening on port: " + PORT);

        //Random random= new Random();
        //double Kwh = 300.00;
        //Console.WriteLine("Smart meter reading has started");


        while (true)
        {
            try
            {
                var client = await server.AcceptTcpClientAsync();

                Console.WriteLine(
                    "Client successfully connected");

                _ = HandleClientAsync(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Server connection error: " + ex.Message);

                await CommunicationLogger.LogErrorAsync(
                    "Error accepting a client connection",
                    ex);
            }
        }





    }


    private static async Task HandleClientAsync(TcpClient client)
    {


        try
        {

            using (NetworkStream stream = client.GetStream())

            using (SslStream sslStream = new SslStream(stream, false))
            {
                var certificate = new X509Certificate2("server.pfx", "password");


                await sslStream.AuthenticateAsServerAsync(certificate, clientCertificateRequired: false, enabledSslProtocols: System.Security.Authentication.SslProtocols.Tls12
                    | System.Security.Authentication.SslProtocols.Tls13, checkCertificateRevocation: false);

                //string? message;

                //byte[] buffer = new byte[1024];
                //int byteRead = await sslStream.ReadAsync(buffer, 0, buffer.Length);

                //if (byteRead > 0)
                //{
                //    message = Encoding.UTF8.GetString(buffer, 0, byteRead);
                //    Console.WriteLine("Message from the client:" + message);

                //    byte[] response = Encoding.UTF8.GetBytes("Server recieved:" + message);
                //    await sslStream.WriteAsync(response, 0, response.Length);
                //}

                using var reader = new StreamReader(sslStream, Encoding.UTF8);
                using var writer = new StreamWriter(sslStream, Encoding.UTF8) { AutoFlush = true };
                // keep reading readings from this client for as long as the connection stays open
                // ReadLineAsync returns null only when the client disconnects, which ends the loop
                var adapter = new JsonReadingAdapter();

                string? line;
                while ((line = await reader.ReadLineAsync()) != null)

                {
                    // TEMPORARY TEST - REMOVE AFTER TESTING
                    // throw new IOException("TEST: Simulated communication failure");

                    Reading reading = adapter.Deserialize(line);

                    Console.WriteLine(
                        $"Reading received: {reading.ClientId} - " +
                        $"{reading.Kwh} Kwh at {reading.Timestamp:HH:mm:ss}");

                    _repository.Add(reading);

                    Console.WriteLine(
                        $"Stored. Total readings: {_repository.Count}");

                    // This gets all readings belonging to this client
                    var clientReadings = _repository.GetByClient(reading.ClientId);

                    // Then calculates the current bill using the Strategy pattern
                    decimal liveBill =
                        _billingCalculator.CalculateBill(clientReadings);

                    Console.WriteLine(
                        $"Live bill for {reading.ClientId}: £{liveBill:F2}");

                    await writer.WriteLineAsync(
                        "ACK:" + reading.ClientId);

                    // Pushes the updated bill back to the client
                    await writer.WriteLineAsync(
                        $"BILL:{reading.ClientId}:£{liveBill:F2}");
                }
            }

        }

        catch (Exception ex)
        {
            Console.WriteLine(
                "Communication error: " + ex.Message);

            await CommunicationLogger.LogErrorAsync(
                "Communication error with client",
                ex);
        }
        finally
        {
            client.Close();
            Console.WriteLine(
                "Client Disconnected Successfully");
        }

    }

}
