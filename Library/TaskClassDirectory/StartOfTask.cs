namespace Library.TaskClassDirectory;

public class StartOfTask
{
    public StartOfTask(PlannedStartOfTask planned, DateTime actual)
    {
        Planned = planned;
        Actual = actual;
    }

    public PlannedStartOfTask Planned { get; set; }
    public DateTime Actual { get; set; }
}
