namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums
{
    public enum SchedulerStateForMotorsportRaceEventPolling
    {
        PollingNotStarted = 0,
        PreLivePolling = 1,
        LivePolling = 2,
        PostLivePolling = 3,
        PollingFinished = 4
    }
}