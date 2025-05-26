// Пример (очень упрощенный, без синхронизации!)

// Процесс-писатель
using System.IO.MemoryMappedFiles;

try
{
    // Создаем или открываем именованную общую память
    using (MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("MySharedMemory", 1024)) // 1024 байта
    {
        // Создаем accessor для доступа к памяти
        using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
        {
            string message = "Hello from Process A! Current time: " + DateTime.Now;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            accessor.WriteArray(0, buffer, 0, buffer.Length); // Записываем в начало
            Console.WriteLine("Process A: Wrote to shared memory.");
            // Здесь нужна синхронизация, чтобы уведомить читателя!
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("Process A Error: " + ex.Message);
}

// Процесс-читатель (запускается отдельно)
try
{
    // Открываем существующую именованную общую память
    using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("MySharedMemory"))
    {
        using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
        {
            byte[] buffer = new byte[1024];
            accessor.ReadArray(0, buffer, 0, buffer.Length); // Читаем из начала
            string message = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            Console.WriteLine("Process B: Read from shared memory: " + message);
        }
    }
}
catch (FileNotFoundException)
{
    Console.WriteLine("Process B: Shared memory not found. Run Process A first.");
}
catch (Exception ex)
{
    Console.WriteLine("Process B Error: " + ex.Message);
}
