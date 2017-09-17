namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums
{
    // Note: If/when extending this, do not change any existing enum int values, as these are being persisted at database level.
    public enum RugbyFixtureStatus
    {
        // TODO: Define the corresponding states (e.g. pre-match, in progress, final, cancellation, postmatch etc.)
        PreMatch = 0,
        InProgress = 1,
        Final = 2,
        GameEnd = 3
    }
}