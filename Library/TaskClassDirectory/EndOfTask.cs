namespace Library.TaskClassDirectory;

public class EndOfTask
{
    public EndOfTask(DateTime plannedEndOfTask, DateTime actual, DateTime required)
    {
        Planned = plannedEndOfTask;
        Actual = actual;
        Required = required;
    }

    public DateTime Planned { get; set; }
    public DateTime Actual { get; set; }
    public DateTime Required { get; set; }
}
