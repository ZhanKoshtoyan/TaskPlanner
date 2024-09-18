using Library.TaskClassDirectory;
using Library.DateDirectory;
using System.Text.Json.Serialization;

namespace Library.TaskPerformerDirectory;

public class TaskPerformerClass
{
    public TaskPerformerClass(
        FullNameOfThePerson fullName,
        WorkDay workDay,
        CalendarYearOfTaskPerformer calendarYearOfTaskPerformer,
        TaskClass? actualTask = null)
    {
        FullName = fullName;
        WorkDay = workDay;
        CalendarYearOfTaskPerformer = calendarYearOfTaskPerformer;
        ActualTaskClass = actualTask;
    }
    /// <summary>
    /// Полное имя исполнителя
    /// </summary>
    [JsonInclude]
    public FullNameOfThePerson FullName { get; set; }
    /// <summary>
    /// Рабочий день
    /// </summary>
    [JsonInclude]
    public WorkDay WorkDay { get; set; }
    /// <summary>
    /// Календарный год исполнителя
    /// </summary>
    [JsonInclude]
    public CalendarYearOfTaskPerformer CalendarYearOfTaskPerformer { get; set; }

    public Status Status
    {
        get => ActualTaskClass is not null ? Status.Busy : Status.Free;
        set => throw new NotImplementedException();
    }

    /// <summary>
    /// Текущая активная задача
    /// </summary>
    [JsonInclude]
    public TaskClass? ActualTaskClass { get; set; }
}
