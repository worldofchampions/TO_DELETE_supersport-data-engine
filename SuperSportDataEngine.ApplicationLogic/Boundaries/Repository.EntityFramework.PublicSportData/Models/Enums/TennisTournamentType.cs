namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums
{
    // Note: If/when extending this, do not change any existing enum int values, as these are being persisted at database level.
    public enum TennisTournamentType
    {
        Unknown = 0,
        ATP = 1,
        WTA = 2,
        GS = 3, // Grand Slam
        CO = 4 // Co-sanctioned
    }
}
