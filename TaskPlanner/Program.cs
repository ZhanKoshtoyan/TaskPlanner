using Library;
using Library.DateDirectory;
using Library.Json;
using Library.Methods;
using Library.TaskClassDirectory;
using Library.TaskPerformerDirectory;
using System.Globalization;

namespace TaskPlanner;

internal static class Program
{
    private static async Task Main()
    {
        // Создаем новый календарный год
        var year2024 = new CalendarYear(
            2024,
            new List<DateTime>
            {
                new(2024,1,1)
            },
            new List<DateTime>
            {
                new(2024, 5, 10)
            });

        var calendarYearList = new List<CalendarYear>();
        if (File.Exists(JsonLoader.PathCalendarYearsJsonFile))
        {
            calendarYearList = await JsonLoader.DeserializeAsync<CalendarYear>(JsonLoader.PathCalendarYearsJsonFile)
            ?? throw new Exception("Ошибка при десериализации");
        }

        calendarYearList.Add(year2024);
        await JsonLoader.SerializeAsync(calendarYearList, JsonLoader.PathCalendarYearsJsonFile);

        // Проверяем, существует ли текущий год в json
        var currentYear = DateTime.Now.Year;

        var indexIfYear = calendarYearList.FindIndex(i => Equals(i.Year, currentYear));
        if (indexIfYear == -1)
        {
            Console.WriteLine($"Текущий год {currentYear} отсутствует в файле {JsonLoader.PathCalendarYearsJsonFile}. Для дальнейшей работы нужно добавить новый год в файл.");
        }

        //Создаем нового исполнителя
        var taskPerformerClassExample =
            new TaskPerformerClass(
                new FullNameOfThePerson("Иван", "Иванов"),
                new WorkDay(
                    new TimeOnly(9, 0),
                    new TimeOnly(18, 0),
                    new TimeOnly(13, 0),
                    new TimeSpan(1, 0, 0)
                ),
                new CalendarYearOfTaskPerformer(
                    calendarYearList[indexIfYear],
                    new List<DateTime>(),
                    new List<DateTime>()
                )
            );

        var taskPerformer1 = new TaskPerformerClass(
            new FullNameOfThePerson("Иван", "Иванов"),
            new WorkDay(
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new TimeOnly(13, 0),
                new TimeSpan(1, 0, 0)),
            new CalendarYearOfTaskPerformer(calendarYearList[indexIfYear],
                new List<DateTime>
                {
                   new(2024, 2, 1),
                   new(2024, 2, 2),
                   new(2024, 2, 3),
                   new(2024, 2, 4),
                   new(2024, 2, 5),
                   new(2024, 2, 6),
                   new(2024, 2, 7)
                },
                new List<DateTime>
                {
                    new(2024,2, 9)
                })

        );

        //Создаем нового исполнителя
        var taskPerformer2 = new TaskPerformerClass(
            new FullNameOfThePerson("Петя", "Петров"),
            new WorkDay(
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new TimeOnly(13, 0),
                new TimeSpan(1, 0, 0)),
            new CalendarYearOfTaskPerformer(calendarYearList[indexIfYear],
                new List<DateTime>
                {
                    new(2024, 2, 1),
                    new(2024, 2, 2),
                    new(2024, 2, 3),
                    new(2024, 2, 4),
                    new(2024, 2, 5),
                    new(2024, 2, 6),
                    new(2024, 2, 7)
                },
                new List<DateTime>
                {
                    new(2024,2, 9)
                })

        );

        var plannedEndOfTask1 = new DateTime(2024,09,09,16, 00, 0);
        var laborTimeSpan1 = new TimeSpan(14, 40, 0);
        var resultStartOfTask1 = laborTimeSpan1.ReturnStartOfTaskAsDateTime(plannedEndOfTask1, taskPerformer1);
        Console.WriteLine($"Результирующая дата начала задачи: {resultStartOfTask1}");
        var resultEndOfTask1 = laborTimeSpan1.ReturnEndOfTaskAsDateTime(plannedEndOfTask1, taskPerformer1);
        Console.WriteLine($"Результирующая дата конца задачи: {resultEndOfTask1}");

        var plannedEndOfTask2 = new DateTime(2024,09,09,13, 20, 0);
        var laborTimeSpan2 = new TimeSpan(9, 0, 0);
        var resultStartOfTask2 = laborTimeSpan2.ReturnStartOfTaskAsDateTime(plannedEndOfTask2, taskPerformer2);
        Console.WriteLine($"Результирующая дата начала задачи: {resultStartOfTask2}");
        var resultEndOfTask2 = laborTimeSpan2.ReturnEndOfTaskAsDateTime(plannedEndOfTask2, taskPerformer2);
        Console.WriteLine($"Результирующая дата конца задачи: {resultEndOfTask2}");

        var today = DateTime.Today;

        TimeSpan plannedLaborCost1;
        TimeSpan actualLaborCost1;
        DateTime requiredEndOfTask1;
        string? input;
        string formatForDataTime = "dd.MM.yyyy HH:mm"; // Формат даты и времени
        CultureInfo provider = CultureInfo.InvariantCulture; // Культура для парсинга

        while (true)
        {
            Console.Write("Введите планируемую трудоемкость задачи (например, 01:30:00 для 1 часа 30 минут): ");
            input = Console.ReadLine();

            // Проверяем, можно ли преобразовать введенное значение в TimeSpan
            if (TimeSpan.TryParse(input, out plannedLaborCost1))
            {
                Console.WriteLine($"Успешно введено: {plannedLaborCost1}");
                break; // Выход из цикла, если ввод корректен
            }
            Console.WriteLine("Ошибка: введенное значение не может быть преобразовано в TimeSpan. Пожалуйста, попробуйте снова.");
        }
        while (true)
        {
            Console.Write("Введите фактическую трудоемкость задачи (например, 01:30:00 для 1 часа 30 минут): ");
            input = Console.ReadLine();

            // Проверяем, можно ли преобразовать введенное значение в TimeSpan
            if (TimeSpan.TryParse(input, out actualLaborCost1))
            {
                Console.WriteLine($"Успешно введено: {actualLaborCost1}");
                break; // Выход из цикла, если ввод корректен
            }
            Console.WriteLine("Ошибка: введенное значение не может быть преобразовано в TimeSpan. Пожалуйста, попробуйте снова.");
        }
        while (true)
        {
            Console.Write("Введите окончание работ (формат: dd.MM.yyyy HH:mm): ");
            input = Console.ReadLine();

            // Попытка преобразовать введенное значение в DateTime
            if (DateTime.TryParseExact(input, formatForDataTime, provider, DateTimeStyles.None, out requiredEndOfTask1))
            {
                Console.WriteLine($"Вы ввели корректную дату и время: {requiredEndOfTask1}");
                break; // Выход из цикла, если ввод корректен
            }
            Console.WriteLine("Ошибка: Введенное значение не соответствует формату. Пожалуйста, попробуйте снова.");
        }

        var performerList = new List<TaskPerformerClass>
        {
            taskPerformer1
        };
        
        plannedLaborCost1 = new TimeSpan(30, 0, 0); //Вручную
        actualLaborCost1 = new TimeSpan(26, 0, 0); //Битрикс
        requiredEndOfTask1 = new DateTime(2024, 09, 20, 09, 0, 0); //Аксапта
        var actualStartOfTask1 = new DateTime(2024, 09, 18); //Битрикс
        var actualEndOfTask1 = new DateTime(2024, 09, 18, 18, 0, 0); //Битрикс
        var plannedStartOfTask1AccordingToDepartmentResources = new DateTime(2024, 09, 17);
        var theNearestFreePerformer = performerList.First(p => p.Status == Status.Free);

        var taskClassList = new List<TaskClass>
        {

            new(
                "Разработка щита автоматики №1.0",
                plannedLaborCost1,
                actualLaborCost1,
                requiredEndOfTask1,
                plannedStartOfTask1AccordingToDepartmentResources,
                theNearestFreePerformer,
                actualStartOfTask1,
                actualEndOfTask1)

        };

        var plannedLaborCost2 = new TimeSpan(30, 0, 0); //Вручную
        var actualLaborCost2 = new TimeSpan(26, 0, 0); //Битрикс
        var requiredEndOfTask2 = new DateTime(2024, 09, 20, 09, 0, 0); //Аксапта
        var actualStartOfTask2 = new DateTime(2024, 09, 18); //Битрикс
        var actualEndOfTask2 = new DateTime(2024, 09, 18, 18, 0, 0); //Битрикс
        var plannedStartOfTask2AccordingToDepartmentResources = taskClassList.Min(f => f.EndOfTask.Planned);
        theNearestFreePerformer = performerList.First(p => p.Status == Status.Free);
        if (theNearestFreePerformer is null)
        {
            
        }
        theNearestFreePerformer = 


    }
}
