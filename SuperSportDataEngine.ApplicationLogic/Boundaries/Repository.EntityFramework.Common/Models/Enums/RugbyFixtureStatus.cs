namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums
{
    // Note: If/when extending this, do not change any existing enum int values, as these are being persisted at database level.
    public enum RugbyFixtureStatus
    {
        PreMatch = 0,
        InProgress = 1,
        PostMatch = 2,
        Result = 3
    }
}