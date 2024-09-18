using System.Text.Json.Serialization;

namespace Library.DateDirectory;

public class CalendarYear
{
    [JsonInclude]
    public int Year { get; set; }
    public bool IsLeap => DateTime.IsLeapYear(Year);
    [JsonInclude]
    public List<DateTime> PublicHolidays { get; set; }
    [JsonInclude]
    public List<DateTime> PublicWorkingDays { get; set; }

    public CalendarYear(int year, List<DateTime> publicHolidays, List<DateTime> publicWorkingDays)
    {
        Year = year;
        PublicHolidays = publicHolidays;
        PublicWorkingDays = publicWorkingDays;
    }

    public void AddPublicHoliday(DateTime holiday)
    {
        PublicHolidays.Add(holiday);
    }

    public void AddPublicWorkingDay(DateTime date)
    {
        PublicWorkingDays.Add(date);
    }

    public bool IsWorkingDay(DateTime date) =>
        PublicWorkingDays.Contains(date) ||
        date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);

    public bool IsWeekend(DateTime date) =>
        PublicHolidays.Contains(date) ||
        date.DayOfWeek is (DayOfWeek.Saturday or DayOfWeek.Sunday);
}
