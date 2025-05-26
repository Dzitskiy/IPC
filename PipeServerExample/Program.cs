using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

public class PipeServerExample
{
    private const string PipeName = "MyTestPipe";

    public static async Task StartServerAsync()
    {
        try
        {
            // Создаем серверный именованный канал
            // PipeDirection.InOut - двунаправленный
            // 1 - максимальное количество экземпляров сервера (для этого примера)
            // PipeTransmissionMode.Byte - передача байтовым потоком
            using (NamedPipeServerStream pipeServer =
                   new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                Console.WriteLine($"NamedPipeServerStream created. Waiting for client connection on pipe '{PipeName}'...");
                await pipeServer.WaitForConnectionAsync(); // Ожидание подключения клиента
                Console.WriteLine("Client connected.");

                using (StreamReader sr = new StreamReader(pipeServer, Encoding.UTF8, true, 1024, true)) // true для leaveOpen
                using (StreamWriter sw = new StreamWriter(pipeServer, Encoding.UTF8, 1024, true)) // true для leaveOpen
                {
                    sw.AutoFlush = true; // Важно для немедленной отправки

                    // Чтение сообщения от клиента
                    string clientMessage = await sr.ReadLineAsync();
                    Console.WriteLine($"Server received: {clientMessage}");

                    // Отправка ответа клиенту
                    string serverResponse = $"Hello client! You said: '{clientMessage}'. Time: {DateTime.Now:HH:mm:ss}";
                    await sw.WriteLineAsync(serverResponse);
                    Console.WriteLine($"Server sent: {serverResponse}");
                }
            }
            Console.WriteLine("Server shutting down.");
        }
        catch (IOException e)
        {
            Console.WriteLine($"ERROR (Server): {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"An unexpected error occurred (Server): {e.Message}");
        }
    }

    public static async Task Main(string[] args)
    {
        await StartServerAsync();
    }
}