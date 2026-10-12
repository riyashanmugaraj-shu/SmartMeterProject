//https://www.youtube.com/watch?v=eYHOZhMA7OI
//https://github.com/mrconter1/Simple-TCP-Communication-SSLStream/blob/master/server.pfx
using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;


class ClientCode
{
    private const int PORT = 7000;

    static void Main()
    {
        try
        {
            TcpClient client = new TcpClient("127.0.0.1", PORT);
            Console.WriteLine("Client connected to server");

            NetworkStream stream = client.GetStream();

            SslStream sslStream = new SslStream(stream, false, new RemoteCertificateValidationCallback(CertificateValidationCallback));
            sslStream.AuthenticateAsClient("clientName");

            string mockMessage = "Reading-Meter 1-100.00kwh";
            Console.WriteLine("Sending encrypted message:" + mockMessage);

            //byte[] data = Encoding.UTF8.GetBytes(mockMessage);
            //stream.Write(data);

            //byte[] response = new byte[1024];
            //int byteRead = stream.Read(response, 0, response.Length);

            //string messageFromServer = Encoding.UTF8.GetString(response, 0, byteRead);
            //Console.WriteLine(messageFromServer);

            //write to the sslstream
            sslStream.Write(Encoding.UTF8.GetBytes(mockMessage), 0, mockMessage.Length);

            //read response
            byte[] response = new byte[1024];
            int byteRead = stream.Read(response, 0, response.Length);
            string messageFromServer = Encoding.UTF8.GetString(response, 0, byteRead);
            Console.WriteLine(messageFromServer);

            Console.WriteLine("closing client connection");




        }
        catch (Exception ex)
        {
            Console.WriteLine("Error:" + ex.Message);
        }

    }
    static bool CertificateValidationCallback( object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}

