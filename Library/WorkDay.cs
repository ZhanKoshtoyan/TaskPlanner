namespace Library;

public class WorkDay
{
    public WorkDay(TimeOnly startOfDay, TimeOnly endOfDay, TimeOnly startOfDinner, TimeSpan durationOfDinner)
    {
        if (endOfDay < startOfDay)
        {
            throw new Exception("Конец рабочего дня не может быть меньше начала рабочего дня.");
        }

        StartOfDay = startOfDay;
        StartOfDinner = startOfDinner;
        EndOfDay = endOfDay;
        EndOfDinner = startOfDinner.Add(durationOfDinner);

        if (EndOfDinner < startOfDinner)
        {
            throw new Exception("Конец обеденного времени не может быть меньше начала обеденного времени.");
        }

        DurationOfDinner = EndOfDinner - startOfDinner;
        if (endOfDay <= startOfDinner || startOfDay >= EndOfDinner)
        {
            DurationOfDinner = new TimeSpan(0, 0, 0);
        }
        DurationOfDay = EndOfDay.ToTimeSpan().Subtract(StartOfDay.ToTimeSpan() + DurationOfDinner);
    }

    /*public int TotalSecondsOfWorkDayDuration => DurationOfDay.DecomposeIntoTotalSecondsOfDay();
    public int TotalSecondsOfStartWorkDay => StartOfDay.DecomposeIntoTotalSecondsOfDay();
    public int TotalSecondsOfEndWorkDay => EndOfDay.DecomposeIntoTotalSecondsOfDay();*/
    public double DurationOfDayAsTotalSeconds => DurationOfDay.TotalSeconds;
    public double StartOfDayAsTotalSeconds => StartOfDay.ToTimeSpan().TotalSeconds;
    public double EndOfDayAsTotalSeconds => EndOfDay.ToTimeSpan().TotalSeconds;
    public TimeOnly StartOfDay { get; set; }
    public TimeSpan DurationOfDay { get; set; }
    public TimeOnly EndOfDay { get; set; }
    public TimeOnly StartOfDinner { get; set; }
    public TimeSpan DurationOfDinner { get; set; }
    public TimeOnly EndOfDinner { get; set; }

    public bool IsDinner(TimeOnly timeOnly) => timeOnly.IsBetween(StartOfDinner, EndOfDinner);
}
