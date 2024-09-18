using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Library.Json;

public class JsonLoader
{
    public const string PathCalendarYearsJsonFile = "C:\\My ProjectCSharp\\TaskPlanner\\Library\\CalendarYears.json";
    public static string? Response { get; private set; }

    public static async Task SerializeAsync<T>(List<T> data, string pathJsonFile)
    {
        if (File.Exists(pathJsonFile))
        {
            /*Console.WriteLine("Файл уже существует. Хотите перезаписать его? (Y/N)");
            Response = Console.ReadLine()?.ToUpper();
            if (Response != "Y")
            {
                return;
            }*/
            Console.WriteLine("Файл уже существует. Он будет перезаписан");
        }

        var options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(
                UnicodeRanges.BasicLatin,
                UnicodeRanges.Cyrillic
            )
        };

        try
        {
            // Используем MemoryStream для временного хранения данных
            using var memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, data, options);

            // Сохраняем данные из MemoryStream в файл
            await File.WriteAllBytesAsync(pathJsonFile, memoryStream.ToArray());

            // Читаем содержимое файла и выводим в консоль
            var jsonContent = await File.ReadAllTextAsync(pathJsonFile);
            Console.WriteLine("Содержимое JSON файла:");
            Console.WriteLine(jsonContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    public static async Task<List<T>?> DeserializeAsync<T>(string pathJsonFile)
    {
        // Проверяем, существует ли файл
        if (!File.Exists(pathJsonFile))
        {
            Console.WriteLine("Файл не найден.");
            return null;
        }

        var options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(
                UnicodeRanges.BasicLatin,
                UnicodeRanges.Cyrillic
            )
        };

        try
        {
            var jsonContent = await File.ReadAllTextAsync(pathJsonFile);
            using (JsonDocument doc = JsonDocument.Parse(jsonContent))
            {
                if (doc.RootElement.ValueKind != JsonValueKind.Array)
                {
                    Console.WriteLine("Корневой элемент не является массивом.");
                    return null;
                }
            }

            // Используем FileStream для асинхронного чтения файла
            await using var fileStream = new FileStream(
                pathJsonFile,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
            );

            // Десериализуем данные из файла в список объектов
            var data = await JsonSerializer.DeserializeAsync<List<T>?>(
                fileStream,
                options
            );

            return data;
        }
        catch (JsonException jsonEx)
        {
            Console.WriteLine($"Ошибка десериализации: {jsonEx.Message}");
            return null;
        }
        catch (InvalidCastException castEx)
        {
            Console.WriteLine($"Ошибка приведения типов: {castEx.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
            return null;
        }
    }
}
