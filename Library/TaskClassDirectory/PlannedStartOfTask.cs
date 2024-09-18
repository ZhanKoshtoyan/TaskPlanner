namespace Library.TaskClassDirectory;

public record PlannedStartOfTask
{
    public DateTime BasedOnTheDateOfShipment { get; set; }
    public DateTime AccordingToDepartmentResources { get; set; }
}
