using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

public class PipeClientExample
{
    private const string PipeName = "MyTestPipe";
    private const string ServerName = "."; // "." означает локальный компьютер

    public static async Task StartClientAsync(string messageToSend)
    {
        try
        {
            // Создаем клиентский именованный канал и подключаемся
            using (NamedPipeClientStream pipeClient =
                   new NamedPipeClientStream(ServerName, PipeName, PipeDirection.InOut, PipeOptions.Asynchronous))
            {
                Console.WriteLine($"Attempting to connect to pipe '{PipeName}' on server '{ServerName}'...");
                await pipeClient.ConnectAsync(5000); // Попытка подключения с таймаутом 5 секунд
                Console.WriteLine("Connected to pipe.");
                Console.WriteLine($"There are currently {pipeClient.NumberOfServerInstances} pipe server instances open.");

                using (StreamReader sr = new StreamReader(pipeClient, Encoding.UTF8, true, 1024, true))
                using (StreamWriter sw = new StreamWriter(pipeClient, Encoding.UTF8, 1024, true))
                {
                    sw.AutoFlush = true;

                    // Отправка сообщения серверу
                    Console.WriteLine($"Client sending: {messageToSend}");
                    await sw.WriteLineAsync(messageToSend);

                    // Чтение ответа от сервера
                    string serverResponse = await sr.ReadLineAsync();
                    Console.WriteLine($"Client received: {serverResponse}");
                }
            }
            Console.WriteLine("Client shutting down.");
        }
        catch (TimeoutException)
        {
            Console.WriteLine($"ERROR (Client): Could not connect to pipe server within the timeout period.");
        }
        catch (IOException e)
        {
            Console.WriteLine($"ERROR (Client): {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"An unexpected error occurred (Client): {e.Message}");
        }
    }

    public static async Task Main(string[] args)
    {
        await StartClientAsync("Hello from lents the pipe client!");
        Console.WriteLine("Press Enter to exit client.");
        Console.ReadLine();
    }
}