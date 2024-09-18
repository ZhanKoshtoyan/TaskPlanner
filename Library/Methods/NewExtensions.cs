using Library.TaskPerformerDirectory;

namespace Library.Methods;

public static class NewExtension
{
    /// <summary>
    /// Вычет полных дней с учетом выходных дней исполнителя. Время игнорируется.
    /// </summary>
    /// <param name="currentDateTime">Дата, из которой следует вычесть</param>
    /// <param name="workingDays">Количество дней для вычета</param>
    /// <param name="taskPerformerClass">Исполнитель</param>
    /// <param name="action">"AddDays" = Проверка в сторону добавления дней, "SubtractDays" = проверка в сторону вычитания дней</param>
    /// <returns>объект DataTime</returns>
    private static DateTime SubtractWeekends(this DateTime currentDateTime, int workingDays,
    TaskPerformerClass taskPerformerClass, string action)
    {
        if (action != "AddDays" && action != "SubtractDays")
        {
            throw new Exception("Введено неверное слово. Должно быть 'AddDays' или 'SubtractDays'.");
        }

        var sign = 1;
        if (action == "SubtractDays")
        {
            sign = -1;
        }

        var resultDateTime = currentDateTime;
        for (var i = 0; i < workingDays; i++)
        {
            resultDateTime = resultDateTime.AddDays(sign);
            // Проверка, является ли день рабочим для исполнителя
            while (taskPerformerClass.CalendarYearOfTaskPerformer.IsWeekend(resultDateTime))
            {
                resultDateTime = resultDateTime.AddDays(sign);
            }
        }
        return resultDateTime;
    }

    private static TimeOnly DecomposeIntoTimeOnly(this int totalSeconds)
    {
        if (totalSeconds < 0)
        {
            throw new ArgumentException("Время не может быть отрицательным.");
        }

        return new TimeOnly(totalSeconds / 3600, (totalSeconds % 3600) / 60, totalSeconds % 60);
    }

    /// <summary>
    /// Учет DinnerTime относительно момента времени timeOnlyForCheck
    /// </summary>
    /// <param name="timeOnlyForCheck">Дата для проверки добавления DinnerTime</param>
    /// <param name="durationOfTaskAtDay">Продолжительность задачи в течении дня</param>
    /// <param name="taskPerformerClass">Исполнитель</param>
    /// <param name="side">"right" = Время задачи справа от timeOnlyForCheck. "left" = Время задачи слева от timeOnlyForCheck.</param>
    /// <returns>Длительность DinnerTime</returns>
    /// <exception cref="Exception">Учет DinnerTime неверен</exception>
    private static TimeSpan AddDinnerTimeToLaborTime(
        this TimeOnly timeOnlyForCheck,
        TimeSpan durationOfTaskAtDay,
        TaskPerformerClass taskPerformerClass,
        string side
    )
    {
        if (side != "right" && side != "left")
        {
            throw new Exception("Введено неверное слово. Должно быть 'right' или 'left'.");
        }

        TimeSpan result;
        if (timeOnlyForCheck != taskPerformerClass.WorkDay.StartOfDinner &&
            timeOnlyForCheck != taskPerformerClass.WorkDay.EndOfDinner &&
            timeOnlyForCheck.IsBetween(taskPerformerClass.WorkDay.StartOfDinner, taskPerformerClass.WorkDay.EndOfDinner))
        {
            switch (side)
            {
                case "right":
                    result = taskPerformerClass.WorkDay.EndOfDinner.ToTimeSpan()
                        .Subtract(timeOnlyForCheck.ToTimeSpan());
                    return result;
                case "left":
                    result = timeOnlyForCheck.ToTimeSpan()
                        .Subtract(taskPerformerClass.WorkDay.StartOfDinner.ToTimeSpan());
                    return result;
            }
        }

        var zeroAsTimeSpan = new TimeSpan(0,0,0);
        switch (side)
        {
            case "right":
                if (timeOnlyForCheck <= taskPerformerClass.WorkDay.StartOfDinner &&
                    taskPerformerClass.WorkDay.StartOfDinner.ToTimeSpan().Subtract(timeOnlyForCheck.ToTimeSpan()) <
                    durationOfTaskAtDay)
                {
                    result = taskPerformerClass.WorkDay.DurationOfDinner;
                }
                else
                {
                    result = zeroAsTimeSpan;
                }
                break;
            case "left":
                if (timeOnlyForCheck >= taskPerformerClass.WorkDay.EndOfDinner &&
                    timeOnlyForCheck.ToTimeSpan().Subtract(taskPerformerClass.WorkDay.EndOfDinner.ToTimeSpan()) <
                    durationOfTaskAtDay)
                {
                    result = taskPerformerClass.WorkDay.DurationOfDinner;
                }
                else
                {
                    result = zeroAsTimeSpan;
                }
                break;
            default:
                throw new Exception("Учет DinnerTime неверен.");
        }

        return result;
    }

    public static DateTime ReturnStartOfTaskAsDateTime(
        this TimeSpan relativeLaborAsTimeSpan,
        DateTime endOfTask,
        TaskPerformerClass taskPerformerClass
    )
    {
        DateTime startOfTask;

        //Проверка на пересечение с обедом в последний день
        var dinnerTimeLastDay =
            TimeOnly.FromDateTime(endOfTask).AddDinnerTimeToLaborTime(relativeLaborAsTimeSpan, taskPerformerClass, "left");

        //Остаток времени в дне с учетом обеда
        var laborOfLastDay = TimeOnly.FromDateTime(endOfTask).ToTimeSpan().Subtract(taskPerformerClass.WorkDay
        .StartOfDay.ToTimeSpan() + dinnerTimeLastDay);

        //Трудозатраты больше остатка времени в дне?
        var laborMoreOneOfDay = relativeLaborAsTimeSpan > laborOfLastDay;

        switch (laborMoreOneOfDay)
        {
            case true:
                //Остаток трудоемкости без учета последнего дня
                var theRemainderOfLabor = relativeLaborAsTimeSpan - laborOfLastDay;
                // Полных дней в остатке трудоемкости без учета последнего дня. Дробное число округлено вверх до ближайшего целого.
                var wholeWorkingDaysForLabor = Convert.ToInt32(Math.Ceiling(theRemainderOfLabor.Divide(taskPerformerClass.WorkDay.DurationOfDay)));
                var theRemainderOfLaborWithoutWholeWorkingDaysForLabor = theRemainderOfLabor - taskPerformerClass
                .WorkDay.DurationOfDay.Multiply(wholeWorkingDaysForLabor - 1);
                //Начало задачи в первый день
                var startOfTaskOfFirstDay = taskPerformerClass.WorkDay.EndOfDay.Add(-theRemainderOfLaborWithoutWholeWorkingDaysForLabor);
                //Проверка на пересечение с обедом в первый день
                var dinnerTimeFirstDay = startOfTaskOfFirstDay.AddDinnerTimeToLaborTime(
                    theRemainderOfLaborWithoutWholeWorkingDaysForLabor,
                    taskPerformerClass,
                    "right"
                );
                //Начало задачи в первый день с учетом обеда
                var laborOfFirstDayWithDinner = startOfTaskOfFirstDay.Add(-dinnerTimeFirstDay);

                //результирующая дата - начало выполнения задачи
                startOfTask = new DateTime(
                    endOfTask.Year,
                    endOfTask.Month,
                    endOfTask.Day,
                    laborOfFirstDayWithDinner.Hour,
                    laborOfFirstDayWithDinner.Minute,
                    laborOfFirstDayWithDinner.Second
                );

                //результирующая дата - начало выполнения задачи за вычетом полных дней трудоемкости задачи
                startOfTask = startOfTask.SubtractWeekends(wholeWorkingDaysForLabor, taskPerformerClass, "SubtractDays");

                if (laborOfFirstDayWithDinner < taskPerformerClass.WorkDay.StartOfDay)
                {
                    var excessOfLaborTime = taskPerformerClass.WorkDay.StartOfDay.ToTimeSpan().Subtract(
                        laborOfFirstDayWithDinner
                            .ToTimeSpan()
                    );
                    var startOfTaskOfFirstDayWithExcessOfLaborTime = taskPerformerClass.WorkDay.EndOfDay.Add(-excessOfLaborTime);
                    startOfTask = new DateTime(
                        startOfTask.Year,
                        startOfTask.Month,
                        startOfTask.Day,
                        startOfTaskOfFirstDayWithExcessOfLaborTime.Hour,
                        startOfTaskOfFirstDayWithExcessOfLaborTime.Minute,
                        startOfTaskOfFirstDayWithExcessOfLaborTime.Second
                    );
                    const int oneDay = 1;
                    startOfTask = startOfTask.SubtractWeekends(oneDay, taskPerformerClass, "SubtractDays");
                }
                break;
            case false:
                var startOfTaskLastDay = endOfTask.Add(-relativeLaborAsTimeSpan-dinnerTimeLastDay);
                startOfTask = new DateTime(
                    endOfTask.Year,
                    endOfTask.Month,
                    endOfTask.Day,
                    startOfTaskLastDay.Hour,
                    startOfTaskLastDay.Minute,
                    startOfTaskLastDay.Second
                );
                break;
        }

        return startOfTask;
    }

    public static DateTime ReturnEndOfTaskAsDateTime(
        this TimeSpan relativeLaborAsTimeSpan, //10:00
        DateTime startOfTask, //12.09.2024 12:00
        TaskPerformerClass taskPerformerClass
    )
    {
        DateTime endOfTask;

        //проверка на пересечение с обедом в первый день
        var dinnerTimeFirstDay = TimeOnly.FromDateTime(startOfTask).AddDinnerTimeToLaborTime(relativeLaborAsTimeSpan,
         taskPerformerClass, "right");

        //Остаток времени в дне с учетом обеда
        var laborOfFirstDay = taskPerformerClass.WorkDay.EndOfDay.ToTimeSpan().Subtract(
            TimeOnly.FromDateTime(startOfTask).ToTimeSpan() + dinnerTimeFirstDay
        );

        //Трудозатраты больше остатка времени в дне?
        var laborMoreOneOfDay = relativeLaborAsTimeSpan > laborOfFirstDay;

        switch (laborMoreOneOfDay)
        {
            case true:
                //Остаток трудоемкости без учета первого дня
                var theRemainderOfLabor = relativeLaborAsTimeSpan - laborOfFirstDay;
                // Полных дней в остатке трудоемкости без учета первого дня. Дробное число округлено вверх? до ближайшего целого.
                var wholeWorkingDaysForLabor = Convert.ToInt32(
                    Math.Ceiling(
                        theRemainderOfLabor.Divide
                        (taskPerformerClass.WorkDay.DurationOfDay)
                    )
                );
                var theRemainderOfLaborWithoutWholeWorkingDaysForLabor = theRemainderOfLabor - taskPerformerClass
                    .WorkDay.DurationOfDay.Multiply(wholeWorkingDaysForLabor - 1);
                //Конец задачи в последний день
                var endOfTaskOfLastDay =
                    taskPerformerClass.WorkDay.StartOfDay.Add(theRemainderOfLaborWithoutWholeWorkingDaysForLabor);
                //проверка на пересечение с обедом в последний день
                var dinnerTimeLastDay = endOfTaskOfLastDay.AddDinnerTimeToLaborTime(
                    theRemainderOfLaborWithoutWholeWorkingDaysForLabor,
                    taskPerformerClass,
                    "left"
                );
                //Конец задачи в последний день с учетом обеда
                var laborOfLastDayWithDinner = endOfTaskOfLastDay.Add(dinnerTimeLastDay);

                //Результирующая дата - конец выполнения задачи
                endOfTask = new DateTime(
                    startOfTask.Year,
                    startOfTask.Month,
                    startOfTask.Day,
                    laborOfLastDayWithDinner.Hour,
                    laborOfLastDayWithDinner.Minute,
                    laborOfLastDayWithDinner.Second
                );

                //результирующая дата - конец выполнения задачи с добавлением полных дней трудоемкости
                endOfTask = endOfTask.SubtractWeekends(wholeWorkingDaysForLabor, taskPerformerClass, "AddDays");

                //Если трудоемкость с учетом обеда больше длительности дня, то
                if (laborOfLastDayWithDinner > taskPerformerClass.WorkDay.EndOfDay)
                {
                    var excessOfLaborTime = laborOfLastDayWithDinner.ToTimeSpan()
                        .Subtract(taskPerformerClass.WorkDay.EndOfDay.ToTimeSpan());
                    var endOfTaskOfLastDayWithExcessOfLaborTime =
                        taskPerformerClass.WorkDay.StartOfDay.Add(excessOfLaborTime);
                    endOfTask = new DateTime(
                        startOfTask.Year,
                        startOfTask.Month,
                        startOfTask.Day,
                        endOfTaskOfLastDayWithExcessOfLaborTime.Hour,
                        endOfTaskOfLastDayWithExcessOfLaborTime.Minute,
                        endOfTaskOfLastDayWithExcessOfLaborTime.Second
                    );
                    const int oneDay = 1;
                    endOfTask = endOfTask.SubtractWeekends(oneDay, taskPerformerClass, "AddDays");
                }
                break;
            case false:
                var endOfTaskLastDay = startOfTask.Add(relativeLaborAsTimeSpan+dinnerTimeFirstDay);
                endOfTask = new DateTime(
                    startOfTask.Year,
                    startOfTask.Month,
                    startOfTask.Day,
                    endOfTaskLastDay.Hour,
                    endOfTaskLastDay.Minute,
                    endOfTaskLastDay.Second
                );
                break;
        }
        return endOfTask;
    }
}
