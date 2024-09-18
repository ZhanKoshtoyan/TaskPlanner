using Library.DateDirectory;

namespace Library.TaskClassDirectory;

public class Labor
{
    public Labor(TimeSpan plannedCost, TimeSpan actualCost)
    {
        PlannedCost = plannedCost;
        ActualCost = actualCost;
    }

    /// <summary>
    /// Планируемые трудозатраты
    /// </summary>
    public TimeSpan PlannedCost { get; set; }

    /// <summary>
    /// Фактические трудозатраты
    /// </summary>
    public TimeSpan ActualCost { get; set; }
}
