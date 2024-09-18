namespace Library.DateDirectory;

public class LaborTime
{
    public LaborTime(int hours, int minutes, int seconds = 0)
    {
        if (hours < 0 || minutes < 0 || seconds < 0)
        {
            throw new ArgumentException("Время не может быть отрицательным.");
        }

        Hours = hours;

        if (minutes > 60)
        {
            throw new Exception("Минуты не могут быть больше 60");
        }

        if (seconds > 60)
        {
            throw new Exception("Секунды не могут быть больше 60");
        }

        Minutes = minutes;
        Seconds = seconds;
        TotalSeconds = hours * 3600 + minutes * 60 + seconds;
    }

    public LaborTime(double totalHours)
    {
        if (totalHours < 0)
        {
            throw new ArgumentException("Время не может быть отрицательным.");
        }

        TotalSeconds = (int)(totalHours * 3600);

        Hours = TotalSeconds / 3600;
        Minutes = (TotalSeconds % 3600) / 60;
        Seconds = TotalSeconds % 60;
    }
    public int Hours { get; }
    public int Minutes { get; }
    public int Seconds { get; }
    public int TotalSeconds { get; }
}
