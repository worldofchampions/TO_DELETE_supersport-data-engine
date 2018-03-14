namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums
{
    // Note: If/when extending this, do not change any existing enum int values, as these are being persisted at database level.
    public enum MotorsportRaceEventStatus
    {
        // Typical race flow states.
        PreRace = 0,
        InProgress = 1,
        Result = 2,

        // Other special cases.
        Postponed = 3,
        Suspended = 4,
        Delayed = 5,
        Cancelled = 6,
        Unknown = -1
    }
}
