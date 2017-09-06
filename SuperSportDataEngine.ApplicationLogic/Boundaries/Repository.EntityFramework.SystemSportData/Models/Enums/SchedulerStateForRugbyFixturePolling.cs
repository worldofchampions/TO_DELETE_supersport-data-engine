namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums
{
    // Note: If/when extending this, do not change any existing enum int values, as these are being persisted at database level.
    public enum SchedulerStateForRugbyFixturePolling
    {
        SchedulingNotYetStarted = 0,
        PreLivePolling = 1,
        LivePolling = 2,
        PostLivePolling = 3,
        ResultOnlyPolling = 4,
        SchedulingCompleted = 5
    }
}