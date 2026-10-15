//https://www.youtube.com/watch?v=eYHOZhMA7OI
//https://github.com/mrconter1/Simple-TCP-Communication-SSLStream/blob/master/server.pfx
using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


public class MultiClientServer
{
    //listens to client connection requests
    private static TcpListener? server;

    private const int PORT = 7000;

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
            var client = await server.AcceptTcpClientAsync();
            Console.WriteLine("Client successfully connected");

            //HandleClientAsync(client);
            _ = Task.Run(() => HandleClientAsync(client));
        }





    }


    private static async void HandleClientAsync(TcpClient client)
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
                // Keep reading readings from this client for as long as the connection stays open.
                // ReadLineAsync returns null only when the client disconnects, which ends the loop.

                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    Console.WriteLine("Reading received: " + line);

                    // temporary acknowledgement for testing — replace with the actual bill
                    // calculation/push once the service layer exists
                    await writer.WriteLineAsync("ACK:" + line);
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error:" + ex.Message);
        }
        finally
        {
            client.Close();
            Console.WriteLine("Client Disconnected Successfully");
        }









    }

}
