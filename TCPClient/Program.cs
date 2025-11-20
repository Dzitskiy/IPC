using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class TcpClientExample
{
    public static async Task StartClientAsync(string serverIp, int port, string message)
    {
        try
        {
            using (TcpClient client = new TcpClient())
            {
                Console.WriteLine($"Connecting to server {serverIp}:{port}...");
                await client.ConnectAsync(serverIp, port); // Подключение к серверу
                Console.WriteLine("Connected to server.");

                using (NetworkStream stream = client.GetStream())
                {
                    for (global::System.Int32 i = 0; i < 50; i++)
                    // Отправка данных серверу
                    byte[] dataToSend = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(dataToSend, 0, dataToSend.Length);
                    Console.WriteLine($"Sent to server: {message}");

                    // Получение ответа от сервера
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string responseData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received from server: {responseData}");
                }
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        finally
        {
            Console.WriteLine("Client finished.");
        }
    }
    public static async Task Main(string[] args)
    {
        await StartClientAsync("127.0.0.1", 8888, "Hello from client lents!");
        Console.ReadLine(); // Pause to see output
    }
}