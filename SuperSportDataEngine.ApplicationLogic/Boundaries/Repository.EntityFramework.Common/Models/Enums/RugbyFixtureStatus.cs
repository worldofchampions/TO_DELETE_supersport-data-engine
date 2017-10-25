namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums
{
    // Note: If/when extending this, do not change any existing enum int values, as these are being persisted at database level.
    public enum RugbyFixtureStatus
    {
        PreMatch = 0,
        FirstHalf = 1,
        HalfTime = 2,
        SecondHalf = 3,
        FullTime = 4,
        ExtraTime = 5,
        Result = 6
    }
}