namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums
{
    // Note: If/when extending this, do not change any existing enum int values, as these are being persisted at database level.
    public enum SchedulerStateForRugbyLogPolling
    {
        NotRunning = 0,
        RunningAt1MinuteCycle = 1,
        RunningAt15MinuteCycle = 2
    }
}