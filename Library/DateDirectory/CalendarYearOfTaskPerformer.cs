namespace Library.DateDirectory;

public class CalendarYearOfTaskPerformer
{
    public CalendarYearOfTaskPerformer(
        CalendarYear calendarYear,
        List<DateTime>? vacations = null,
        List<DateTime>? dayOffs = null)
    {
        _calendarYear = calendarYear;
        _vacations = vacations;
        _dayOffs = dayOffs;
    }

    private readonly CalendarYear _calendarYear;
    private readonly List<DateTime>? _vacations;
    private readonly List<DateTime>? _dayOffs;

    public bool IsWorkingDay(DateTime date) =>
        _calendarYear.IsWorkingDay(date) ||
        (!_vacations?.Contains(date) ?? false) ||
        (!_dayOffs?.Contains(date) ?? false);

    public bool IsWeekend(DateTime date) =>
        _calendarYear.IsWeekend(date) ||
        (_vacations?.Contains(date) ?? false) ||
        (_dayOffs?.Contains(date) ?? false);
}
