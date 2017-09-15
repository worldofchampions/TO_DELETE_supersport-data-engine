namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums
{
    // Note: If/when extending this, do not change any existing enum int values, as these are being persisted at database level.
    public enum SchedulerStateForManagerJobPolling
    {
        Undefined = 0,
        NotRunning = 1,
        Running = 2,
    }
}