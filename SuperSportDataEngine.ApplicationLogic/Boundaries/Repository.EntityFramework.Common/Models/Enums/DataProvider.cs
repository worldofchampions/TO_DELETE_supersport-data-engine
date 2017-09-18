namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums
{
    /// <summary>
    /// Data Provider identification to track record references to a particular Data Provider.
    /// This is to cater for future scenarios once there is data for more than one Data Provider for a particular sport.
    /// </summary>
    // Note: If/when extending this, do not change any existing enum int values, as these are being persisted at database level.
    public enum DataProvider
    {
        StatsProzone = 1
    }
}