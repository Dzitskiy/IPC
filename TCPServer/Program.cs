using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class TcpServerExample
{
    public static async Task StartServerAsync(int port)
    {
        TcpListener server = null;
        try
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1"); // Слушаем на локальном интерфейсе
            server = new TcpListener(localAddr, port);
            server.Start();
            Console.WriteLine($"Server started on port {port}. Waiting for a connection...");

            while (true) // Бесконечный цикл для приема нескольких клиентов (упрощенно)
            {
                TcpClient client = await server.AcceptTcpClientAsync(); // Ожидание подключения клиента
                Console.WriteLine($"Client connected from {((IPEndPoint)client.Client.RemoteEndPoint).Address}");

                // Обработка клиента в отдельном потоке/задаче, чтобы не блокировать прием новых
                _ = HandleClientAsync(client);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e.Message}");
        }
        finally
        {
            server?.Stop();
        }
    }

    private static async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                // Чтение данных от клиента
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received from client: {dataReceived}");

                    // Эхо-ответ клиенту
                    string responseMessage = $"Server received: {dataReceived.ToUpper()} at {DateTime.Now:HH:mm:ss}";
                    byte[] responseBuffer = Encoding.UTF8.GetBytes(responseMessage);
                    await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
                    Console.WriteLine($"Sent to client: {responseMessage}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error handling client: {e.Message}");
        }
        finally
        {
            client.Close();
            Console.WriteLine("Client disconnected.");
        }
    }

    public static async Task Main(string[] args)
    {
        await StartServerAsync(8888);
    }
}
